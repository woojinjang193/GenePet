using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [Header("코인 소지량")]
    [SerializeField] private TMP_Text _moneyAmount;

    private void Awake()
    {
        Manager.Item.OnMoneyChanged += UpdateMoney;
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
        {
            Manager.Item.OnMoneyChanged -= UpdateMoney;
        }
    }
    private void OnEnable()
    {
        _moneyAmount.text = Manager.Save.CurrentData.UserData.Items.Money.ToString(); // 소지금 초기화
    }

    private void UpdateMoney(int value)
    {
        _moneyAmount.text = value.ToString();
    }
}
