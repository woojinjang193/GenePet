
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpGameUIManager : MonoBehaviour
{
    [Header("점프게임 매니저")]
    [SerializeField] private JumpMiniGame _jumpGame;

    [Header("패널")]
    [SerializeField] private JumpGameTitlePanel _titlePanel;
    [SerializeField] private JumpGameOverPanel _gameOverPanel;

    private void Awake()
    {
        _jumpGame.OnGameStart += OnGameStart;   // GameBase 이벤트 사용
        _jumpGame.OnGameOver += OnGameOver;
    }
    private void OnDestroy()
    {
        _jumpGame.OnGameStart -= OnGameStart;
        _jumpGame.OnGameOver -= OnGameOver;
    }
    private void OnGameStart()
    {
        _titlePanel.Close();
        _gameOverPanel.Close();
    }
    private void OnGameOver()
    {
        int curScore = _jumpGame.Score;
        int bestScore = curScore; // TODO: 실제 최고점수 연결

        _gameOverPanel.Open(curScore, bestScore);
    }
    // ===== 패널에서 호출 =====
    public void StartGame()
    {
        _jumpGame.OnGameStartClicked();
    }
    public void RestartGame()
    {
        _jumpGame.OnGameStartClicked();
    }
    public void GoHome()
    {
        SceneManager.LoadScene("InGameScene");
    }
}
