using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    [SerializeField] private Button _button;

    private PetUnit _pet;
    private LeftReason _reason;

    private void Awake()
    {
        if(_button == null)
        {
            _button = GetComponentInChildren<Button>();
        }
        _button.onClick.AddListener(OnLetterClicked);
    }
    public void Init(PetUnit pet, LeftReason reason)
    {
        _pet = pet;
        _reason = reason;
    }
    private void OnLetterClicked()
    {
        if(_pet.Petmanager.ZoomedUnit != _pet)
        {
            return;
        }

        //Debug.Log("편지 클릭");
        InGameUIManager ui = FindObjectOfType<InGameUIManager>();
        if (ui == null)
        {
            return;
        }
        ui.TryOpenLetter(_pet, _reason);
    }

    public void SetClickable(bool on)
    {
        //Debug.Log($"편지 클릭 설정 {on}");
        _button.interactable = on;
        _button.image.raycastTarget = on;
    }

}
