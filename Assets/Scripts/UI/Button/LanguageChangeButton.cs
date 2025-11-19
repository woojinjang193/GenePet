using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChangeButton : MonoBehaviour
{
    [SerializeField] private Button _korButton;
    [SerializeField] private Button _engButton;
    [SerializeField] private Button _jpButton;
    [SerializeField] private Button _deButton;
    [SerializeField] private Button _chButton;

    private void Awake()
    {
        _korButton.onClick.AddListener(ChangeToKor);
        _engButton.onClick.AddListener(ChangeToEng);
        _jpButton.onClick.AddListener(ChangeToJP);
        _deButton.onClick.AddListener(ChangeToDE);
        _chButton.onClick.AddListener(ChangeToCH);
    }

    private void ChangeToKor()
    {
        Manager.Lang.ChangeLanguage(Language.KOR);
        gameObject.SetActive(false);
    }
    private void ChangeToEng()
    {
        Manager.Lang.ChangeLanguage(Language.ENG);
        gameObject.SetActive(false);
    }
    private void ChangeToJP()
    {
        Manager.Lang.ChangeLanguage(Language.JP);
        gameObject.SetActive(false);
    }
    private void ChangeToDE()
    {
        Manager.Lang.ChangeLanguage(Language.DE);
        gameObject.SetActive(false);
    }
    private void ChangeToCH()
    {
        Manager.Lang.ChangeLanguage(Language.CH);
        gameObject.SetActive(false);
    }
}
