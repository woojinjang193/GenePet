using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameTutorial : MonoBehaviour
{
    [SerializeField] private Image _screenShot;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [SerializeField] List<MiniTutorial> _tutorials;
    [SerializeField] private TMP_Text _text;

    private int _curIndex = -1;

    [Serializable]
    public class MiniTutorial
    {
        public Sprite ScreenShot;
        public string TextID;
    }

    private void Awake()
    {
        _leftButton.onClick.AddListener(OnLeftClicked);
        _rightButton.onClick.AddListener(OnRightClicked);
    }
    private void OnEnable()
    {
        _curIndex = 0;
        _leftButton.interactable = false;
        ShowPage();
    }
    private void OnLeftClicked()
    {
        _curIndex--;
        ButtonCheck();
        ShowPage();
        
    }
    private void OnRightClicked()
    {
        _curIndex++;
        ButtonCheck();
        ShowPage();
        
    }
    private void ShowPage()
    {
        var curMaterial = _tutorials[_curIndex];
        _screenShot.sprite = curMaterial.ScreenShot;
        _text.text = Manager.Lang.GetText(curMaterial.TextID);
    }
    private void ButtonCheck()
    {
        if (_curIndex <= 0) _curIndex = 0;
        if (_curIndex >= _tutorials.Count - 1) _curIndex = _tutorials.Count - 1;

        bool isLowest = _curIndex <= 0;
        bool isHighest = _curIndex >= _tutorials.Count - 1;

        _leftButton.interactable = !isLowest;
        _rightButton.interactable = !isHighest;
    }
}
