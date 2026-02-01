using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        if (Manager.Lang == null) { Debug.LogError("LangManager 없음"); return; }

        Manager.Lang.OnLanguageChanged += OnLanguageChanged;
    }
    private void OnDestroy()
    {
        if (Manager.Lang != null)
            Manager.Lang.OnLanguageChanged -= OnLanguageChanged;
    }
    private void OnLanguageChanged(TMP_FontAsset font)
    {
        _text.font = font;
    }
    public void ShowMessage(string msg)
    {
        _background.gameObject.SetActive(false);
        _background.gameObject.SetActive(true);
        _text.text = msg;
    }
}
