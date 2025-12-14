using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseWithGoldButton : MonoBehaviour
{
    [SerializeField] private string _productID;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        Manager.Shop.PurchaseWithGold(_productID);
    }
}
