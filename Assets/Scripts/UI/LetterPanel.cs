using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterPanel : MonoBehaviour
{
    [Header("유저가 보게될 그림")]
    [SerializeField] private Image _reasonSprite;

    [Header("떠난 이유 스프라이트\nKOR = 0, ENG = 1, DE = 2, JP = 3, CH = 4")]
    [SerializeField] private Sprite[] _hunger;
    [SerializeField] private Sprite[] _dirty;
    [SerializeField] private Sprite[] _unhappy;
    [SerializeField] private Sprite[] _sick;
    [SerializeField] private Sprite[] _noReason;

    private Language _curLanguage;
 
    public void WriteLetter(LeftReason reason)
    {
        _curLanguage = Manager.Lang.CurLanguage;
        switch (reason)
        {
            case LeftReason.Hunger: 
                _reasonSprite.sprite = _hunger[(int)_curLanguage];
                break;
            case LeftReason.Dirty:
                _reasonSprite.sprite = _dirty[(int)_curLanguage];
                break;
            case LeftReason.Unhappy:
                _reasonSprite.sprite = _unhappy[(int)_curLanguage];
                break;
            case LeftReason.Sick:
                _reasonSprite.sprite = _sick[(int)_curLanguage];
                break;
            case LeftReason.NoReason:
                _reasonSprite.sprite = _noReason[(int)_curLanguage];
                break;
        }
    }
}
