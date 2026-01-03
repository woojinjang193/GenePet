using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EggListUI : MonoBehaviour
{
    [Header("알 버튼 목록")]
    [SerializeField] private Button[] _buttons;
    [SerializeField] private Image[] _images;

    [Header("랜덤스폰 버튼")]
    [SerializeField] private Button _randomSpawnButton;
    [SerializeField] private TMP_Text _priceText;

    [Header("닫기 버튼")]
    [SerializeField] private Button _closeButton;

    [Header("펫 매니저")]
    [SerializeField] private PetManager _petManager;

    private List<EggData> _curEggList;
    private int _randomSpawnPrice;
    private void Awake()
    {
        if (_petManager == null)
        {
            _petManager = FindObjectOfType<PetManager>();
        }
        _randomSpawnButton.onClick.AddListener(OnRandomClicked);
        _closeButton.onClick.AddListener(OnCloseClicked);

        for (int i = 0; i < _buttons.Length; i++)
        {
            int idx = i;
            _buttons[i].onClick.AddListener(() => OnEggClicked(idx));
        }

        _randomSpawnPrice = Manager.Game.Config.RandomSpawnPrice; //가격
        _priceText.text = _randomSpawnPrice.ToString();
    }
    public void Open()
    {
        gameObject.SetActive(true);
        Init();
    }
    private void Init() //초기화
    {
        _curEggList = Manager.Save.CurrentData.UserData.EggList;
        int maxEgg = Manager.Game.Config.MaxEggAmount;

        if (_curEggList == null) return;

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (i >= maxEgg)
            {
                _buttons[i].interactable = false;
                _images[i].sprite = null;
                Debug.LogWarning("알 개수 이상함");
                continue;
            }

            if (i < _curEggList.Count)
            {
                _buttons[i].interactable = true;
                _images[i].sprite = _curEggList[i].PetSaveData.EggSprite;
            }
            else
            {
                _buttons[i].interactable = false;
                _images[i].sprite = null;
            }
        }
    }
    private void OnEggClicked(int index) //알 클릭시
    {
        if (_curEggList == null) return;
        if (index < 0 || index >= _curEggList.Count) return;

        if (CanSpawn())
        {
            Manager.Save.RegisterNewPet(_curEggList[index].PetSaveData, true);
            _petManager.SpawnPet(_curEggList[index].PetSaveData);
            _curEggList.RemoveAt(index);
            Init();
            gameObject.SetActive(false);
        }
    }
    private void OnRandomClicked() //랜덤소환 클릭
    {
        int haveMoney = Manager.Save.CurrentData.UserData.Items.Money;

        if (_randomSpawnPrice > haveMoney)
        {
            Manager.Game.ShowPopup("You are broke");
            return;
        }
        if (CanSpawn())
        {
            Manager.Game.CreateRandomPet(true);
            Manager.Item.AddOrSubtractMoney(-_randomSpawnPrice);
            gameObject.SetActive(false);
        }
    }
    private void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }
    private bool CanSpawn() //소환 가능 여부
    {
        int maxPetAmount = Manager.Game.Config.MaxPetAmount;
        int playerMaxAmount = Manager.Save.CurrentData.UserData.PetSlot;
        int havePet = Manager.Save.CurrentData.UserData.HavePetList.Count;

        if (havePet >= maxPetAmount || havePet >= playerMaxAmount)
        {
            Debug.Log("펫 자리 없음");
            Manager.Game.ShowPopup("Lack of slot");
            return false;
        }
        else
            return true;
    }
}
