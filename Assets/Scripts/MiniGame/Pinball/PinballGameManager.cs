using TMPro;
using UnityEngine;

public class PinballGameManager : MiniGameBase
{
    [Header("점수")]
    [SerializeField] private float _scorePerfect = 1f;
    [SerializeField] private float _scoreGood = 1f;
    [SerializeField] private float _scoreMiss = -1f;
    [SerializeField] private TMP_Text _curScoreText;

    // ===== 내부 상태 =====


    // ===== 미니게임별 능력 계수 =====
    private float _coinMul = 1f;  //코인 아이템 획득 배율

    protected override void Start()
    {
        base.Start();
    }
    public void OnGameStartClicked() //게임 시작 버튼 눌림
    {
        GameReset();
        GameStart();
        _isGameOver = false;
    }
    protected override void GameReset()
    {
        ApplyAbilities();
        base.GameReset();
        _curScoreText.text = $"Score: {_score}";
    }
    public void GoBackHome()
    {
        FinishGame();
    }
    public void OnPlayerDead()
    {
        _isGameOver = true;

        Debug.Log("게임오버");
    }

    //===========특수능력 ====================
    public void ApplyAbilities()
    {
        if (_effectContext == null) { Debug.LogWarning("_effectContext 없음"); return; }

        _coinMul = _effectContext.GoldMultiplier; //코인 배율
    }
}
