using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChangeButton : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button _krButton;
    [SerializeField] private Button _enButton;
    [SerializeField] private Button _deButton;
    [SerializeField] private Button _spButton;
    [SerializeField] private Button _jpButton;
    [SerializeField] private Button _chsButton;
    [SerializeField] private Button _chtButton;

    [Header("Colors")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _selectedColor;

    private Button _currentButton;

    private void Awake()
    {
        _krButton.onClick.AddListener(() => Change(Language.KR, _krButton));
        _enButton.onClick.AddListener(() => Change(Language.EN, _enButton));
        _deButton.onClick.AddListener(() => Change(Language.DE, _deButton));
        _spButton.onClick.AddListener(() => Change(Language.SP, _spButton));
        _jpButton.onClick.AddListener(() => Change(Language.JP, _jpButton));
        _chsButton.onClick.AddListener(() => Change(Language.CHS, _chsButton));
        _chtButton.onClick.AddListener(() => Change(Language.CHT, _chtButton));

        Init();
    }
    private void Init()
    {
        Language curLang = Manager.Lang.CurLanguage;

        switch (curLang)
        {
            case Language.KR: _currentButton = _krButton; break;
            case Language.EN: _currentButton = _enButton; break;
            case Language.DE: _currentButton = _deButton; break;
            case Language.SP: _currentButton = _spButton; break;
            case Language.JP: _currentButton = _jpButton; break;
            case Language.CHS: _currentButton = _chsButton; break;
            case Language.CHT: _currentButton = _chtButton; break;
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
