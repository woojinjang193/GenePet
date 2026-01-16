using System;
using TMPro;
using UnityEngine;

public class RythmScoring : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmJudge _judge;          // 판정기
    [SerializeField] private RythmInput _input;          // 입력 사운드
    [SerializeField] private TMP_Text _scoreText;        // 점수 UI

    [Header("점수(Delta)")]
    [SerializeField] private int _scorePerfect = 2;      // Perfect일 때 더할 점수
    [SerializeField] private int _scoreGood = 1;         // Good일 때 더할 점수
    [SerializeField] private int _scoreMiss = -1;        // Miss일 때 더할 점수

    private RythmFlowController _flow;                   // 턴/패턴 진행 정보 제공자

    // "점수 값"은 외부(MiniGameBase)가 주인이므로,
    // 현재 점수 조회와 점수 반영을 콜백으로 받는다.
    private Func<int> _getScore;                         // 현재 점수 읽기
    private Action<int> _addScoreDelta;                  // 점수 delta 반영 요청

    // 초기화: GameManager가 한 번만 호출
    public void Init(RythmFlowController flow, Func<int> getScore, Action<int> addScoreDelta)
    {
        _flow = flow;                                    // 플로우 연결
        _getScore = getScore;                            // 점수 getter 연결
        _addScoreDelta = addScoreDelta;                  // 점수 delta 반영 콜백 연결
        RefreshScoreUI();                                // UI 초기 갱신
    }

    // 외부에서 강제로 UI만 새로고침하고 싶을 때 사용
    public void RefreshScoreUI()
    {
        if (_scoreText == null) return;                  // 텍스트 없으면 종료
        if (_getScore == null) return;                   // getter 없으면 종료
        _scoreText.text = $"Score: {_getScore()}";       // 현재 점수 표시
    }

    // =========================
    // 입력 처리: 버튼/터치에서 호출
    // - 항상 판정
    // - 마지막 박 구간이면 무조건 LastClip
    // - 점수는 delta로 외부에 반영 요청
    // =========================
    public void HandlePlayerInput()
    {
        if (_flow == null) return;                       // 플로우 없으면 종료
        if (_flow.IsSampleTurn) return;                  // 샘플 턴이면 입력 무시

        var preset = _flow.CurrentPreset;                // 현재 레벨 프리셋
        if (preset == null) return;                      // 없으면 종료

        double now = AudioSettings.dspTime;              // 현재 입력 시점 DSP
        double beatDuration = 60.0 / preset.BPM;         // 1박(초)

        // 마지막 패턴의 마지막 마디 4번째 박(시간상)인지 체크
        bool isLastBeatWindow = IsLastMeasureLastBeatWindow(now, beatDuration, preset);

        // 입력 사운드 재생(요구사항: 마지막 박 구간이면 무조건 LastClip)
        if (_input != null)
            _input.PlayInputSound(isLastBeatWindow);

        // 항상 판정: 기본은 Miss
        JudgeResult result = JudgeResult.Miss;

        // 다음 목표 비트가 있다면 그 비트로 판정
        if (_judge != null && _flow.TryGetNextBeat(out var beat))
        {
            float inputDsp = (float)now;                                   // 실제 입력 시간
            float targetDsp = (float)(_flow.TurnStartDspTime + beat.Time); // 정답 시간(절대 DSP)
            float beatDurF = (float)beatDuration;                           // 1박 길이(초)

            result = _judge.Judge(inputDsp, targetDsp, beatDurF);          // Perfect/Good/Miss

            _flow.ConsumeNextBeat();                                       // 입력 1번 = 비트 1개 소비(현재 규칙)
        }

        // 판정 결과를 점수 delta로 변환해서 외부에 반영 요청
        ApplyJudgeResult(result);
    }

    // =========================
    // 오토 미스 처리: 매 프레임 호출 권장
    // - "Good 윈도우"가 지나면 그 비트는 Miss로 확정
    // =========================
    public void ProcessAutoMisses()
    {
        if (_flow == null) return;                      // 플로우 없으면 종료
        if (_flow.IsSampleTurn) return;                 // 샘플 턴이면 종료

        var preset = _flow.CurrentPreset;               // 현재 레벨 프리셋
        if (preset == null) return;                     // 없으면 종료
        if (_judge == null) return;                     // 판정기 없으면 종료

        double now = AudioSettings.dspTime;             // 현재 DSP
        double beatDuration = 60.0 / preset.BPM;        // 1박(초)

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
            case JudgeResult.Miss: delta = _scoreMiss; break; // Miss
        }

        _addScoreDelta(delta);                          // 실제 점수 반영은 외부가 함
        RefreshScoreUI();                               // UI는 여기서 갱신
    }

    // "마지막 박 구간" 판별(패턴 데이터에 마지막 박이 Rest여도 시간으로 판단)
    private bool IsLastMeasureLastBeatWindow(double nowDsp, double beatDuration, RythmLevelPresetSO preset)
    {
        // 입력 턴 시작 기준으로 몇 박째인지
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
