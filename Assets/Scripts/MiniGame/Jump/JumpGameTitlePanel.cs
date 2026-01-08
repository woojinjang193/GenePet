
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JumpGameTitlePanel : MonoBehaviour
{
    [SerializeField] private JumpGameUIManager _uiManager;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backHoemButton;
    [SerializeField] private TMP_Text _bestScore;

    private void Awake()
    {
        //TODO: 최고점수 가져오는 로직 추가
        _playButton.onClick.AddListener(OnPlayClicked);
        _backHoemButton.onClick.AddListener(OnBackHomeClicked);
    }
    private void OnPlayClicked()
    {
        _uiManager.StartGame();
    }
    private void OnBackHomeClicked()
    {
        _uiManager.GoHome();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
