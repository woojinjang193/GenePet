using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandPetChangeConfirm : MonoBehaviour
{
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    private SelectPet _selectPet;
    private int _newIndex;
    private int _ogIndex;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(OnConfirm);
        _cancelButton.onClick.AddListener(OnCancel);
    }

    public void Open(SelectPet selectPet, int newIndex, int ogIndex)
    {
        _selectPet = selectPet;
        _newIndex = newIndex;
        _ogIndex = ogIndex;

        gameObject.SetActive(true);
    }

    private void OnConfirm()
    {
        _selectPet.ApplyFinalChange(_newIndex);
        gameObject.SetActive(false);
    }

    private void OnCancel()
    {
        _selectPet.Rollback(_ogIndex);
        gameObject.SetActive(false);
    }
}
