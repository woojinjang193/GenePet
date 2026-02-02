using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TMP_Text _description;

    private void Awake()
    {
        _geneGuarantee.onClick.AddListener(GuaranteeClicked);
        _geneScissors.onClick.AddListener(GeneScissorsClicked);
        _geneGlue.onClick.AddListener(GeneGlueClicked);
        _closeButton.onClick.AddListener(Close);
    }
    private void GuaranteeClicked()
    {
        if (!_desPanel.activeSelf) _desPanel.SetActive(true);

        _description.text = Manager.Lang.GetText("Item_GeneGuarantor");
    }
    private void GeneScissorsClicked()
    {
        if (!_desPanel.activeSelf) _desPanel.SetActive(true);

        _description.text = Manager.Lang.GetText("Item_GeneScissors");
    }
    private void GeneGlueClicked()
    {
        if (!_desPanel.activeSelf) _desPanel.SetActive(true);

        _description.text = Manager.Lang.GetText("Item_GeneGlue");
    }
    private void Close()
    {
        _desPanel.SetActive(false);
    }
}
