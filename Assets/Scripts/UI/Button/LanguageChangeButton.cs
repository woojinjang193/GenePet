using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChangeButton : MonoBehaviour
{
    [SerializeField] private Button _krButton;
    [SerializeField] private Button _enButton;
    [SerializeField] private Button _deButton;
    [SerializeField] private Button _jpButton;
    [SerializeField] private Button _chButton;
    [SerializeField] private Button _spButton;

    [Header("Colors")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _selectedColor;

    private Button _currentButton;

    private void Awake()
    {
        _krButton.onClick.AddListener(() => Change(Language.KR, _krButton));
        _enButton.onClick.AddListener(() => Change(Language.EN, _enButton));
        _deButton.onClick.AddListener(() => Change(Language.DE, _deButton));
        _jpButton.onClick.AddListener(() => Change(Language.JP, _jpButton));
        _chButton.onClick.AddListener(() => Change(Language.CH, _chButton));
        _spButton.onClick.AddListener(() => Change(Language.SP, _spButton));

        Init();
    }
    private void Init()
    {
        Language curLang = Manager.Lang.CurLanguage;

        switch (curLang)
        {
            case Language.KR: _currentButton = _krButton; break;
            case Language.EN: _currentButton = _krButton; break;
            case Language.DE: _currentButton = _krButton; break;
            case Language.JP: _currentButton = _krButton; break;
            case Language.CH: _currentButton = _krButton; break;
            case Language.SP: _currentButton = _krButton; break;
        }

        if (_currentButton != null)
        {
            _currentButton.interactable = false;
            _currentButton.image.color = _selectedColor;
        }
    }

    private void Change(Language lang, Button clickedButton)
    {
        // 이전 선택 버튼 복구
        if (_currentButton != null)
        {
            _currentButton.interactable = true;
            _currentButton.image.color = _normalColor;
        }

        // 새 버튼 선택 처리
        _currentButton = clickedButton;
        _currentButton.interactable = false;
        _currentButton.image.color = _selectedColor;

        // 언어 변경
        Manager.Lang.ChangeLanguage(lang);
    }
}
