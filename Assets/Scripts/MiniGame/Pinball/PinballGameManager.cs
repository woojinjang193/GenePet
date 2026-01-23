using TMPro;
using UnityEngine;

public class PinballGameManager : MiniGameBase
{
    [Header("참조")]
    [SerializeField] private PinballBrickSpanwer _spawner;
    [SerializeField] private PinballPlayer _player;
    [SerializeField] private MiniGamePetVisualLoader _petLoader;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private PinballUiManager _uiManager;

    [Header("점수 텍스트")]
    [SerializeField] private TMP_Text _curScoreText;

    [Header("게임 세팅")]
    [SerializeField] private PinballGamePresetSO _preset; 

    [Header("보상 세팅")]
    [SerializeField] private ItemForMiniGame _slot1Reward;
    [SerializeField] private ItemForMiniGame _slot2Reward;
    [SerializeField] private ItemForMiniGame _slot3Reward;
    [SerializeField] private ItemForMiniGame _slot4Reward;

    // ===== 미니게임별 능력 계수 =====
    private float _coinMul = 1f;  //코인 아이템 획득 배율

    private GameObject _rouletteMap;
    // ============== 초기화 ===============
    protected override void Start()
    {
        base.Start();
        _player.OnRewardGet += OnItemCollected;

        if(_pet != null)
        {
            _petLoader.LoadPetVisual(_pet, MiniGame.Pinball); //펫 로드
        }
    }
    private void OnDestroy()
    {
        _player.OnRewardGet -= OnItemCollected;
    }
    // ================게임 사이클 ====================
    public void OnGameStartClicked() //게임 시작 버튼 눌림
    {
        GameReset();
        base.GameStart();
        _isGameOver = false;

        _playerSpawner.OpenDoor(); //플레이어 소환
       
    }
    protected override void GameReset()
    {
        ApplyAbilities();
        base.GameReset();

        RouletteMapReset();
        RewardSlotReset();

        _player.FlagReset();
        _playerSpawner.CloseDoor();
        _spawner.StartSettingBricks(_preset); //0 레벨 세팅
        _curScoreText.text = $"Score: {_score}";

    }
    public void GoBackHome()
    {
        FinishGame();
    }

    //=================== 외부 호출 ======================
    private void OnBrickBroken(BrickColor color, Vector3 worldPos) //블록 파괴시
    {
        //블록 파괴 이벤트 여기에
    }
    private void OnAddScore(int score) //점수 추가
    {
        AddScore(score);
        _curScoreText.text = $"Score: {_score}";
    }
    private void OnGivenItem(BrickColor color, LevelReward rewardInfo, Vector3 worldPos) //아이템 블록 파괴 이벤트
    {
        if (color == BrickColor.None)
        {
            //일반 블록 아이템이면 바로 획득
            RewardType type = rewardInfo.RewardType;
            int amount = rewardInfo.Amount;

            GainItem(type, amount);
        }
        //색 블록이면 아래쪽에 세팅
        else if(color == BrickColor.one)
        {
            _slot1Reward.Init(rewardInfo.RewardType, rewardInfo.Amount);
        }
        else if (color == BrickColor.two)
        {
            _slot2Reward.Init(rewardInfo.RewardType, rewardInfo.Amount);
        }
        else if (color == BrickColor.three)
        {
            _slot3Reward.Init(rewardInfo.RewardType, rewardInfo.Amount);
        }
    }
    private void OnItemCollected(RewardType type, int amount) //플레이어가 룰렛 아이템 획득시 호출
    {
        //if(!_isPlaying) return;

        if (type == RewardType.Coin)
        {
            amount = Mathf.FloorToInt(amount * _coinMul); //골드 배율만큼 더 획득
        }

        GainItem(type, amount); // 아이템 누적

        base.GameOver();// 게임결과 저장
        _uiManager.GameEndUiOpen();
    }
    //============== 브릭 등록/해제 ==================
    public void RegisterBrick(PinballBrick brick)
    {
        brick.OnBroken += OnBrickBroken;
        brick.OnAddScore += OnAddScore;
        brick.OnGiveItem += OnGivenItem;
    }
    public void UnregisterBrick(PinballBrick brick)
    {
        brick.OnBroken -= OnBrickBroken;
        brick.OnAddScore -= OnAddScore;
        brick.OnGiveItem -= OnGivenItem;
    }
    // ================리셋 ===============
    private void RouletteMapReset()
    {
        if (_preset.RouletteMaps.Length == 0 || _preset.RouletteMaps == null) return; //룰렛맵 없으면 리턴

        if (_rouletteMap != null) //전에 맵 남아있으면 삭제
        {
            Destroy(_rouletteMap);
            _rouletteMap = null;
        }

        int rand = Random.Range(0, _preset.RouletteMaps.Length);

        _rouletteMap = Instantiate(_preset.RouletteMaps[rand], transform); // 룰렛맵 소환
    }
    private void RewardSlotReset()
    {
        _slot1Reward.ResetItem();
        _slot2Reward.ResetItem();
        _slot3Reward.ResetItem();

        //4번 슬롯 세팅
        LevelReward reward = MiniGameRewardPicker.GetRewardByWeight(_preset.LevelClearRewards);
        _slot4Reward.Init(reward.RewardType, reward.Amount);
    }
    //===========특수능력 ====================
    public void ApplyAbilities()
    {
        if (_effectContext == null) { Debug.LogWarning("_effectContext 없음"); return; }

        _coinMul = _effectContext.GoldMultiplier; //코인 배율
    }
}
