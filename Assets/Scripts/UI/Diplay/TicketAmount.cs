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
    }

    private void OnEnable()
    {
        _text.text = Manager.Save.CurrentData.UserData.Items.IslandTicket.ToString();
    }
}
