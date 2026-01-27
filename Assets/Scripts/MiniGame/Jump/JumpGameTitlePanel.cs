
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
    public void Open(int curScore, int bestScore)
    {
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        _playButton.interactable = true;
    }
    private void OnPlayClicked()
    {
        if (!Manager.Mini.CanPlayMiniGame(out int cost)) return;

        var user = Manager.Save.CurrentData.UserData;
        user.Energy = Mathf.Max(0, user.Energy - cost); // 음수 방어

        _playButton.interactable = false; //여러번 눌림 방지

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
