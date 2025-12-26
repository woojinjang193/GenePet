using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPet : MonoBehaviour, IConfirmRequester
{
    [Header("비쥬얼 로더")]
    [SerializeField] private IslandPetVisualLoader _visualLoader;

    [Header("좌우 버튼")]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [Header("확인/취소 버튼")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    [Header("IslandManager")]
    [SerializeField] private IslandManager _islandManager;

    [Header("Confirm Popup")]
    [SerializeField] private ConfirmMessage _popup;

    private int _curIndex;
    private int _ogIndex;

    private List<PetSaveData> _petList = new();

    private void Awake()
    {
        _leftButton.onClick.AddListener(OnLeftButtonClicked);
        _rightButton.onClick.AddListener(OnRightButtonClicked);
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        _cancelButton.onClick.AddListener(OnCancelButtonClicked);
    }
    private void OnEnable()
    {
        _petList.Clear();
        GetPetList();

        string selectedId = _islandManager.IslandMyPetID;

        if (string.IsNullOrWhiteSpace(selectedId))
        {
            _ogIndex = -1;
            _curIndex = 0;
        }
        else
        {
            _ogIndex = GetIndexByID(selectedId);
            _curIndex = _ogIndex;
        }

        if (_petList.Count > 0)
        {
            DisplayPetImage(_curIndex);
        }
    }
    private void OnConfirmButtonClicked()
    {
        if (_ogIndex == _curIndex)
        {
            gameObject.SetActive(false);
            return;
        }

        if(_ogIndex == -1)
        {
            ApplyFinalChange(_curIndex);
            return;
        }

        _popup.OpenConfirmUI("Warning_AffinityReset", this);
    }
    private void OnCancelButtonClicked()
    {
        if (_ogIndex == -1)
        {
            ApplyFinalChange(0);
            DisplayPetImage(0);
            gameObject.SetActive(false);
            return;
        }
        
        DisplayPetImage(_ogIndex);
        gameObject.SetActive(false);
    }
    private void OnLeftButtonClicked()
    {
        if (_curIndex - 1 < 0)
        {
            _curIndex = _petList.Count - 1;
        }
        else
        {
            _curIndex--;
        }

        DisplayPetImage(_curIndex);
    }
    private void OnRightButtonClicked()
    {
        if (_curIndex + 1 >= _petList.Count)
        {
            _curIndex = 0;
        }
        else
        {
            _curIndex++;
        }

        DisplayPetImage(_curIndex);
    }
    private void DisplayPetImage(int index)
    {
        if (_petList.Count == 0) return;

        var data = _petList[index];

        if (data.IsLeft)
        {
            _visualLoader.LoadIslandPet(null);
            return;
        }
        _visualLoader.LoadIslandPet(data);
    }

    public void Confirmed()
    {
        ApplyFinalChange(_curIndex);
    }
    public void Canceled()
    {
        Rollback(_ogIndex);
    }

    public void ApplyFinalChange(int newIndex)
    {
        Manager.Save.CurrentData.UserData.Island.IslandMyPetID = _petList[newIndex].ID;
        _islandManager.UpdateIslandMyPetID(_petList[newIndex]);

        gameObject.SetActive(false);
    }
    public void Rollback(int ogIndex)
    {
        _curIndex = ogIndex;
        DisplayPetImage(ogIndex);
    }
    private int GetIndexByID(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return 0;

        for (int i = 0; i < _petList.Count; i++)
        {
            if (_petList[i].ID == id) return i;
        }
        return 0;
    }

    private void GetPetList() //떠나지않은 어른 개체만 담음
    {
        var petListFromSave = Manager.Save.CurrentData.UserData.HavePetList;
        PetSaveData curPet = _islandManager.IslandMypetData;

        if (curPet != null)
        {
            if (curPet.IsLeft) _petList.Add(curPet);
        }
        
        for (int i = 0; i < petListFromSave.Count;i++)
        {
            if (petListFromSave[i].GrowthStage == GrowthStatus.Adult && !petListFromSave[i].IsLeft)
            {
                _petList.Add(petListFromSave[i]);
            }
        }
    }
}
