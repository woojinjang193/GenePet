using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseWithGoldButton : MonoBehaviour
{
    [SerializeField] private string _productID;
    [SerializeField] private int _price;
    [SerializeField] private TMP_Text _priceText;

    private Button _button;

    private void Awake()
    {
        _priceText.text = _price.ToString();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        Manager.Shop.PurchaseWithGold(_productID, _price);
    }
}
