using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemovePetButton : MonoBehaviour, IConfirmRequester
{
    [SerializeField] private Button _removeButton;
    [SerializeField] private PetManager _petManager;
    [SerializeField] private ConfirmMessage _confirmMessage;

    private void Awake()
    {
        if(_petManager == null ) { _petManager = FindObjectOfType<PetManager>(); }
        _removeButton.onClick.AddListener(TryToRemovePet);
    }

    private void TryToRemovePet()
    {
        if (_petManager.ZoomedPet == null) { Debug.LogWarning("선택된 펫 없음."); return; }

        //어른검사
        if (_petManager.ZoomedUnit.Status.Growth != GrowthStatus.Adult) return; 

        if(_confirmMessage == null)
        {
            _confirmMessage = FindObjectOfType<ConfirmMessage>(true);
        }
        _confirmMessage.OpenConfirmUI(Confirm.RemovePet, this);
    }

    public void Confirmed()
    {
        _petManager.RemovePet();
    }
    public void Canceled() { }
}
