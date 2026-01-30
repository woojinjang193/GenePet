using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemovePetButton : MonoBehaviour, IConfirmRequester
{
    [SerializeField] private Button _removeButton;
    [SerializeField] private PetManager _petManager;

    private void Awake()
    {
        if(_petManager == null ) { _petManager = FindObjectOfType<PetManager>(); }
        _removeButton.onClick.AddListener(TryToRemovePet);
    }

    private void TryToRemovePet()
    {
        if (_petManager.ZoomedPet == null) { Debug.LogWarning("선택된 펫 없음."); return; }

        //성장 검사
        if (_petManager.ZoomedUnit.Status.Growth == GrowthStatus.Egg)
        {
            Manager.Game.ShowPopup("PopText_NotAdult"); //TODO: 로컬라이제이션
            return;
        }

        if(Manager.Game != null)
        {
            Manager.Game.ShowConfirmMessage("Warning_RemovePet",0, this);
        }
    }

    public void Confirmed(int requestNum)
    {
        if (requestNum == 0) //펫 삭제
        {
            _petManager.RemovePet();
        }
    }
    public void Canceled(int requestNum) { }
}
