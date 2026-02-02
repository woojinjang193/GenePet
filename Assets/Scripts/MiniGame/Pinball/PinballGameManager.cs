using System.Collections;
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

    private int _remainingBricks; //현재 웨이브에서 남아있는 벽돌 수 카운트
    private bool _isWaveRespawning; //벽돌이 0 됐을 때 리젠이 중복으로 여러 번 호출되는 것 방지하는 플래그
    private Coroutine _respawnRoutine; //웨이브 리젠 코루틴

    private bool _isNextWavePending; // 다음 웨이브 스폰 대기
    // ===== 미니게임별 능력 계수 =====
    private float _coinMul = 1f;  //코인 아이템 획득 배율
    private int _ballDamage = 1;

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

        // 웨이브 카운트/리젠 상태 초기화
        _remainingBricks = 0; // 새 웨이브 시작 전 카운트 리셋
        _isWaveRespawning = false; // 리젠 플래그 초기화
        _isNextWavePending = false;

        if (_respawnRoutine != null) // 남아있는 코루틴 정리
        {
            StopCoroutine(_respawnRoutine); // 중복 리젠 방지
            _respawnRoutine = null; // 참조 정리
        }

        RouletteMapReset();
        RewardSlotReset();

        _player.BallReset(_ballDamage);
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
        //게임 중이 아니면 웨이브 카운트 처리 안함
        if (!_isPlaying || _isGameOver) return; //게임 종료/대기 상태 보호
        if (_isWaveRespawning) return; // 리젠 중 중복 트리거 방지

        _remainingBricks--; //벽돌 1개 파괴 처리
        if (_remainingBricks <= 0) // 웨이브 클리어 조건
        {
            _remainingBricks = 0; //음수 방지
            _isNextWavePending = true; // 소환존 들어올 때까지 대기
        }
    }
    // ============= 다음 웨이브 =============
    public void TrySpawnNextWaveFromZone()
    {
        if (!_isNextWavePending) return; //대기 상태 아니면 무시
        if (_isWaveRespawning) return; //이미 리스폰 중이면 무시
        if (!_isPlaying || _isGameOver) return;

        _isNextWavePending = false;
        RequestNextWave(); //웨이브 소환 시작
    }
    private void RequestNextWave() //웨이브 클리어시 리젠을 시작시키는 함수
    {
        // 중복 리젠 요청 방지
        if (_isWaveRespawning) return; // 이미 리젠 중이면 무시
        _isWaveRespawning = true; //리젠 플래그 ON

        if (_respawnRoutine != null) // 혹시 남아있으면 정리
        {
            StopCoroutine(_respawnRoutine); //중복 코루틴 방지
            _respawnRoutine = null; //참조 정리
        }

        _respawnRoutine = StartCoroutine(RespawnNextWaveRoutine()); //다음 프레임에 웨이브 교체
    }
    //=========== 다음 웨이브 리스폰 코루틴 ================
    private IEnumerator RespawnNextWaveRoutine()
    {
        yield return null; // 물리 콜백 프레임 분리(안전)

        // 게임 상태가 바뀌었으면 중단
        if (!_isPlaying || _isGameOver) //종료/대기 상태 보호
        {
            _isWaveRespawning = false; //플래그 원복
            _respawnRoutine = null; // 참조 정리
            yield break; // 중단
        }

        _remainingBricks = 0; // 새 웨이브 카운트 초기화(등록하면서 다시 올라감)
        _spawner.StartSettingBricks(_preset); // 랜덤 맵 다시 배치

        _isWaveRespawning = false; // 리젠 완료
        _respawnRoutine = null; // 참조 정리
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

        if(type == RewardType.Egg)
        {
            int rand = Random.Range(0, _preset.RewardEggs.Length);
            EggData egg = EggDataGenerator.GenerateRewardEgg(_preset.RewardEggs[rand]);
            base.GainEgg(egg);
        }
        else
        {
            GainItem(type, amount); // 아이템 누적
        }
  
        base.GameOver();// 게임결과 저장
        _uiManager.GameEndUiOpen();
    }
    //============== 브릭 등록/해제 ==================
    public void RegisterBrick(PinballBrick brick)
    {
        brick.OnBroken += OnBrickBroken;
        brick.OnAddScore += OnAddScore;
        brick.OnGiveItem += OnGivenItem;

        // 현재 웨이브 벽돌 수 카운트
        _remainingBricks++; // 웨이브 벽돌 +1
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
        if (_preset.RouletteMaps == null || _preset.RouletteMaps.Length == 0) return; //null 체크

        if (_rouletteMap != null) //전에 맵 남아있으면 반환
        {
            Manager.Pool.Release(_rouletteMap); //풀 반환
            _rouletteMap = null;
        }

        int rand = Random.Range(0, _preset.RouletteMaps.Length);

        _rouletteMap = Manager.Pool.Get(_preset.RouletteMaps[rand], transform.position, transform); //풀로 맵 소환
        _rouletteMap.transform.localPosition = Vector3.zero; //부모 기준 위치 정리
        _rouletteMap.transform.localRotation = Quaternion.identity; //회전 정리
        _rouletteMap.transform.localScale = Vector3.one; //스케일 정리
    }
    private void RewardSlotReset()
    {
        _slot1Reward.ResetItem();
        _slot2Reward.ResetItem();
        _slot3Reward.ResetItem();

        //4번 슬롯 세팅
        var pool = Manager.Mini.GetAvailableRewardPool(_preset.LevelClearRewards , _rewardReservation);//조건 필터링 리스트
        LevelReward reward = MiniGameRewardPicker.GetRandomReward(pool , _rewardReservation); //필터된 풀에서 뽑기

        if (reward.RewardType == RewardType.None)
        {
            Debug.LogError("보상 풀이 비었음");
            return;
        }
        _slot4Reward.Init(reward.RewardType, reward.Amount);
    }
    //===========특수능력 ====================
    public void ApplyAbilities()
    {
        if (_effectContext == null) { Debug.LogWarning("_effectContext 없음"); return; }

        _coinMul = _effectContext.GoldMultiplier; //코인 배율
        _ballDamage = _effectContext.BallDamage;
    }
}
