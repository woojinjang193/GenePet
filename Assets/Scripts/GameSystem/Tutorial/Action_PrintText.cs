using TMPro;
using UnityEngine;

public class Action_PrintText : TutorialActionBase // TextID를 번역해서 TMP_Text에 출력하는 액션
{
    [Header("TMP")]
    [SerializeField] private TMP_Text _text;

    [Header("텍스트ID")]
    [SerializeField] private string _textId;

    public override void Execute()
    {
        if (_text == null) return;

        if (string.IsNullOrEmpty(_textId)) //ID가 비어있으면 빈 문자열 처리
        {
            _text.text = string.Empty;
            return;
        }

        string localized = Manager.Lang.GetText(_textId); // 번역
        _text.text = localized; // 출력
    }
}
