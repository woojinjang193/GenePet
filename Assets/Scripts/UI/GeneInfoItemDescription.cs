using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneInfoItemDescription : MonoBehaviour
{
    [Header("설명 판넬")]
    [SerializeField] private GameObject _desPanel;

    [Header("아이템")]
    [SerializeField] private Button _geneGuarantee;
    [SerializeField] private Button _geneScissors;
    [SerializeField] private Button _geneGlue;

    [Header("닫기 버튼")]
    [SerializeField] private Button _closeButton;

    [Header("텍스트")]
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _description;

    private Button _lastButton;
    private void Awake()
    {
        _geneGuarantee.onClick.AddListener(GuaranteeClicked);
        _geneScissors.onClick.AddListener(GeneScissorsClicked);
        _geneGlue.onClick.AddListener(GeneGlueClicked);
        _closeButton.onClick.AddListener(Close);
    }
    private void GuaranteeClicked()
    {
        DesPanelActive(_geneGuarantee);

        _itemName.text = Manager.Lang.GetText("Item_GeneGuarantor");
        _description.text = Manager.Lang.GetText("ItemDes_GeneGuarantor");
    }
    private void GeneScissorsClicked()
    {
        DesPanelActive(_geneScissors);

        _itemName.text = Manager.Lang.GetText("Item_GeneGuarantor");
        _description.text = Manager.Lang.GetText("ItemDes_GeneScissors");
    }
    private void GeneGlueClicked()
    {
        DesPanelActive(_geneGlue);

        _itemName.text = Manager.Lang.GetText("Item_GeneGuarantor");
        _description.text = Manager.Lang.GetText("ItemDes_GeneGlue");
    }
    private void Close()
    {
        _desPanel.SetActive(false);
    }

    private void DesPanelActive(Button button) //같은버튼 두번 클릭시 판넬끄고, 아니라면 판넬 켜주기
    {
        if(_lastButton ==  button)
        {
            _desPanel.SetActive(false);
            _lastButton = null;
            return;
        }

        if (!_desPanel.activeSelf) _desPanel.SetActive(true);
        _lastButton = button;
    }
}
