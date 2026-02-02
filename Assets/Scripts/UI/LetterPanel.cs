using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterPanel : MonoBehaviour, IConfirmRequester, IAdRequester
{
    [Header("유저가 보게될 그림")]
    [SerializeField] private Image _reasonSprite;

    [Header("사진")]
    [SerializeField] private PetPictureOnLetter _picture;

    [Header("떠난 이유 스프라이트\nKR = 0, EN = 1, DE = 2, SP = 3, JP = 4, CHS = 5, CHT = 6,")]
    [SerializeField] private Sprite[] _hunger;
    [SerializeField] private Sprite[] _dirty;
    [SerializeField] private Sprite[] _unhappy;
    [SerializeField] private Sprite[] _sick;
    [SerializeField] private Sprite[] _noReason;

    [Header("버튼 & 아이템 소지 수")]
    [SerializeField] private Button _missingPosterButton;
    [SerializeField] private TMP_Text _missingPosterAmount;

    [SerializeField] private Button _watchAdButton;
    [SerializeField] private Button _giveUpButton;
    [SerializeField] private Button _closeButton;

    public event Action OnClickMissingPoster;

    private Language _curLanguage;

    private void Awake()
    {
        _missingPosterButton.onClick.AddListener(OnMissingPosterClicked);
        _giveUpButton.onClick.AddListener(OnGiveUpClicked);
        _closeButton.onClick.AddListener(OnCloseClicked);
        _watchAdButton.onClick.AddListener(OnClickRequestAD);

        Manager.Item.OnRewardGranted += UpdateAmount;
    }
    private void OnDestroy()
    {
        if(Manager.Item != null)
        Manager.Item.OnRewardGranted -= UpdateAmount;
    }
    private void OnEnable()
    {
        int amount = Manager.Save.CurrentData.UserData.Items.MissingPoster;
        _missingPosterAmount.text = $"x{amount}";
    }
    public void SetLetter(LeftReason reason, PetSaveData pet, GrowthStatus growth)
    {
        WriteLetter(reason);
        _picture.SetPictureOnLetter(pet, growth);
    }
    private void WriteLetter(LeftReason reason)
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
    //==================광고버튼 클릭====================
    private void OnClickRequestAD()
    {
        Manager.AD.ShowRewardedAd(this);
    }
    private void OnMissingPosterClicked()
    {
        UserItemData items = Manager.Save.CurrentData.UserData.Items;
        
        if (items.MissingPoster <= 0)
        {
            Debug.Log("포스터 수량 부족");
            Manager.Game.ShowConfirmMessage("Asking_MoveToShop",0, this);
            return;
        }
        items.MissingPoster--;

        BringPetBack();
    }
    private void BringPetBack()
    {
        OnClickMissingPoster?.Invoke(); //펫 매니저가 구독함
        gameObject.SetActive(false);
    }
    private void OnGiveUpClicked() 
    {
        if (Manager.Game != null)
        {
            Manager.Game.ShowConfirmMessage("Warning_RemovePet",1, this);
        }
    }
    private void OnCloseClicked() 
    { 
        gameObject.SetActive(false);
    }
    //=================컨펌창 인터페이스 ========================
    public void Confirmed(int requestNum)
    {
        if (requestNum == 0) //상점이동 요청
        {
            Manager.Game.OpenUiPanel(UIPanel.Shop);
        }
        else if (requestNum == 1) //펫 포기 요청
        {
            PetManager petManager = FindObjectOfType<PetManager>();
            if (petManager != null)
            {
                gameObject.SetActive(false);
                petManager.RemovePet();
            }
        }
    }
    public void Canceled(int requestNum) { }

    private void UpdateAmount(RewardType item, int newValue)
    {
        if (item != RewardType.MissingPoster) return;

        _missingPosterAmount.text = $"x{newValue}";
    }
    public void AdWatched()
    {
        
    }
    public void AdClosed()
    {
        BringPetBack();
    }
}
