using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class PetSlotBuyButton : MonoBehaviour
{
    private string _productID = "petslot";

    [Header("슬롯 가격 테이블")]
    [SerializeField] private int[] _prices;

    [Header("UI")]
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private TMP_Text _haveAmountText;

    private int _maxSlot;
    private int _curPrice;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);

        Manager.Item.OnRewardGranted += UpdateUI;  // 슬롯 증가 이벤트 구독
    }
    private void OnEnable()
    {
        RefreshUI();
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
        {
            Manager.Item.OnRewardGranted -= UpdateUI;
        }
    }
    private void OnClicked()
    {
        Manager.Shop.PurchaseWithGold(_productID, _curPrice);
    }
    private void UpdateUI(RewardType type, int newValue)
    {
        if (type != RewardType.PetSlot) return;

        RefreshUI();
    }
    private void RefreshUI()
    {
        _maxSlot = Manager.Game.Config.MaxPetAmount;       // 최대 슬롯 수
        int curSlotNum = Manager.Save.CurrentData.UserData.PetSlot; // 현재 슬롯 수

        if (curSlotNum >= _maxSlot)  // 슬롯이 꽉 찼으면
        {
            _haveAmountText.text = "Full";
            _button.interactable = false;  // 버튼 비활성화
            _priceText.text = "-";  // 가격 비움
            return;
        }

        _haveAmountText.text = $"{curSlotNum} / {_maxSlot}"; // 슬롯 표시
        _curPrice = GetPrice(curSlotNum);                  // 현재 가격 계산
        _priceText.text = _curPrice.ToString();            // 가격 출력
        _button.interactable = true;                       // 버튼 활성화
    }

    private int GetPrice(int curSlotNum)
    {
        if (_prices == null || _prices.Length == 0)
        {
            Debug.LogError("슬롯 가격 테이블 없음");
            _button.interactable = false;
            return 99999;
        }

        if (curSlotNum < 0 || curSlotNum >= _prices.Length) // 범위 초과 방어
        {
            Debug.LogWarning("슬롯 가격 범위 초과");
            _button.interactable = false;
            return 99999;
        }

        return _prices[curSlotNum];    // 현재 슬롯 수 기준 가격 반환
    }
}
