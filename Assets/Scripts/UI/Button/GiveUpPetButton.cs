using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiveUpPetButton : MonoBehaviour
{
    [SerializeField] private GameObject _popUp;
    [SerializeField] private GameObject _letterPanel;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        //Manager.Save.RemovePet();
        _letterPanel.SetActive(false);
        _popUp.SetActive(false);
    }
}
