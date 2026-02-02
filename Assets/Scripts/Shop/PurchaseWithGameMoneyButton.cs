using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PurchaseWithGameMoneyButton : MonoBehaviour
{
    [Header("프로덕트 ID")]
    [SerializeField] private string _productID;
    [Header("결제 화폐")]
    [SerializeField] private GMPurchaseType _gmPurchaseType;
    [Header("가격")]
    [SerializeField] private int _price;
    [Header("가격 텍스트")]
    [SerializeField] private TMP_Text _priceText;
 
    [Header("버튼 비활성화 컬러")]
    [SerializeField] private Color _nonInteractableC;
    [Header("버튼 커버 이미지")]
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
        if (_gmPurchaseType == GMPurchaseType.None) return; //결제타입 설정 안하면 return

        bool canbuy = Manager.Shop.TryPurchaseWith(_productID, _price , _gmPurchaseType, out _type); //돈 확인

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
