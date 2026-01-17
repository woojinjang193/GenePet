using System;
using System.Collections.Generic;
using UnityEngine;

public class RythmFlowController : MonoBehaviour //마디가 바뀔 때마다 샘플턴/입력턴 전환, 패턴 시작/종료,레벨 전환을 결정
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

    // --- 레벨/게임 종료 ---
    public event Action OnGameFinished;
    public event Action<RythmLevelPresetSO> OnLevelStarted; // 레벨 시작(보상 플래닝 등)
    public event Action<int, bool, int> OnInputTurnStarted; // (patternIndex, isLastPattern, totalBeats)
    public event Action<int, bool, int> OnPatternFinished;  // (patternIndex, isLastPattern, totalBeats)

    // ===== 레벨/패턴 상태 =====
    private int _levelIndex; //레벨
    private int _playedRythmCount; //이 레벨에서 몇 번째 패턴까지 끝냈는지

    private RythmPatternSO _currentPattern; //현재 패턴
    private List<RythmBeatTime> _currentBeats;//현재 박
    private int _inputBeatIndex;// 현재 박 인덱스

    // ===== 턴 상태 =====
    private bool _isSampleTurn; //샘플 플레이 턴인지
    private int _turnRemainMeasures; //현재 턴이 몇마디 남았는지
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
            _conductor.OnMeasureTick += HandleMeasureTick;// 마디가 바뀔때 호출됨
        }
    }

    public void StartGame(int startLevelIndex = 0) //게임 시작
    {
        _levelIndex = startLevelIndex;
        StartLevelNow();
    }

    public void StopGame() //게임 중지
    {
        if (_conductor != null) _conductor.StopLevel();
    }

    // 스코어링이 사용할: 다음 판정 비트 얻기/소비
    public bool TryGetNextBeat(out RythmBeatTime beat)
    {
        beat = default;

        if (_currentBeats == null) return false;
        if (_inputBeatIndex < 0 || _inputBeatIndex >= _currentBeats.Count) return false;

        beat = _currentBeats[_inputBeatIndex]; //다음 비트
        return true;
    }

    public void ConsumeNextBeat()
    {
        if (_currentBeats == null) return;
        if (_inputBeatIndex < _currentBeats.Count) _inputBeatIndex++; //처리할 비트 남아있으면 인덱스 ++
    }
    public int GetCurrentBeatsCount() //현재 패턴 비트 수 알려줌
    {
        return _currentBeats == null ? 0 : _currentBeats.Count;
    }

    private void StartLevelNow() //레벨 스타트
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

        // 레벨 시작 이벤트(보상 플래닝 등)
        OnLevelStarted?.Invoke(preset);

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

        if (_levelPresets == null || _levelIndex >= _levelPresets.Length) //다음 레벨이 없으면
        {
            OnGameFinished?.Invoke(); //게임종료 이벤트 발생
            return;
        }

        var nextPreset = CurrentPreset;
        if (nextPreset == null) //다음 레벨 프리셋 없으면
        {
            OnGameFinished?.Invoke(); //게임종료 이벤트 발생
            return;
        }

        // 다음 마디 경계에서 레벨 전환(박자 끊김 방지)
        double now = AudioSettings.dspTime;
        double nextStart = _conductor.GetNextMeasureBoundaryDsp(now); //바로 다음 마디 시작 dsp를 구함

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

        // 레벨 시작 이벤트(보상 플래닝 등)
        OnLevelStarted?.Invoke(nextPreset);

        // 콘덕터 기준/마디 길이 갱신 + BGM 교체 예약
        _conductor.StartLevel(nextPreset.BPM, nextStart);
        _conductor.PlayBgmAt(nextPreset.BGMClip, nextStart);
    }

    // ---- 마디가 시작될 때마다 게임 상태를 한 칸 진행시키는 메인 진행 함수 ----
    private void HandleMeasureTick(int measureIndex, double measureStartDsp) //마디 시작마다 conductor 에서 호출
    {
        if (measureIndex < _startWaitMeasure) return; //대기 마디동안 리턴

        var preset = CurrentPreset;
        if (preset == null) return;

        // 현재 진행 중인 "패턴 인덱스"(0-based): 완료한 수 = 현재 진행 번호
        int patternIndex = _playedRythmCount;
        bool isLastPattern = (patternIndex == _totalRythmCount - 1);

        // 턴 전환(마디 시작에서만)
        if (_currentPattern != null && _turnRemainMeasures == 0) //패턴이 있고, 끝났을때만
        {
            if (_isSampleTurn) //방금전 마디에 샘플재생중이었을 때
            {
                // 샘플 턴 끝 > 입력 턴 시작 (이 마디부터 입력)
                _isSampleTurn = false;
                _turnRemainMeasures = preset.MeasureCount;
                _inputBeatIndex = 0;

                // 입력 턴 기준 0점(이 값으로 판정 타겟 DSP 계산)
                _turnStartDspTime = measureStartDsp;

                // 입력 턴 시작 이벤트(스코어링이 패턴 통계 초기화하는 타이밍)
                OnInputTurnStarted?.Invoke(patternIndex, isLastPattern, GetCurrentBeatsCount());
            }
            else 
            {
                // 입력 턴 끝 -> 패턴 종료(스코어링이 성공/실패 판단하는 타이밍)
                int totalBeats = GetCurrentBeatsCount();
                OnPatternFinished?.Invoke(patternIndex, isLastPattern, totalBeats);

                // 다음 패턴 준비
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
            _currentPattern = _picker.Pick(preset); //레벨의 랜덤 패턴 뽑기
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
        if (_isSampleTurn && !_sampleScheduled) //샘플턴이고 샘플 예약 전일때
        {
            _presenter.PlaySample(_turnStartDspTime, _currentPattern, preset);
            _sampleScheduled = true;
        }

        // 4) 현재 마디 소비
        _turnRemainMeasures--;
        if (_turnRemainMeasures < 0) _turnRemainMeasures = 0;
    }
}
