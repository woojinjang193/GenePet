using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleLinkButton : MonoBehaviour
{
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LinkToGoogle);
    }
    private void LinkToGoogle()
    {
        Manager.Fire.LinkGoogleButton();
    }
}
