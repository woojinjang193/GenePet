using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomOutButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ZoomOutClicked);
    }
    private void ZoomOutClicked()
    {
        PetManager petmanger = FindObjectOfType<PetManager>();
        petmanger.ZoomOutPet();
    }
}
