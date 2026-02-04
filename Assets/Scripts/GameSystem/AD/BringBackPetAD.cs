using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BringBackPetAD : AdRequestBase
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        base.Request();
    }
    protected override void OnReward()
    {
        Debug.Log("리워드 짜잔");
    }
    protected override void OnClosed()
    {
        Debug.Log("광고 닫힘 짜잔");
    }

}
