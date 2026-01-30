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

    private Button _button;

    private ProductType _type;

    private void Awake()
    {
        _priceText.text = _price.ToString();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        bool canbuy = Manager.Shop.TryPurchaseWithGold(_productID, _price, out _type);

        if (!canbuy) return;

        switch (_type)
        {
            case ProductType.Unknown: break;
            case ProductType.Consumable: break;
            case ProductType.NonConsumable: _button.interactable = false; break;
            case ProductType.Subscription: break;
        }
    }
}
