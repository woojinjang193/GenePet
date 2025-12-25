using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : MonoBehaviour
{
    [SerializeField] private PetManager _petManager;
    [SerializeField] private GameObject[] _hiddenUIs;

    [SerializeField] private TMP_InputField _input;
    [SerializeField] private GameObject _namePanel;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TMP_Text _nameText;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(OnclickedConfirm);
    }
    private void OnEnable()
    {
        if (string.IsNullOrWhiteSpace(_petManager.ZoomedPet.DisplayName))
        {
            TurnOnUIs(false);
        }
    }

    private void OnclickedConfirm()
    {
        //이름 예외처리 여기에 

        _nameText.text = _petManager.ZoomedPet.DisplayName = _input.text;
        TurnOnUIs(true);
        Debug.Log($"이름설정 : {_petManager.ZoomedPet.DisplayName} ");
    }

    private void TurnOnUIs(bool on)
    {
        _namePanel.SetActive(!on);

        foreach (GameObject go in _hiddenUIs)
        {
            go.SetActive(on);
        }
    }
}
