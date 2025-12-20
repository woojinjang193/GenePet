using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FamilyTreeButton : MonoBehaviour
{
    [SerializeField] private PetManager _petManager;
    [SerializeField] private FamilyTreeUI _familyTreeUI;
    [SerializeField] private Button _button;
    private void Awake()
    {
        if(_button == null) { _button = GetComponent<Button>(); }
        if( _petManager == null ) { _petManager = FindObjectOfType<PetManager>(); }
        if(_familyTreeUI == null ) { _familyTreeUI = FindObjectOfType<FamilyTreeUI>(); }
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        PetSaveData pet = _petManager.ZoomedPet;
        if (pet.GrowthStage != GrowthStatus.Adult)//어른이 아니면 리턴
        {
            Manager.Game.ShowPopup("Wait until it grows up");
            return; 
        } 

        _familyTreeUI.gameObject.SetActive(true);
        _familyTreeUI.Init(pet);
    }
}
