using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyIdButton : MonoBehaviour
{
    private Button _button;
    private string _uid;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);

        _uid = Manager.Fire.Auth.CurrentUser.UserId;
    }
    private void OnClicked()
    {
        GUIUtility.systemCopyBuffer = _uid;
        Debug.Log("복사완료");
    }
}
