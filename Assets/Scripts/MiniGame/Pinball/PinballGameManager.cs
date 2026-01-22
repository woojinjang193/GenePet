using TMPro;
using UnityEngine;

public class PinballGameManager : MiniGameBase
{
    [Header("참조")]
    [SerializeField] private PinballBrickSpanwer _spawner;
    [SerializeField] private PinballVisualManager _visualManager;
    [SerializeField] private PinballUiManager _uiManager;

    [Header("점수")]
    [SerializeField] private TMP_Text _curScoreText;

    [Header("레벨 프리셋")]
    [SerializeField] private PinballGameLevelPresetSO[] _presets;

    // ===== 내부 상태 =====


    // ===== 미니게임별 능력 계수 =====
    private float _coinMul = 1f;  //코인 아이템 획득 배율
    
    // ============== 초기화 ===============
    protected override void Start()
    {
        base.Start();
    }
    public void OnGameStartClicked() //게임 시작 버튼 눌림
    {
        GameReset();
        base.GameStart();
        _isGameOver = false;
    }
    protected override void GameReset()
    {
        ApplyAbilities();
        base.GameReset();

        _spawner.StartSettingBricks(_presets[0]); //0 레벨 세팅

        _curScoreText.text = $"Score: {_score}";
    }
    public void GoBackHome()
    {
        FinishGame();
    }
    public void OnGameEnd()
    {
        _isGameOver = true;

        Debug.Log("게임오버");
    }
    // ================게임 사이클 ====================
    private void OnBrickBroken(BrickColor color, Vector3 worldPos) //블록 파괴시
    {
        //블록 파괴 이벤트 여기에
    }
    private void OnAddScore(int score) //점수 추가
    {
        AddScore(score);
        _curScoreText.text = $"Score: {_score}";
    }
    private void OnGivenItem(BrickColor color, LevelReward rewardInfo, Vector3 worldPos) //아이템 획득
    {
        if (color != BrickColor.None) return;

        //일반 블록 아이템이면 바로 획득
        RewardType type = rewardInfo.RewardType;
        int amount = rewardInfo.Amount;

        GainItem(type, amount);
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

    //===========특수능력 ====================
    public void ApplyAbilities()
    {
        if (_effectContext == null) { Debug.LogWarning("_effectContext 없음"); return; }

        _coinMul = _effectContext.GoldMultiplier; //코인 배율
    }
}
