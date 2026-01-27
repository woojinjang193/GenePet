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
    [SerializeField] private TMP_Text _errorText;

    [SerializeField] private int _nameLengthLimit = 10;

    private bool _isSubscribed = false;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(OnclickedConfirm);
    }
    private void OnEnable()
    {
        if (_petManager.ZoomedUnit == null) return;

        if (string.IsNullOrWhiteSpace(_petManager.ZoomedPet.DisplayName))
        {
            _nameText.text = null;
            _petManager.ZoomedUnit.Status.OnGrown += OnGrown;
            _isSubscribed = true; //구독중 플레그 변경

            if (_petManager.ZoomedUnit.Status.Growth == GrowthStatus.Egg)
            {
                TurnOnUIs(false, false);
            }
            else
            {
                TurnOnUIs(true, false);
                _input.text = null;
                _errorText.text = "";
            }
        }
        else
        {
            _nameText.text = _petManager.ZoomedPet.DisplayName;
            TurnOnUIs(false, true);
        }
    }
    public void CancelSubscribe()
    {
        if (!_isSubscribed) return; //구독중아니면 리턴

        _petManager.ZoomedUnit.Status.OnGrown -= OnGrown;
        _isSubscribed = false;
    }
    private void OnclickedConfirm()
    {
        if (string.IsNullOrWhiteSpace(_input.text))
        {
            _errorText.text = "It can not be empty"; //TODO: 로컬라이제이션
            return;
        }
        if(_input.text.Length > _nameLengthLimit)
        {
            _errorText.text = "It can not be longer than 10 letters"; //TODO: 로컬라이제이션
            return;
        }

        _nameText.text = _petManager.ZoomedPet.DisplayName = _input.text;
        TurnOnUIs(false, true);
        Debug.Log($"이름설정 : {_petManager.ZoomedPet.DisplayName} ");
    }

    private void TurnOnUIs(bool namePanel, bool others)
    {
        _namePanel.SetActive(namePanel);

        foreach (GameObject go in _hiddenUIs)
        {
            go.SetActive(others);
        }
    }

    private void OnGrown(GrowthStatus growth)
    {
        if (growth != GrowthStatus.Egg)
        {
            if (string.IsNullOrWhiteSpace(_petManager.ZoomedPet.DisplayName) && !_namePanel.activeSelf)
            {
                TurnOnUIs(true, false);
            }   
        }
    }
}
