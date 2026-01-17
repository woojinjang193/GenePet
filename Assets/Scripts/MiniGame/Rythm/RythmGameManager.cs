using UnityEngine;

public class RythmGameManager : MiniGameBase
{
    [Header("플레이어 목숨")]
    [SerializeField] private int _playerHeart = 3; //플레이어 목숨

    [Header("모듈")]
    [SerializeField] private RythmFlowController _flow; // 리듬 진행 담당
    [SerializeField] private RythmScoring _scoring;  // 판정/점수/UI 담당
    [SerializeField] private RythmRewardPlanner _rewardPlanner;
    [SerializeField] private RythmUiManager _uiManager;

    protected override void Start()
    {
        base.Start();

        if (_flow != null)
            _flow.OnGameFinished += HandleGameFinished; // 레벨 끝나면 종료

        if (_scoring != null)
        {
            _scoring.Init(_flow, () => Score, AddScoreDelta);
            _scoring.OnPatternResult += HandlePatternResult;
        }
        if (_rewardPlanner != null)
            _rewardPlanner.OnGiveReward += HandleGiveReward;
    }

    // 게임 시작 버튼 눌림
    public void OnGameStartClicked()
    {
        GameReset();
        base.GameStart();

        if (_scoring != null)
            _scoring.RefreshScoreUI();

        if (_flow != null)
            _flow.StartGame(0);
    }
    protected override void GameReset()
    {
        base.GameReset();
        _uiManager.SetHeart(_playerHeart);
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

        if (_scoring != null) 
            _scoring.HandlePlayerInput(); // 판정 + delta 반영 + UI 갱신
    }

    // 나가기 버튼
    public void GoBackHome()
    {
        FinishGame();
    }

    // 플레이어 사망
    protected override void GameOver()
    {
        if (_isGameOver) return;   // 중복 종료 방지

        _isGameOver = true;  // 입력 차단
        _isPlaying = false;  // AddScore 차단

        if (_flow != null)
            _flow.StopGame(); // 마디 틱/오디오 정지

        _uiManager.GameOverPanelOn();
        base.GameOver();  // 점수 기반 코인 보상 누적(GainMoneyByScore)
    }

    // 플로우(레벨 전체)가 끝났을 때
    private void HandleGameFinished()
    {
        GameOver();
    }
    private void AddScoreDelta(int delta)
    {
        AddScore(delta);   // MiniGameBase에 점수 반영(_isPlaying 체크)
    }
    
    // 패턴 성공/실패 처리(목숨/보상)
    private void HandlePatternResult(int patternIndex, bool success, bool isLastPattern)
    {
        var preset = (_flow != null) ? _flow.CurrentPreset : null;

        if (success)
        {
            // 1) 패턴 성공 보상: "고정 계획"에 있으면 지급
            if (_rewardPlanner != null)
            {
                _rewardPlanner.TryGivePatternReward(patternIndex);
            }

            // 2) 레벨 마지막 패턴 성공 시: 클리어 보상 1개 지급
            if (isLastPattern && _rewardPlanner != null)
            {
                _rewardPlanner.GiveClearReward(preset);
            }

            _uiManager.PatternSuccess(true);
        }
        else
        {
            // 패턴 실패해도 즉시 게임오버는 안 함. 대신 목숨 깎기.
            _playerHeart--;
            _uiManager.PatternSuccess(false);
            _uiManager.RemoveHeart();

            if (_playerHeart <= 0)
            {
                GameOver();
            }
        }
    }

    // RewardPlanner가 “이 보상 지급해”라고 하면 여기서 누적
    private void HandleGiveReward(RewardType type, int amount)
    {
        _uiManager.ShowItem(type, amount); //아이콘 보여줌
        GainItem(type, amount); // MiniGameBase의 보상 누적
        Debug.Log($"{type}x{amount} 지급");
        
    }
}
