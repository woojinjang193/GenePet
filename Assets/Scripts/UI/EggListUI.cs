using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggListUI : MonoBehaviour
{
    [Header("알 버튼 목록")]
    [SerializeField] private Button[] _buttons;
    [SerializeField] private Image[] _images;

    [Header("랜덤스폰 버튼")]
    [SerializeField] private Button _randomSpawnButton;

    [Header("닫기 버튼")]
    [SerializeField] private Button _closeButton;

    [Header("펫 매니저")]
    [SerializeField] private PetManager _petManager;

    private List<EggData> _curEggList;
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
    }
    public void Open()
    {
        gameObject.SetActive(true);
        Init();
    }
    private void Init()
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
    private void OnEggClicked(int index)
    {
        if (_curEggList == null) return;
        if (index < 0 || index >= _curEggList.Count) return;

        if (CanSpawn())
        {
            _petManager.SpawnPet(_curEggList[index].PetSaveData);
            Manager.Save.RegisterNewPet(_curEggList[index].PetSaveData, true);
            _curEggList.RemoveAt(index);
            Init();
            gameObject.SetActive(false);
        }
    }
    private void OnRandomClicked()
    {
        if (CanSpawn())
        {
            Manager.Game.CreateRandomPet(true);
            gameObject.SetActive(false);
        }
    }
    private void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }
    private bool CanSpawn()
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
