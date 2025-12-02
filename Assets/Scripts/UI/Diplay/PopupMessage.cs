using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;

    public void ShowMessage(string msg)
    {
        _background.gameObject.SetActive(false);
        _background.gameObject.SetActive(true);
        _text.text = msg;
    }
}
