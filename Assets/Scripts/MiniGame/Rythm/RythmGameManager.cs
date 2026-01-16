// =========================
// RythmGameManager.cs
// - MiniGameBase 라이프사이클(시작/리셋/오버/보상) 담당
// - 점수 "값"은 MiniGameBase(_score)가 주인
// - RythmScoring이 delta를 콜백으로 보내면 AddScore(delta)로 반영
// =========================

using UnityEngine;

public class RythmGameManager : MiniGameBase
{
    [Header("모듈")]
    [SerializeField] private RythmFlowController _flow;     // 레벨/턴/패턴 상태머신
    [SerializeField] private RythmScoring _scoring;         // 판정/오토미스/점수 UI

    protected override void Start()
    {
        base.Start();                                       // 보상 딕셔너리 등 초기화

        if (_flow != null)
            _flow.OnGameFinished += HandleGameFinished;     // 레벨 끝나면 종료

        if (_scoring != null)
            _scoring.Init(_flow, () => Score, AddScoreDelta); // 점수는 Base, UI는 Scoring
    }

    // 게임 시작 버튼
    public void OnGameStartClicked()
    {
        GameReset();                                        // 점수/시간 초기화
        GameStart();                                        // _isPlaying = true

        if (_scoring != null)
            _scoring.RefreshScoreUI();                      // UI 갱신

        if (_flow != null)
            _flow.StartGame(0);                             // 레벨 0 시작
    }

    // 매 프레임: 오토미스는 매 프레임 처리해야 함
    private void Update()
    {
        // MiniGameBase.Update()는 private라 자동 호출됨(플레이 시간 누적)

        if (!_isPlaying || _isGameOver) return;             // 게임 중 아니면 종료

        if (_scoring != null)
            _scoring.ProcessAutoMisses();                   // 자동 Miss 처리
    }

    // 입력 버튼
    public void OnPlayerInput()
    {
        if (!_isPlaying || _isGameOver) return;             // 종료 상태면 무시
        if (_scoring != null) _scoring.HandlePlayerInput(); // 판정 + delta 반영 + UI 갱신
    }

    // 나가기 버튼
    public void GoBackHome()
    {
        EndAndCloseGame();                                  // 종료 처리
    }

    // 플레이어 사망
    public void OnPlayerDead()
    {
        EndAndCloseGame();                                  // 종료 처리
    }

    // 플로우(레벨 전체)가 끝났을 때
    private void HandleGameFinished()
    {
        EndAndCloseGame();                                  // 종료 처리
    }

    // 점수 delta 반영(Scoring 콜백)
    private void AddScoreDelta(int delta)
    {
        AddScore(delta);                                    // MiniGameBase 점수 반영(_isPlaying 체크)
        // 점수 UI는 RythmScoring에서 갱신하므로 여기서는 아무것도 안 함
    }

    // 공통 종료 루틴
    private void EndAndCloseGame()
    {
        if (_isGameOver) return;                             // 중복 종료 방지

        _isGameOver = true;                                  // 입력 차단
        _isPlaying = false;                                  // AddScore 차단

        if (_flow != null)
            _flow.StopGame();                                // 마디 틱/오디오 정지

        GameOver();                                          // 점수 기반 코인 보상 누적(GainMoneyByScore)
        FinishGame();                                        // 누적 보상 전달
    }
}
