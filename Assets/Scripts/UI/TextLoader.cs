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

        Manager.Lang._OnLanguageChanged += UpdateText;
    }
    private void OnEnable()
    {
        _text.text = Manager.Lang.GetText(_textID);
    }

    private void OnDestroy()
    {
        if(Manager.Lang != null)
        Manager.Lang._OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        _text.text = Manager.Lang.GetText(_textID);
    }
}
