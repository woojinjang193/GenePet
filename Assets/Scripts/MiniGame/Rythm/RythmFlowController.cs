using System;
using System.Collections.Generic;
using UnityEngine;

public class RythmFlowController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmConductor _conductor;
    [SerializeField] private RythmPicker _picker;
    [SerializeField] private RythmPresenter _presenter;
    [SerializeField] private RythmInput _input;

    [Header("레벨 설정")]
    [SerializeField] private RythmLevelPresetSO[] _levelPresets;
    [SerializeField] private int _totalRythmCount = 5;

    [Header("시작 대기 마디 수")]
    [SerializeField] private int _startWaitMeasure = 2;

    public event Action OnGameFinished;

    // ===== 레벨/패턴 상태 =====
    private int _levelIndex;
    private int _playedRythmCount;

    private RythmPatternSO _currentPattern;
    private List<RythmBeatTime> _currentBeats;
    private int _inputBeatIndex;

    // ===== 턴 상태 =====
    private bool _isSampleTurn;
    private int _turnRemainMeasures;
    private bool _sampleScheduled;

    // "현재 턴(샘플/입력)의 기준 DSP(0점)"
    private double _turnStartDspTime;

    // ---- 외부(스코어링)가 읽을 정보 ----
    public bool IsSampleTurn => _isSampleTurn;
    public bool IsInputTurn => !_isSampleTurn;
    public int PlayedRythmCount => _playedRythmCount;
    public int TotalRythmCount => _totalRythmCount;
    public int InputBeatIndex => _inputBeatIndex;
    public double TurnStartDspTime => _turnStartDspTime;

    public RythmLevelPresetSO CurrentPreset
    {
        get
        {
            if (_levelPresets == null || _levelPresets.Length == 0) return null;
            if (_levelIndex < 0 || _levelIndex >= _levelPresets.Length) return null;
            return _levelPresets[_levelIndex];
        }
    }

    private void Awake()
    {
        if (_conductor != null)
        {
            _conductor.OnMeasureTick += HandleMeasureTick;
        }
    }

    public void StartGame(int startLevelIndex = 0)
    {
        _levelIndex = startLevelIndex;
        StartLevelNow();
    }

    public void StopGame()
    {
        if (_conductor != null) _conductor.StopLevel();
    }

    // 스코어링이 사용할: 다음 판정 비트 얻기/소비
    public bool TryGetNextBeat(out RythmBeatTime beat)
    {
        beat = default;

        if (_currentBeats == null) return false;
        if (_inputBeatIndex < 0 || _inputBeatIndex >= _currentBeats.Count) return false;

        beat = _currentBeats[_inputBeatIndex];
        return true;
    }

    public void ConsumeNextBeat()
    {
        if (_currentBeats == null) return;
        if (_inputBeatIndex < _currentBeats.Count) _inputBeatIndex++;
    }

    private void StartLevelNow()
    {
        var preset = CurrentPreset;
        if (preset == null)
        {
            OnGameFinished?.Invoke();
            return;
        }

        // 레벨 상태 초기화
        _playedRythmCount = 0;

        _currentPattern = null;
        _currentBeats = null;
        _inputBeatIndex = 0;

        _isSampleTurn = true;
        _turnRemainMeasures = 0;
        _sampleScheduled = false;
        _turnStartDspTime = 0;

        // 입력 사운드 클립 초기화(레벨별)
        if (_input != null) _input.Init(preset);

        // 레벨 기준 DSP 시작(예약 안정성 위해 미래로)
        double startDsp = AudioSettings.dspTime + 0.10;

        // 콘덕터(마디 틱) 시작 + BGM 예약
        if (_conductor != null)
        {
            _conductor.StartLevel(preset.BPM, startDsp);
            _conductor.PlayBgmAt(preset.BGMClip, startDsp);
        }
    }

    private void ScheduleNextLevelAtMeasureBoundary()
    {
        _levelIndex++;

        if (_levelPresets == null || _levelIndex >= _levelPresets.Length)
        {
            OnGameFinished?.Invoke();
            return;
        }

        var nextPreset = CurrentPreset;
        if (nextPreset == null)
        {
            OnGameFinished?.Invoke();
            return;
        }

        // 다음 마디 경계에서 레벨 전환(박자 끊김 방지)
        double now = AudioSettings.dspTime;
        double nextStart = _conductor.GetNextMeasureBoundaryDsp(now);

        // 상태 초기화
        _playedRythmCount = 0;

        _currentPattern = null;
        _currentBeats = null;
        _inputBeatIndex = 0;

        _isSampleTurn = true;
        _turnRemainMeasures = 0;
        _sampleScheduled = false;
        _turnStartDspTime = 0;

        if (_input != null) _input.Init(nextPreset);

        // 콘덕터 기준/마디 길이 갱신 + BGM 교체 예약
        _conductor.StartLevel(nextPreset.BPM, nextStart);
        _conductor.PlayBgmAt(nextPreset.BGMClip, nextStart);
    }

    // ---- 마디 틱 핸들러(게임 규칙/턴 전환은 여기서만) ----
    private void HandleMeasureTick(int measureIndex, double measureStartDsp)
    {
        if (measureIndex < _startWaitMeasure) return;

        var preset = CurrentPreset;
        if (preset == null) return;

        // 1) 턴 전환(마디 시작에서만)
        if (_currentPattern != null && _turnRemainMeasures == 0)
        {
            if (_isSampleTurn)
            {
                // 샘플 턴 끝 -> 입력 턴 시작 (이 마디부터 입력)
                _isSampleTurn = false;
                _turnRemainMeasures = preset.MeasureCount;
                _inputBeatIndex = 0;

                // 입력 턴 기준 0점(이 값으로 판정 타겟 DSP 계산)
                _turnStartDspTime = measureStartDsp;
            }
            else
            {
                // 입력 턴 끝 -> 다음 패턴
                _isSampleTurn = true;
                _currentPattern = null;
                _currentBeats = null;

                _playedRythmCount++;

                // 레벨에서 요구하는 패턴 수 끝나면 다음 레벨 예약
                if (_playedRythmCount >= _totalRythmCount)
                {
                    ScheduleNextLevelAtMeasureBoundary();
                    return;
                }
            }
        }

        // 2) 패턴이 없으면 새 패턴 시작(샘플 턴)
        if (_currentPattern == null)
        {
            _currentPattern = _picker.Pick(preset);
            if (_currentPattern == null) return;

            _currentBeats = RythmTimingCalculator.ConvertToTime(_currentPattern, preset.BPM, preset.MeasureCount);
            _inputBeatIndex = 0;

            _isSampleTurn = true;
            _turnRemainMeasures = preset.MeasureCount;

            _sampleScheduled = false;

            // 샘플 턴 기준 0점(샘플 예약 기준)
            _turnStartDspTime = measureStartDsp;
        }

        // 3) 샘플은 샘플 턴에서 1회만 예약
        if (_isSampleTurn && !_sampleScheduled)
        {
            _presenter.PlaySample(_turnStartDspTime, _currentPattern, preset);
            _sampleScheduled = true;
        }

        // 4) 현재 마디 소비
        _turnRemainMeasures--;
        if (_turnRemainMeasures < 0) _turnRemainMeasures = 0;
    }
}
