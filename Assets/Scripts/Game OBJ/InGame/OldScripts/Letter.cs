using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    private PetUnit _pet;
    private LeftReason _reason;
    public void Init(PetUnit pet, LeftReason reason)
    {
        _pet = pet;
        _reason = reason;
    }
    private void OnMouseDown()
    {
        Debug.Log("편지 클릭");
        InGameUIManager ui = FindObjectOfType<InGameUIManager>();
        if (ui == null)
        {
            return;
        }
        ui.TryOpenLetter(_pet, _reason);
    }
}
