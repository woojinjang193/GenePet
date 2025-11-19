using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterWriter : MonoBehaviour
{
    [SerializeField] private Image _handwritingImage;

    [Header("순서: Hunger, Unhappy, Dirty, LayEgg,")]

    [SerializeField] private Sprite[] _korLetters;
    [SerializeField] private Sprite[] _engLetters;
    [SerializeField] private Sprite[] _jpLetters;
    [SerializeField] private Sprite[] _deLetters;


    //private void OnEnable()
    //{
    //    Language lang = Manager.Lang.CurLanguage;
    //
    //    LeftReason reason = Manager.Game.Reason;
    //
    //    switch (lang)
    //    {
    //        case Language.KOR:
    //            ChangeSprite(_korLetters, reason);
    //            break;
    //
    //        case Language.ENG:
    //            ChangeSprite(_engLetters, reason);
    //            break;
    //
    //        case Language.JP:
    //            ChangeSprite(_jpLetters, reason);
    //            break;
    //
    //        case Language.DE:
    //            ChangeSprite(_deLetters, reason);
    //            break;
    //
    //
    //    }
    //}
    //
    //private void ChangeSprite(Sprite[] letters, LeftReason reason)
    //{
    //    switch(reason)
    //    {
    //        case LeftReason.Hunger:
    //            _handwritingImage.sprite = letters[0];
    //            break;
    //        case LeftReason.Unhappy:
    //            _handwritingImage.sprite = letters[0];
    //            break;
    //        case LeftReason.Dirty:
    //            _handwritingImage.sprite = letters[0];
    //            break;
    //        case LeftReason.LayEgg:
    //            _handwritingImage.sprite = letters[0];
    //            break;
    //    }
    //}
        
}
