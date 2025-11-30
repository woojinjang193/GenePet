using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPet : MonoBehaviour
{
    [Header("파츠 리스트")]
    [SerializeField] private PetPartSpriteList _renderers;

    [Header("좌우 버튼")]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [Header("확인/취소 버튼")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    [Header("IslandManager")]
    [SerializeField] private IslandManager _islandManager;

    [Header("Confirm Popup")]
    [SerializeField] private IslandPetChangeConfirm _popup;

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

        _popup.Open(this, _curIndex, _ogIndex);
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

        PetVisualHelper.ApplyVisual(data.Genes, _renderers);
    }
    public void ApplyFinalChange(int newIndex)
    {
        Manager.Save.CurrentData.UserData.Island.IslandMyPetID = _petList[newIndex].ID;
        _islandManager.UpdateIslandMyPetID(_petList[newIndex]);

        Manager.Save.CurrentData.UserData.Island.Affinity = 0;
        Debug.Log("호감도 초기화");

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

    private void GetPetList() //어른 개체만 담음
    {
        var petListFromSave = Manager.Save.CurrentData.UserData.HavePetList;
        for (int i = 0; i < petListFromSave.Count;i++)
        {
            if (petListFromSave[i].GrowthStage == GrowthStatus.Adult)
            {
                _petList.Add(petListFromSave[i]);
            }
        }
    }
}
