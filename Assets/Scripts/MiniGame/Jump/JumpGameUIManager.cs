
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpGameUIManager : MonoBehaviour 
{
    [Header("점프게임 매니저")]
    [SerializeField] private JumpMiniGame _jumpGame;

    [Header("패널")]
    [SerializeField] private JumpGameTitlePanel _titlePanel;

    private void Awake()
    {
        _jumpGame.OnGameOver += OnGameOver;
    }
    private void OnGameOver()
    {
        int curScore = _jumpGame.Score;
        int bestScore = Manager.Mini.GetBestScore(MiniGame.Jump);

        _titlePanel.UpdateScore(curScore, bestScore);
        _titlePanel.gameObject.SetActive(true);
    }
    // ===== 패널에서 호출 =====
    public void StartGame()
    {
        _jumpGame.OnGameStartClicked();
    }
    public void GoBackHome()
    {
        _jumpGame.GoBackHome();
    }
}
