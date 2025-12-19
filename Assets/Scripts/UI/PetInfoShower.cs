using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PetInfoShower : MonoBehaviour //현재 펫 수 보여주는 스크립트
{
    [SerializeField] private TMP_Text _petInfo;
    [SerializeField] private PetManager _petManager;

    private void Awake()
    {
        UpdateText();

        if( _petManager == null )
        {
            _petManager = FindObjectOfType<PetManager>();
        }
        _petManager.OnPetSpawned += UpdateText;
        _petManager.OnPetRemoved += UpdateText;
    }
    private void OnDestroy()
    {
        _petManager.OnPetSpawned -= UpdateText;
        _petManager.OnPetRemoved -= UpdateText;
    }

    private void UpdateText()
    {
        var user = Manager.Save.CurrentData.UserData;
        int havePet = user.HavePetList.Count;
        int userMaxPet = user.MaxPetAmount;

        _petInfo.text = $"{havePet} / {userMaxPet}";
    }
}
