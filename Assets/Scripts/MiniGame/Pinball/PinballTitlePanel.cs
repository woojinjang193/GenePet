using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinballTitlePanel : MonoBehaviour
{
    [SerializeField] private PinballGameManager _pinballManager;

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _howToPlayButton;
    [SerializeField] private Button _goHomeButton;

    private void Awake()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _howToPlayButton.onClick.AddListener(OnHowToPlayButtonClicked);
        _goHomeButton.onClick.AddListener(OnGoHomeButtonClicked);
    }
    private void OnEnable()
    {
        _startButton.interactable = true;
    }
    private void OnStartButtonClicked()
    {
        if (!Manager.Mini.CanPlayMiniGame(out int cost)) return;

        var user = Manager.Save.CurrentData.UserData;
        user.Energy = Mathf.Max(0, user.Energy - cost); // 음수 방어

        _startButton.interactable = false; //버튼 여러번 눌리기 방지

        _pinballManager.OnGameStartClicked();
        gameObject.SetActive(false);
    }
    private void OnHowToPlayButtonClicked()
    {
        //TODO: 튜토리얼 오픈
    }
    private void OnGoHomeButtonClicked()
    {
        _pinballManager.GoBackHome();
    }
}
