using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ShopUiManager : MonoBehaviour
{
    [Header("잼 소지량 텍스트")]
    [SerializeField] private TMP_Text _gemAmount;
    [Header("코인 소지량 텍스트")]
    [SerializeField] private TMP_Text _moneyAmount;
    private void Awake()
    {
        Manager.Item.OnItemConsumed += UpdateAmounts;
        Manager.Item.OnRewardGranted += UpdateAmounts;
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
        {
            Manager.Item.OnItemConsumed -= UpdateAmounts;
            Manager.Item.OnRewardGranted -= UpdateAmounts;
        }
    }
    private void OnEnable()
    {
        _moneyAmount.text = Manager.Save.CurrentData.UserData.Items.Money.ToString(); // 잼 개수 초기화
        _gemAmount.text = Manager.Save.CurrentData.UserData.Items.Gem.ToString(); // 소지금 초기화
    }

    private void UpdateAmounts(RewardType type, int newValue)
    {
        if (type == RewardType.Gem)
        {
            _gemAmount.text = newValue.ToString();
        }
        else if (type == RewardType.Coin)
        {
            _moneyAmount.text = newValue.ToString();
        }
    }
}
