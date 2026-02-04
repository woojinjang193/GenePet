using UnityEngine;

public class RythmGameManager : MiniGameBase
{
    [Header("플레이어")]
    [SerializeField] private MiniGamePetVisualLoader _playerVisual;

    [Header("플레이어 목숨")]
    [SerializeField] private int _playerMaxHeart = 3; //플레이어 목숨

    [Header("모듈")]
    [SerializeField] private RythmFlowController _flow; // 리듬 진행 담당
    [SerializeField] private RythmScoring _scoring;  // 판정/점수/UI 담당
    [SerializeField] private RythmRewardPlanner _rewardPlanner;
    [SerializeField] private RythmUiManager _uiManager;
    [SerializeField] private RythmVisualController _rythmVisual;

    [Header("랜덤 알 보상")]
    [SerializeField] private RewardEggPresetSO[] _rewardEggs;

    //마지막 레벨 클리어 보상 1회 제한 플래그
    private bool _lastLevelClearRewardGiven = false; //마지막 레벨에서 클리어 보상은 1회만 주기

    private int _playerCurHeart;

    //===성격 능력=====
    private float _coinMul = 1f;  //코인 아이템 획득 배율
    private int _heartExtra = 0;  //목숨 추가 수

    protected override void Start()
    {
        base.Start();
        //여기에 성격 능력 넣기
        _playerCurHeart = _playerMaxHeart;
        _playerVisual.LoadPetVisual(_pet, MiniGame.Rythm); //펫 비주얼 로드

        if (_flow != null)
            _flow.OnGameFinished += HandleGameFinished; // 레벨 끝나면 종료

        if (_scoring != null)
        {
            _scoring.Init(_flow, () => Score, AddScoreDelta);
            _scoring.OnPatternResult += HandlePatternResult;
        }
        if (_rewardPlanner != null)
        {
            _rewardPlanner.OnGiveReward += HandleGiveReward;
            _rewardPlanner.InjectManager(this);
        }
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
        ApplyAbilities(); //특수능력 초기화
        _playerCurHeart = _playerMaxHeart + _heartExtra;
        _uiManager.SetHeart(_playerCurHeart); //체력 켜주기

        _lastLevelClearRewardGiven = false; // 새 게임 시작이면 마지막 레벨 클리어 보상 다시 1회 허용
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

        int bestScore = Manager.Mini.GetBestScore(MiniGame.Rythm);
        _uiManager.GameOverPanelOn(_score , bestScore);
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
            if (isLastPattern)
            {
                bool isLastLevel = (_flow != null) && _flow.IsLastLevel; //Flow의 마지막 레벨 여부 사용

                if (!isLastLevel) //마지막 레벨이 아니면 기존대로 클리어 보상
                {
                    _rewardPlanner?.GiveClearReward(preset);
                }
                else //마지막 레벨이면 1회만 클리어 보상
                {
                    if (!_lastLevelClearRewardGiven) //아직 1회 지급 안 했으면
                    {
                        _rewardPlanner?.GiveClearReward(preset); // 마지막 레벨 클리어 보상 1회 지급
                        _lastLevelClearRewardGiven = true;       // 이후부터는 지급 금지
                    }
                    // 이후 루프에서는 클리어 보상 없음
                }
            }
            else
            {
                _rewardPlanner?.TryGivePatternReward(patternIndex); //일반 패턴 보상
            }

            _rythmVisual.PatternSuccess(true);
        }
        else
        {
            //체력 깎기
            _playerCurHeart--;
            _rythmVisual.PatternSuccess(false);
            _uiManager.RemoveHeart();

            if (_playerCurHeart <= 0)
            {
                GameOver();
            }
        }
    }

    // RewardPlanner가 “이 보상 지급해”라고 하면 여기서 누적
    private void HandleGiveReward(RewardType type, int amount, bool isLast)
    {
        if (type == RewardType.None) return; //물고기 표시용, 여기선 무시해도 됨

        if (type == RewardType.Coin)
        {
            amount = Mathf.FloorToInt(amount * _coinMul); //골드 배율만큼 더 획득
        }

        if (type == RewardType.Egg)
        {
            int rand = Random.Range(0, _rewardEggs.Length);
            EggData egg = EggDataGenerator.GenerateRewardEgg(_rewardEggs[rand]);
            base.GainEgg(egg);
        }
        else
        {
            GainItem(type, amount); // 아이템 누적
        }

        Debug.Log($"{type}x{amount} 지급");
    }

    //특수 능력
    public void ApplyAbilities()
    {
        if (_effectContext == null) { Debug.LogWarning("_effectContext 없음"); return; }

        _coinMul = _effectContext.GoldMultiplier; //코인 배율
        _heartExtra = _effectContext.ExtraHeart; //추가하트
    }
}
