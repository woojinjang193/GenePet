using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PurchaseWithGoldButton : MonoBehaviour
{
    [SerializeField] private string _productID;
    [SerializeField] private int _price;
    [SerializeField] private TMP_Text _priceText;

    [SerializeField] private Color _nonInteractableC;
    [SerializeField] private Image _buttonCover;

    private Button _button;
    private List<string> _ncList;
    private ProductType _type;

    private void Awake()
    {
        _priceText.text = _price.ToString();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);

        if (Manager.Save == null) { _button.interactable = false; return; }

        _ncList = Manager.Save.CurrentData.UserData.Items.PurchasedGoldNCs;

        if (_ncList == null)
        {
            _ncList = Manager.Save.CurrentData.UserData.Items.PurchasedGoldNCs = new List<string>(); // 리스트 생성 후 다시 저장
        }

        CheckNonConsumable();
    }
    private void OnClicked()
    {
        bool canbuy = Manager.Shop.TryPurchaseWithGold(_productID, _price, out _type);

        if (!canbuy) return;

        if(_type == ProductType.NonConsumable)
        {
            SaveAndOffButton();
        }
    }
    private void CheckNonConsumable()
    {
        if(_ncList.Contains(_productID))
        {
            _priceText.text = "Sold";
            _button.interactable = false;
            _buttonCover.color = _nonInteractableC;
        }
    }
    private void SaveAndOffButton()
    {
        if (!_ncList.Contains(_productID))
        {
            _ncList.Add(_productID);
            Manager.Save.SaveGame();
        }
        _priceText.text = "Sold";
        _button.interactable = false;
        _buttonCover.color = _nonInteractableC;
    }
}
