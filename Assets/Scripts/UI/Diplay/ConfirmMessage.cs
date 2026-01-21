using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmMessage : MonoBehaviour
{
    [Header("판넬")]
    [SerializeField] private GameObject _panel;

    [Header("확인/취소 버튼")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    [Header("텍스트")]
    [SerializeField] private TMP_Text _text;

    private IConfirmRequester _requester;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(OnClickedConfirm);
        _cancelButton.onClick.AddListener(OnClickedCancel);
    }
    public void OpenConfirmUI(string textID, IConfirmRequester requster)
    {
        _panel.SetActive(true);
        _requester = requster;

        _text.text = Manager.Lang.GetText(textID);
    }
    private void OnClickedConfirm()
    {
        _requester.Confirmed();
        _panel.SetActive(false);
    }
    private void OnClickedCancel()
    {
        _requester.Canceled();
        _requester = null;
        _panel.SetActive(false);
    }
    
}
