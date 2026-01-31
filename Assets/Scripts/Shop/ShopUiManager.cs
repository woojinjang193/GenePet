using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUiManager : MonoBehaviour
{
    [Header("잼 소지량 텍스트")]
    [SerializeField] private TMP_Text _gemAmount;
    [Header("코인 소지량 텍스트")]
    [SerializeField] private TMP_Text _moneyAmount;
    private void Awake()
    {
        Manager.Item.OnMoneyChanged += UpdateMoney;
        Manager.Item.OnItemConsumed += UpdateGemAmount;
        Manager.Item.OnRewardGranted += UpdateGemAmount;
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
        {
            Manager.Item.OnMoneyChanged -= UpdateMoney;
            Manager.Item.OnItemConsumed -= UpdateGemAmount;
            Manager.Item.OnRewardGranted -= UpdateGemAmount;
        }
    }
    private void OnEnable()
    {
        _moneyAmount.text = Manager.Save.CurrentData.UserData.Items.Money.ToString(); // 잼 개수 초기화
        _gemAmount.text = Manager.Save.CurrentData.UserData.Items.Gem.ToString(); // 소지금 초기화
    }

    private void UpdateGemAmount(RewardType type, int newValue)
    {
        if (type != RewardType.Gem) return;

        _gemAmount.text = newValue.ToString();
    }
    private void UpdateMoney(int value)
    {
        _moneyAmount.text = value.ToString();
    }
}
