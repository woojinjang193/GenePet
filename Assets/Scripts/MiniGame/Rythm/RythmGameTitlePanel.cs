using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RythmGameTitlePanel : MonoBehaviour
{
    [SerializeField] private RythmGameManager _rythmManager;

    [Header("버튼")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _howToPlayButton;
    [SerializeField] private Button _goHomeButton;

    [Header("점수 텍스트")]
    [SerializeField] private TMP_Text _curScoreText;
    [SerializeField] private TMP_Text _bestScoreText;

    [Header("점수 이미지들")]
    [SerializeField] private GameObject _bestScoreSPrite;
    [SerializeField] private GameObject _curScoreSPrite;
    [SerializeField] private GameObject _newRecordSprite;

    private void Awake()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _howToPlayButton.onClick.AddListener(OnHowToPlayButtonClicked);
        _goHomeButton.onClick.AddListener(OnGoHomeButtonClicked);

        int bestScore = Manager.Mini.GetBestScore(MiniGame.Rythm);

        _curScoreSPrite.SetActive(false);
        _newRecordSprite.SetActive(false);
        _bestScoreText.text = bestScore.ToString();
        _curScoreText.text = "";
    }
    private void OnEnable()
    {
        Manager.Audio.PlayBGM("BGM_Rythm");
        _startButton.interactable = true;
    }
    public void UpdateScore(int curScore, int bestScore)
    {
        bool isNewRecord = curScore == bestScore;

        if (!isNewRecord)
        {
            Manager.Audio.PlaySFX("GameOver");
        }
        else
        {
            Manager.Audio.PlaySFX("NewRecord");
        }
        _newRecordSprite.SetActive(isNewRecord);
        _curScoreSPrite.SetActive(true);

        _curScoreText.text = curScore.ToString();
        _bestScoreText.text = bestScore.ToString();
    }
    private void OnStartButtonClicked()
    {
        if (!Manager.Mini.CanPlayMiniGame(out int cost)) return;

        var user = Manager.Save.CurrentData.UserData;
        user.Energy = Mathf.Max(0, user.Energy - cost); // 음수 방어

        _startButton.interactable = false; //버튼 여러번 눌리기 방지

        _rythmManager.OnGameStartClicked();
        Manager.Audio.StopBGM();
        gameObject.SetActive(false);
    }
    private void OnHowToPlayButtonClicked()
    {
        //TODO: 튜토리얼 오픈
    }
    private void OnGoHomeButtonClicked()
    {
        _rythmManager.GoBackHome();
    }
}
