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

    private void Awake()
    {
        _krButton.onClick.AddListener(() => Change(Language.KR));
        _enButton.onClick.AddListener(() => Change(Language.EN));
        _jpButton.onClick.AddListener(() => Change(Language.JP));
        _deButton.onClick.AddListener(() => Change(Language.DE));
        _chButton.onClick.AddListener(() => Change(Language.CH));
        _spButton.onClick.AddListener(() => Change(Language.SP));
    }

    private void Change(Language lang)
    {
        Manager.Lang.ChangeLanguage(lang);
        gameObject.SetActive(false);
    }
}
