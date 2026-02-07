using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestoreButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(TryRestore);
    }
    private void TryRestore()
    {
        bool isAdRemovePurchased = Manager.Shop.CheckNonConsumableOwned("testremoveadbanner");
        
        ShowResultPopUp(isAdRemovePurchased);
    }
    private void ShowResultPopUp(bool isPurchased)
    {
        if (isPurchased)
        {
            Manager.Game.ShowPopup("PopUp_RestoreComplete");
        }
        else
        {
            Manager.Game.ShowPopup("PopUp_NothingToRestore");
        }
    }
}
