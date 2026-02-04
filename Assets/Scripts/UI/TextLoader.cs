using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLoader : MonoBehaviour
{
    [SerializeField] private string _textID;
    private TMP_Text _text;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        Manager.Lang.OnLanguageChanged += UpdateFont;
    }
    private void OnEnable()
    {
        ApplyTextAndFont();
    }

    private void OnDestroy()
    {
        if(Manager.Lang != null)
        Manager.Lang.OnLanguageChanged -= UpdateFont;
    }

    private void UpdateFont(TMP_FontAsset font) //언어 변경 시 폰트도 같이 적용
    {
        ApplyTextAndFont();
    }
    //================텍스트 + 폰트 동시 적용===================
    private void ApplyTextAndFont() 
    {
        if(!string.IsNullOrWhiteSpace(_textID))
        {
            _text.text = Manager.Lang.GetText(_textID);   // 텍스트 갱신
        }
        _text.font = Manager.Lang.CurFont; // 폰트 갱신
    }
}
