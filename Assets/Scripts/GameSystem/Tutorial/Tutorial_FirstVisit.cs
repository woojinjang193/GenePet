using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_FirstVisit : TutorialBase
{
    [Header("대사 넘김버튼")]
    [SerializeField] private Button _dialogueButton;
    [Header("스킵 버튼")]
    [SerializeField] private Button _skipButton;
    [Header("이미지")]
    [SerializeField] private Image _image;
    [Header("TMP")]
    [SerializeField] private TMP_Text _text;
    [Header("TMP 이름")]
    [SerializeField] private TMP_Text _nameText;
    [Header("대사")]
    [SerializeField] private List<LineInfo> _lines;

    private int _curLine = -1;
    private void Awake()
    {
        _dialogueButton.onClick.AddListener(NextLine);
        _skipButton.onClick.AddListener(Skip);
    }
    public override void Enter()
    {
        gameObject.SetActive(true);
        NextLine();
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
    }
    private void Skip()
    {
        OnCompleted();
    }
    private void NextLine()
    {
        if(_curLine >= _lines.Count - 1)
        {
            OnCompleted();
            return;
        }

        _curLine++;

        LineInfo lineInfo = _lines[_curLine];

        if (lineInfo == null) //할당 안됐으면 다음으로
        {
            NextLine();
            return;
        }

        Sprite sprite = lineInfo.Sprite != null ? lineInfo.Sprite : null; //있으면 쓰고 없으면 null
        string nextLineID = string.IsNullOrEmpty(lineInfo.TextID) ? "" : lineInfo.TextID;
        string nameID = string.IsNullOrEmpty(lineInfo.NameID) ? "" : lineInfo.NameID;


        _image.sprite = sprite;
        _image.gameObject.SetActive(sprite != null); //이미지 있으면 띄워줌
        
        string nextLine = Manager.Lang.GetText(nextLineID); //번역
        string nextName = Manager.Lang.GetText(nameID); //번역
        //Debug.Log(nextLine);
        _nameText.text = nextName;
        _text.text = nextLine;
    }
}
