using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JumpGameOverPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _curScore;
    [SerializeField] private TMP_Text _bestScore;

    [SerializeField] private Button _reStartButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        _reStartButton.onClick.AddListener(OnRestartClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
    }
    public void Open(int curScore, int bestScore)
    {
        _curScore.text = curScore.ToString();
        _bestScore.text = bestScore.ToString();

        gameObject.SetActive(true);
    }

    public void OnRestartClicked()
    {
        Debug.Log("다시하기 클릭");
        SceneManager.LoadScene("JumpGameScene");
    }
    public void OnExitClicked()
    {
        Debug.Log("나가기 클릭");
    }
}
