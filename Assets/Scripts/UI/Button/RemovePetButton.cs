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

        //어른검사
        if (_petManager.ZoomedUnit.Status.Growth != GrowthStatus.Adult)
        {
            //Manager.Game.ShowPopup("PopText_NotAdult"); //스프레드 시트에 추가
            return;
        }

        if(Manager.Game != null)
        {
            Manager.Game.ShowWarning("Warning_RemovePet", this);
        }
    }

    public void Confirmed()
    {
        _petManager.RemovePet();
    }
    public void Canceled() { }
}
