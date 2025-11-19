using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInputPopUp : MonoBehaviour
{
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _errorTooLong;
    [SerializeField] private GameObject _error;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    private void OnConfirmClicked()
    {
        if (string.IsNullOrWhiteSpace(_inputField.text))
        {
            _error.SetActive(true);
            _errorTooLong.SetActive(false);
            return;
        }

        if (_inputField.text.Length >= 10)
        {
            _error.SetActive(false);
            _errorTooLong.SetActive(true);
            return;
        }

        _error.SetActive(false);
        _errorTooLong.SetActive(false);

        //Manager.Save.CurrentData.UserData.HavePet.DisplayName = _inputField.text;
        Debug.Log($"이름 설정: {_inputField.text}");
        gameObject.SetActive(false);
    }
}
