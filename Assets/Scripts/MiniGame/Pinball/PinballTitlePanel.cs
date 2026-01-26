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
    private void OnStartButtonClicked()
    {
        if (!Manager.Mini.CanPlayMiniGame()) return;
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
