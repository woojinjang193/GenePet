using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StringLanguage : MonoBehaviour
{
    [SerializeField] private StringSO _stringSO;
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    private void OnEnable()
    {
        var curLang = Manager.Lang.CurLanguage;

        string localizedText = "";

        switch (curLang)
        {
            case Language.KOR:
                localizedText = _stringSO.KOR;
                break;
            case Language.ENG:
                localizedText = _stringSO.ENG;
                break;
            case Language.JP:
                localizedText = _stringSO.JP;
                break;
            case Language.DE:
                localizedText = _stringSO.DE;
                break;
            case Language.CH:
                localizedText = _stringSO.DE;
                break;
            default:
                localizedText = _stringSO.ENG;
                break;
        }

        _text.text = localizedText;
    }
}
