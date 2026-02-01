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
    private int _requestNum = -1;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(OnClickedConfirm);
        _cancelButton.onClick.AddListener(OnClickedCancel);

        if (Manager.Lang == null) { Debug.LogError("LangManager 없음"); return; }

        Manager.Lang.OnLanguageChanged += OnLanguageChanged;
    }
    private void OnDestroy()
    {
        if (Manager.Lang != null)
            Manager.Lang.OnLanguageChanged -= OnLanguageChanged;
    }
    private void OnLanguageChanged(TMP_FontAsset font) //언어변경
    {
        _text.font = font;
    }
    //============================UI 컨트롤===============================
    public void OpenConfirmUI(string textID, int requestNum, IConfirmRequester requster)
    {
        _panel.SetActive(true);
        _requester = requster; // 리퀘스터
        _requestNum = requestNum; //요청 번호
        _text.text = Manager.Lang.GetText(textID);
    }
    private void OnClickedConfirm()
    {
        _requester.Confirmed(_requestNum);
        _requester = null;
        _requestNum = -1;
        _panel.SetActive(false);
    }
    private void OnClickedCancel()
    {
        _requester.Canceled(_requestNum);
        _requester = null;
        _requestNum = -1;
        _panel.SetActive(false);
    }

}
