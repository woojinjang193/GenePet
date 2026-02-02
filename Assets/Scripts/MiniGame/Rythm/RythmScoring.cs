using System;
using TMPro;
using UnityEngine;

public class RythmScoring : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmJudge _judge;          // 판정기
    [SerializeField] private RythmInput _input;          // 입력 사운드
    
    [Header("점수(Delta)")]
    [SerializeField] private int _scorePerfect = 2;      // Perfect일 때 더할 점수
    [SerializeField] private int _scoreGood = 1;         // Good일 때 더할 점수
    [SerializeField] private int _scoreMiss = -1;        // Miss일 때 더할 점수

    [Header("패턴 성공 기준(미스 비율)")]
    [Range(0f, 1f)]
    [SerializeField] private float _maxMissRatioToSucceed = 0.5f; // 예: 0.5면 "미스가 절반 이하"면 성공

    [Header("마디 시작 직전 입력 허용 구간")]
    [SerializeField] private float _earlyInputBufferSec = 0.10f;
    private double _bufferedInputDsp = -1; //(샘플턴에 눌린 입력 시간 저장)

    [Header("UI")]
    [SerializeField] private TMP_Text _scoreText;        // 점수 UI

    // 외부 주입
    private RythmFlowController _flow;                   // 턴/패턴 진행 정보 제공자
    private Func<int> _getScore;                         // 현재 점수 읽기
    private Action<int> _addScoreDelta;                  // 점수 delta 반영 요청

    // 패턴 통계(입력 턴 기준)
    private int _patternMissCount;

    // 패턴 결과 이벤트(Manager/RewardPlanner가 구독)
    public event Action<int, bool, bool> OnPatternResult;
    // (patternIndex, success, isLastPattern)
    public event Action<JudgeResult> OnJudged; //판정 결과

    private bool _patternResultSent; //한 패턴당 결과 1회만
    private int _currentTotalBeats;  //이번 패턴의 총 비트 수 저장

    // 초기화: GameManager가 한 번만 호출
    public void Init(RythmFlowController flow, Func<int> getScore, Action<int> addScoreDelta)
    {
        _flow = flow;                                    // 플로우 연결
        _getScore = getScore;                            // 점수 getter 연결
        _addScoreDelta = addScoreDelta;                  // 점수 delta 반영 콜백 연결

        if (_flow != null)
        {
            _flow.OnInputTurnStarted += HandleInputTurnStarted;
            _flow.OnPatternFinished += HandlePatternFinished;
        }

        RefreshScoreUI();                                // UI 초기 갱신
    }

    // 외부에서 강제로 UI만 새로고침하고 싶을 때 사용
    public void RefreshScoreUI()
    {
        if (_scoreText == null) return;                  // 텍스트 없으면 종료
        if (_getScore == null) return;                   // getter 없으면 종료
        _scoreText.text = $"Score: {_getScore()}";       // 현재 점수 표시
    }

    public void HandlePlayerInput()  // 입력 처리(판정 + 점수 + 사운드)
    {
        if (_flow == null) return;   // 플로우 없으면 종료

        double now = AudioSettings.dspTime;  // 현재 입력 시점 DSP

        if (_flow.IsSampleTurn)
        {
            _bufferedInputDsp = now;
            return;
        }

        var preset = _flow.CurrentPreset; // 현재 레벨 프리셋
        if (preset == null) return;    // 없으면 종료

        // 1) 마지막 박이면 무조건 last clip이 나와야 함(시간기준 판별)
        bool isLastMeasureLastBeat = IsLastMeasureLastBeat(now, preset);
        if (_input != null) _input.PlayInputSound(isLastMeasureLastBeat);

        // 2) 판정(항상)
        JudgeResult result = JudgeResult.Miss;

        //남은 비트가 없으면 "즉시 패턴 결과"만 쏘고 끝
        if (!_flow.TryGetNextBeat(out var beat))
        {
            TrySendEarlyPatternResult();
            RefreshScoreUI();
            return;
        }

        // 다음 목표 비트가 있다면 그 비트로 판정
        if (_judge != null)
        {
            double beatDuration = 60.0 / preset.BPM;

            //절대 DSP(float) 대신 "턴 시작 기준 상대시간"으로 판정
            double inputRel = now - _flow.TurnStartDspTime; //입력 상대시간(초)
            double targetRel = beat.Time;     // 정답 상대시간(초)
            double beatDur = beatDuration;    // 1박 길이(초)

            result = _judge.Judge((float)inputRel, (float)targetRel, (float)beatDur); // Judge는 동일 시그니처 유지(내부는 상대시간)
            _flow.ConsumeNextBeat();
        }

        // 판정 결과를 점수 delta로 변환해서 외부에 반영 요청
        ApplyJudgeResult(result);

        TrySendEarlyPatternResult(); //마지막 비트를 소비해서 "남은 비트 0"이면 즉시 패턴 결과 전송

        // UI 갱신
        RefreshScoreUI();
    }

    public void ProcessAutoMisses()    // 오토미스(놓친 노트 자동 미스 처리)
    {
        if (_flow == null) return;  // 플로우 없으면 종료
        if (_flow.IsSampleTurn) return;  // 샘플 턴이면 종료

        var preset = _flow.CurrentPreset;  // 현재 레벨 프리셋
        if (preset == null) return;  // 없으면 종료
        if (_judge == null) return;   // 판정기 없으면 종료

        double now = AudioSettings.dspTime;  // 현재 DSP
        double beatDuration = 60.0 / preset.BPM; // 1박(초)

        // Good 윈도우 끝까지 기다렸다가 지나면 Miss 처리
        double lateLimit = _judge.goodBeatWindow * beatDuration; // 정답+lateLimit 지나면 Miss

        // 한 프레임에 여러 비트를 놓쳤을 수도 있으니 while
        while (_flow.TryGetNextBeat(out var beat))
        {
            double target = _flow.TurnStartDspTime + beat.Time;  // 다음 정답 시간

            // 아직 늦지 않으면 종료
            if (now <= target + lateLimit) break;

            // 늦었으면 Miss 처리 + 비트 소비
            ApplyJudgeResult(JudgeResult.Miss);
            _flow.ConsumeNextBeat();
        }
        TrySendEarlyPatternResult();
        RefreshScoreUI();
    }
    // =========================Flow 이벤트 핸들러=========================
    private void HandleInputTurnStarted(int patternIndex, bool isLastPattern, int totalBeats)
    {
        // 입력 턴 시작 시점에 패턴 통계 초기화
        _patternMissCount = 0;
        _patternResultSent = false;
        _currentTotalBeats = totalBeats;

        if (_bufferedInputDsp > 0)
        {
            // 버퍼 입력이 "입력턴 시작 전"에 눌린 것만 허용
            double diff = _flow.TurnStartDspTime - _bufferedInputDsp;

            if (diff >= 0 && diff <= _earlyInputBufferSec)
            {
                HandleBufferedInput(_bufferedInputDsp);
            }

            _bufferedInputDsp = -1;
        }
    }
    private void HandleBufferedInput(double bufferedNow) //버퍼 입력 1회 판정 함수
    {
        var preset = _flow.CurrentPreset;
        if (preset == null) return;

        //버퍼 입력도 입력 사운드 재생
        bool isLastMeasureLastBeat = IsLastMeasureLastBeat(bufferedNow, preset);
        if (_input != null) _input.PlayInputSound(isLastMeasureLastBeat);

        JudgeResult result = JudgeResult.Miss;

        //버퍼 입력도 남은 비트 없으면 즉시 정산만
        if (!_flow.TryGetNextBeat(out var beat))
        {
            TrySendEarlyPatternResult();
            RefreshScoreUI();
            return;
        }

        if (_judge != null)
        {
            double beatDuration = 60.0 / preset.BPM;

            // [수정] 상대시간 판정
            double inputRel = bufferedNow - _flow.TurnStartDspTime; // 입력 상대시간(초)
            double targetRel = beat.Time;       // 정답 상대시간(초)
            double beatDur = beatDuration;    // 1박 길이(초)

            result = _judge.Judge((float)inputRel, (float)targetRel, (float)beatDur);
            _flow.ConsumeNextBeat();
        }

        ApplyJudgeResult(result);
        //버퍼 입력이 마지막 비트였으면 즉시 정산
        TrySendEarlyPatternResult();
        RefreshScoreUI();
    }

    private void HandlePatternFinished(int patternIndex, bool isLastPattern, int totalBeats)
    {
        if (_patternResultSent) return; //조기 정산했으면 중복 방지
        SendPatternResult(patternIndex, isLastPattern, totalBeats);
    }
    // =========================조기 정산 로직=========================
    private void TrySendEarlyPatternResult()
    {
        if (_patternResultSent) return;
        if (_flow == null) return;
        if (_flow.IsSampleTurn) return;

        // 다음 비트가 있으면 아직 정산하면 안됨
        if (_flow.TryGetNextBeat(out _)) return;

        int patternIndex = _flow.PlayedRythmCount;
        bool isLastPattern = (patternIndex == _flow.TotalRythmCount - 1);

        SendPatternResult(patternIndex, isLastPattern, _currentTotalBeats);
    }

    private void SendPatternResult(int patternIndex, bool isLastPattern, int totalBeats)
    {
        _patternResultSent = true;

        int beats = Mathf.Max(0, totalBeats);

        bool success;
        if (beats <= 0)
        {
            success = true;
        }
        else
        {
            int maxMissAllowed = Mathf.FloorToInt(beats * _maxMissRatioToSucceed);
            success = (_patternMissCount <= maxMissAllowed);
        }

        OnPatternResult?.Invoke(patternIndex, success, isLastPattern);
    }

    // 판정 결과를 점수 delta로 바꿔서 외부에 반영 요청 + UI 갱신
    private void ApplyJudgeResult(JudgeResult result)
    {
        if (_addScoreDelta == null) return;             // 콜백 없으면 종료
        if (_getScore == null) return;                  // getter 없으면 종료

        int delta = 0;                                  // 점수 변화량

        switch (result)
        {
            case JudgeResult.Perfect: delta = _scorePerfect; break; // Perfect
            case JudgeResult.Good: delta = _scoreGood; break; // Good
            case JudgeResult.Miss: delta = _scoreMiss; _patternMissCount++; break; // Miss
        }

        OnJudged?.Invoke(result);
        _addScoreDelta(delta);                          // 실제 점수 반영은 외부가 함
    }

    // "마지막 박 구간" 판별(패턴 데이터에 마지막 박이 Rest여도 시간으로 판단)
    private bool IsLastMeasureLastBeat(double nowDsp, RythmLevelPresetSO preset)
    {
        // 입력 턴 시작 기준으로 현재 몇 박째인지 계산
        double beatDuration = 60.0 / preset.BPM;
        double beatPos = (nowDsp - _flow.TurnStartDspTime) / beatDuration;
        if (beatPos < 0) beatPos = 0;

        int curMeasure = (int)(beatPos / 4.0);          // 현재 마디
        int curBeat = (int)(beatPos % 4.0);             // 현재 박(0,1,2,3)

        bool isLastPattern = (_flow.PlayedRythmCount == _flow.TotalRythmCount - 1); // 마지막 패턴인지
        bool isLastMeasure = (curMeasure == preset.MeasureCount - 1);              // 마지막 마디인지
        bool isBeat4 = (curBeat == 3);                                           // 4번째 박인지

        return isLastPattern && isLastMeasure && isBeat4;                         // 모두 만족하면 true
    }
}
