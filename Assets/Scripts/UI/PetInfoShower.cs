using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PetInfoShower : MonoBehaviour
{
    [SerializeField] private TMP_Text _petInfo;

    private void Awake()
    {
        UpdateText();

        //이벤트 구독 여기에
    }
    private void OnDestroy()
    {
        //이벤트 해제 여기에
    }

    private void UpdateText()
    {
        var user = Manager.Save.CurrentData.UserData;
        int havePet = user.HavePetList.Count;
        int userMaxPet = user.MaxPetAmount;

        _petInfo.text = $"{havePet} / {userMaxPet}";
    }
}
