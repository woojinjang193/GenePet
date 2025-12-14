using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TicketAmount : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        Manager.Item.OnRewardGranted += OnRewardGranted;
    }
    private void OnDestroy()
    {
        Manager.Item.OnRewardGranted -= OnRewardGranted;
    }
    private void OnEnable()
    {
        _text.text = Manager.Save.CurrentData.UserData.Items.IslandTicket.ToString();
    }

    private void OnRewardGranted(RewardType type, int newValue)
    {
        if(type == RewardType.IslandTicket)
        {
            _text.text = newValue.ToString();
        }
    }
}
