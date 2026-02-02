using TMPro;
using UnityEngine;

public class TextLoaderForInputField : MonoBehaviour
{
    private TMP_InputField _text;
    private void Awake()
    {
        _text = GetComponent<TMP_InputField>();

        Manager.Lang.OnLanguageChanged += UpdateFont;
    }
    private void OnEnable()
    {
        ApplyTextAndFont();
    }

    private void OnDestroy()
    {
        if (Manager.Lang != null)
            Manager.Lang.OnLanguageChanged -= UpdateFont;
    }

    private void UpdateFont(TMP_FontAsset font) //언어 변경 시 폰트도 같이 적용
    {
        ApplyTextAndFont();
    }
    //================텍스트 + 폰트 동시 적용===================
    private void ApplyTextAndFont()
    {
        _text.textComponent.font = Manager.Lang.CurFont;
    }
}
