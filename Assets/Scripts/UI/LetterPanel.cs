using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterPanel : MonoBehaviour, IConfirmRequester
{
    [Header("컨펌 메세지")]
    [SerializeField] private ConfirmMessage _confirmMessage;

    [Header("유저가 보게될 그림")]
    [SerializeField] private Image _reasonSprite;

    [Header("떠난 이유 스프라이트\nKOR = 0, ENG = 1, DE = 2, JP = 3, CH = 4")]
    [SerializeField] private Sprite[] _hunger;
    [SerializeField] private Sprite[] _dirty;
    [SerializeField] private Sprite[] _unhappy;
    [SerializeField] private Sprite[] _sick;
    [SerializeField] private Sprite[] _noReason;

    [Header("버튼 & 아이템 소지 수")]
    //[SerializeField] private Button _callingButton;
    //[SerializeField] private TMP_Text _callingAmount;

    [SerializeField] private Button _missingPosterButton;
    [SerializeField] private TMP_Text _missingPosterAmount;

    [SerializeField] private Button _giveUpButton;
    [SerializeField] private Button _closeButton;

    public event Action OnClickMissingPoster;

    private Language _curLanguage;

    private void Awake()
    {
        //_callingButton.onClick.AddListener(OnCallingClicked);
        _missingPosterButton.onClick.AddListener(OnMissingPosterClicked);
        _giveUpButton.onClick.AddListener(OnGiveUpClicked);
        _closeButton.onClick.AddListener(OnCloseClicked);

    }
    private void OnEnable()
    {
        //_callingAmount.text = "0";
        _missingPosterAmount.text = Manager.Save.CurrentData.UserData.Items.MissingPoster.ToString();
    }
    public void WriteLetter(LeftReason reason)
    {
        _curLanguage = Manager.Lang.CurLanguage;
        switch (reason)
        {
            case LeftReason.Hunger: 
                _reasonSprite.sprite = _hunger[(int)_curLanguage];
                break;
            case LeftReason.Dirty:
                _reasonSprite.sprite = _dirty[(int)_curLanguage];
                break;
            case LeftReason.Unhappy:
                _reasonSprite.sprite = _unhappy[(int)_curLanguage];
                break;
            case LeftReason.Sick:
                _reasonSprite.sprite = _sick[(int)_curLanguage];
                break;
            case LeftReason.NoReason:
                _reasonSprite.sprite = _noReason[(int)_curLanguage];
                break;
        }
    }
    private void OnCallingClicked()
    {
    }
    private void OnMissingPosterClicked()
    {
        UserItemData items = Manager.Save.CurrentData.UserData.Items;
        
        if (items.MissingPoster <= 0)
        {
            Debug.Log("포스터 수량 부족");
            return;
        }
        items.MissingPoster--;

        OnClickMissingPoster?.Invoke(); //펫 매니저가 구독함
        gameObject.SetActive(false);
    }
    private void OnGiveUpClicked() 
    {
        if (_confirmMessage == null)
        {
            _confirmMessage = FindObjectOfType<ConfirmMessage>(true);
        }
        _confirmMessage.OpenConfirmUI("Warning_RemovePet", this);
    }
    private void OnCloseClicked() 
    { 
        gameObject.SetActive(false);
    }
    public void Confirmed()
    {
        PetManager petManager = FindObjectOfType<PetManager>();
        if (petManager != null)
        {
            gameObject.SetActive (false);
            petManager.RemovePet();
        }
    }
    public void Canceled() { }
}
