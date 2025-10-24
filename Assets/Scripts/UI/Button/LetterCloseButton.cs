using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject _pet;
    [SerializeField] private GameObject _confirmPopUp;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ShowConfirmPopUp);
    }

    private void ShowConfirmPopUp()
    {
        _confirmPopUp.SetActive(true);
    }    

}
