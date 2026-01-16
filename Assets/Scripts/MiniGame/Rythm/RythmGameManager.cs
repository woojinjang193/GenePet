
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RythmGameManager : MiniGameBase
{
    [Header("참조")]
    [SerializeField] private RythmJudge _Judge; //판정 담당
    [SerializeField] private RythmPicker _picker; //리듬 패턴 선택 담당
    [SerializeField] private RythmPresenter _presenter; //리즘 샘플 재생
    [SerializeField] private RythmInput _input; //플레이어 입력 사운드 재생

    [Header("BGM")]
    [SerializeField] private AudioSource _bgmSource;

    [Header("레벨 설정")]
    [SerializeField] private RythmLevelPresetSO[] _levelPresets; //레벨 프리셋 목록
    [SerializeField] private int _totalRythmCount = 5; // 이 레벨에서 플레이할 리듬 개수

    [Header("시작 대기 마디 수")]
    [SerializeField] private int _startWaitMeasure = 2; // 레벨 시작 전 대기 마디 수

    [Header("점수")]
    [SerializeField] private int _scorePerfect = 2;
    [SerializeField] private int _scoreGood = 1;
    [SerializeField] private int _scoreMiss = -1;
    [SerializeField] private TMP_Text _curScoreText;

    // ===== 레벨 상태 =====
    private int _levelIndex; //현재 레벨
    private int _playedRythmCount; //현재 레벨 재생한 리듬 개수

    // ===== 패턴 상태 =====
    private RythmPatternSO _currentPattern; //현재 재생중인 리듬 패턴
    private List<RythmBeatTime> _currentBeats; //현재 박자 리스트
    private int _inputBeatIndex; //입력 인덱스

    // ===== 턴 상태 =====
    private bool _isSampleTurn; //샘플 턴 여부
    private int _turnRemainMeasures;  // 현재 턴(샘플/입력)이 몇 마디 남았는지
    private bool _sampleScheduled;  // 샘플을 이번 패턴에서 이미 예약했는지

    //=======입력=======
    private double _turnStartDspTime; // 현재 턴(샘플/입력) 시작 기준 DSP

    // ===== dsp 기준 =====
    private double _levelStartDspTime; // 레벨 기준 dsp 시작(=BGM 시작 기준)
    private double _measureDuration; // 1마디 길이(초)
    private int _lastProcessedMeasure; // 마지막으로 처리한 마디 인덱스

    // ===== 미니게임별 능력 계수 =====
    private float _coinMul = 1f;  //코인 아이템 획득 배율

    //==========초기화================
    protected override void Start()
    {
        base.Start();
        if (_bgmSource != null) _bgmSource.playOnAwake = false;
    }
    //=============게임 사이클===============
    public void OnGameStartClicked() //게임 시작 버튼 눌림
    {
        GameReset();
        GameStart();

        _levelIndex = 0;
        StartLevelAndSyncBGM();
    }
    protected override void GameReset()
    {
        ApplyAbilities();
        base.GameReset();
        _curScoreText.text = $"Score: {_score}";
    }
    public void OnPlayerDead()
    {
        _isGameOver = true;

        Debug.Log("게임오버");
    }

    public void GoBackHome()
    {
        FinishGame();
    }
    // ================== 레벨 ================== // 구분 주석
    private void StartLevelAndSyncBGM() // 레벨 시작 기준을 BGM 시작과 동일하게 맞춤
    {
        if (_levelPresets == null || _levelPresets.Length == 0) { FinishGame(); return; } // 프리셋 없으면 종료
        if (_levelIndex < 0 || _levelIndex >= _levelPresets.Length) { FinishGame(); return; } // 인덱스 안전장치
        if (_bgmSource == null) { Debug.LogWarning("BGM AudioSource 없음"); } // 없으면 경고만

        _playedRythmCount = 0; // 카운트 초기화
        _currentPattern = null; // 패턴 초기화
        _currentBeats = null; // 비트 초기화
        _inputBeatIndex = 0; // 입력 인덱스 초기화
        _isSampleTurn = true; // 샘플부터 시작
        _turnRemainMeasures = 0;
        _sampleScheduled = false;
        _turnStartDspTime = 0;


        float bpm = _levelPresets[_levelIndex].BPM; // 현재 레벨 BPM
        _measureDuration = 4.0 * (60.0 / bpm); // 4/4 기준 1마디 시간

        _input.Init(_levelPresets[_levelIndex]); // 입력 사운드 클립 세팅

        double now = AudioSettings.dspTime; // 현재 dsp 시간
        _levelStartDspTime = now + 0.10; // 약간 미래로 잡아(예약 안정성) 기준 고정
        _lastProcessedMeasure = -1; // 업데이트용 리셋

        PlayLevelBGMAt(_levelStartDspTime); // BGM도 같은 기준 시간에 시작
    }

    private void EndLevelAndScheduleNext() // 레벨 종료 > 다음 레벨을 "마디 경계"에서 시작
    {
        _levelIndex++; // 다음 레벨
        if (_levelIndex >= _levelPresets.Length) { FinishGame(); return; } // 마지막이면 종료

        float nextBpm = _levelPresets[_levelIndex].BPM; // 다음 레벨 BPM
        double nextMeasureDuration = 4.0 * (60.0 / nextBpm); // 다음 레벨의 마디 길이

        double now = AudioSettings.dspTime; // 현재 dsp 시간
        double elapsed = now - _levelStartDspTime; // 현재 레벨 기준으로 지난 시간
        if (elapsed < 0) elapsed = 0; // 안전장치

        int curMeasure = (int)(elapsed / _measureDuration); // 현재 마디 인덱스
        double curMeasureStart = _levelStartDspTime + curMeasure * _measureDuration; // 현재 마디 시작 dsp
        double nextMeasureStart = curMeasureStart + _measureDuration; // 다음 마디 시작 dsp(경계)

        _playedRythmCount = 0; // 카운트 초기화
        _currentPattern = null; // 패턴 초기화
        _currentBeats = null; // 비트 초기화
        _inputBeatIndex = 0; // 입력 인덱스 초기화
        _isSampleTurn = true; // 샘플부터

        _turnRemainMeasures = 0;
        _sampleScheduled = false;
        _turnStartDspTime = 0;


        _measureDuration = nextMeasureDuration; // 다음 레벨 마디 길이로 교체
        _levelStartDspTime = nextMeasureStart; // 기준도 마디 경계로 이동(=BGM 시작 기준)
        _lastProcessedMeasure = -1; // 업데이트용 리셋

        _input.Init(_levelPresets[_levelIndex]); // 입력 사운드 클립 교체
        PlayLevelBGMAt(_levelStartDspTime); // 다음 레벨 BGM도 같은 기준에 예약
    }

    private void PlayLevelBGMAt(double dspStart) // 특정 dsp 시간에 BGM 시작
    {
        if (_bgmSource == null) return; // 소스 없으면 종료
        AudioClip clip = _levelPresets[_levelIndex].BGMClip; // 레벨 BGM
        if (clip == null) return; // 클립 없으면 종료

        _bgmSource.Stop(); // 기존 재생 중지
        _bgmSource.clip = clip; // 클립 교체
        _bgmSource.loop = true; // 루프
        _bgmSource.PlayScheduled(dspStart); // 예약 재생(정확한 시작)
    }

    private void Update() // 메인 루프
    {
        if (!_isPlaying || _isGameOver) return; // 게임 중 아니면 무시

        double now = AudioSettings.dspTime; // 현재 dsp
        double levelTime = now - _levelStartDspTime; // 레벨 시작 기준으로 경과 시간
        if (levelTime < 0) return; // 아직 시작 전이면 무시(예약 대기)

        ProcessAutoMisses(); // 입력 안 한 비트 자동 Miss 처리

        int measureIndex = (int)(levelTime / _measureDuration); // 현재 마디 인덱스
        if (measureIndex == _lastProcessedMeasure) return; // 같은 마디면 중복 처리 방지

        _lastProcessedMeasure = measureIndex; // 마디 갱신
        OnMeasureTick(measureIndex); // 마디 이벤트 처리
    }
    private void OnMeasureTick(int measureIndex)
    {
        if (measureIndex < _startWaitMeasure) return;

        RythmLevelPresetSO preset = _levelPresets[_levelIndex];
        if (preset == null) return;

        double measureStartDsp = _levelStartDspTime + (measureIndex * _measureDuration);

        // 1) 턴 전환은 "마디 시작 시점"에만 처리
        if (_currentPattern != null && _turnRemainMeasures == 0)
        {
            if (_isSampleTurn)
            {
                // 샘플 턴 종료 -> 입력 턴 시작(이 마디부터 입력 가능)
                _isSampleTurn = false;
                _turnRemainMeasures = preset.MeasureCount;
                _inputBeatIndex = 0;
                _turnStartDspTime = measureStartDsp; // ★입력 판정 기준점
            }
            else
            {
                // 입력 턴 종료 -> 다음 패턴
                _isSampleTurn = true;
                _currentPattern = null;
                _currentBeats = null;
                _playedRythmCount++;

                if (_playedRythmCount >= _totalRythmCount)
                {
                    EndLevelAndScheduleNext();
                    return;
                }
            }
        }

        // 2) 패턴이 없으면 새 패턴 시작(항상 샘플 턴부터)
        if (_currentPattern == null)
        {
            _currentPattern = _picker.Pick(preset);
            if (_currentPattern == null) return;

            _currentBeats = RythmTimingCalculator.ConvertToTime(_currentPattern, preset.BPM, preset.MeasureCount);
            _inputBeatIndex = 0;

            _isSampleTurn = true;
            _turnRemainMeasures = preset.MeasureCount;

            _sampleScheduled = false;
            _turnStartDspTime = measureStartDsp; // ★샘플 턴 시작 기준점(샘플 예약 기준)
        }

        // 3) 샘플 턴이면 샘플은 한 번만 예약(턴 시작 DSP 기준)
        if (_isSampleTurn && !_sampleScheduled)
        {
            _presenter.PlaySample(_turnStartDspTime, _currentPattern, preset);
            _sampleScheduled = true;
        }

        // 4) 현재 마디 소비
        _turnRemainMeasures--;
        if (_turnRemainMeasures < 0) _turnRemainMeasures = 0;
    }
    public void OnPlayerInput()
    {
        if (_isGameOver) return;
        if (_isSampleTurn) return;

        var preset = _levelPresets[_levelIndex];
        double now = AudioSettings.dspTime;

        // --- 마지막 박(시간기준) 판별: 패턴에 마지막 박 정보가 없어도 동작 ---
        double beatDuration = 60.0 / preset.BPM;                  // 1박(초)
        double beatPos = (now - _turnStartDspTime) / beatDuration; // 입력턴 시작 기준 몇 박째인지
        if (beatPos < 0) beatPos = 0;

        int curMeasure = (int)(beatPos / 4.0); // 4/4 기준 현재 마디
        int curBeat = (int)(beatPos % 4.0);    // 0,1,2,3

        bool isLastMeasureLastBeat =
            (_playedRythmCount == _totalRythmCount - 1) &&
            (curMeasure == preset.MeasureCount - 1) &&
            (curBeat == 3);

        // 입력 소리(요구사항: 마지막 박이면 무조건 LastClip)
        _input.PlayInputSound(isLastMeasureLastBeat);

        // --- 항상 판정 ---
        JudgeResult result = JudgeResult.Miss;

        if (_Judge != null && _currentBeats != null && _inputBeatIndex < _currentBeats.Count)
        {
            // 이번에 판정할 목표 비트(정답 타이밍)
            RythmBeatTime beat = _currentBeats[_inputBeatIndex];

            float inputDsp = (float)now;                              // 실제 입력 시간
            float targetDsp = (float)(_turnStartDspTime + beat.Time);  // 정답 시간(절대 DSP)
            float beatDurF = (float)beatDuration;                      // 1박(초)

            result = _Judge.Judge(inputDsp, targetDsp, beatDurF);
        }
        ApplyJudgeResult(result);
        // 비트가 없거나 다 끝났으면 result는 Miss 유지

        // 지금은 단순 진행(나중에 규칙 바꿔도 됨)
        _inputBeatIndex++;

        // 여기서 result로 점수/연출/콤보 처리하면 됨
        // 예: Perfect면 +, Good면 +, Miss면 -
    }
    private void ApplyJudgeResult(JudgeResult result) // 판정에 따른 점수 반영
    {
        switch (result)
        {
            case JudgeResult.Perfect: _score += _scorePerfect; break;
            case JudgeResult.Good: _score += _scoreGood; break;
            case JudgeResult.Miss: _score += _scoreMiss; break;
        }

        if (_curScoreText != null) _curScoreText.text = $"Score: {_score}";
    }
    private void ProcessAutoMisses() // 입력 안 하고 지나친 비트 자동 Miss
    {
        if (_isGameOver) return; // 게임오버면 종료
        if (_isSampleTurn) return; // 샘플 턴이면 종료
        if (_Judge == null) return; // 판정기 없으면 종료
        if (_currentBeats == null) return; // 비트 목록 없으면 종료

        var preset = _levelPresets[_levelIndex]; // 현재 프리셋
        double now = AudioSettings.dspTime; // 현재 dsp
        double beatDuration = 60.0 / preset.BPM; // 1박(초)

        // "Good 윈도우"가 끝난 시점을 지나면 자동 Miss
        double lateLimit = _Judge.goodBeatWindow * beatDuration; // 늦어도 되는 최대 시간(초)

        // 한 프레임에 여러 비트를 놓쳤을 수도 있으니 while
        while (_inputBeatIndex < _currentBeats.Count)
        {
            double target = _turnStartDspTime + _currentBeats[_inputBeatIndex].Time; // 다음 정답 시간

            if (now <= target + lateLimit) break; // 아직 Miss 처리할 만큼 늦지 않음

            // 지나쳤으니 Miss 처리
            ApplyJudgeResult(JudgeResult.Miss);
            _inputBeatIndex++; // 다음 비트로 이동
        }
    }


    //===========특수능력 ====================
    public void ApplyAbilities()
    {
        if (_effectContext == null) { Debug.LogWarning("_effectContext 없음"); return; }

        _coinMul = _effectContext.GoldMultiplier; //코인 배율
    }
}
