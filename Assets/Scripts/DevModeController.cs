using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevModeController : MonoBehaviour
{
    [SerializeField] private int[] _passKey;
    [SerializeField] private Button[] _buttons;

    private int _passLength;
    private int _curIndex = 0;
    private List<int> _input = new();
    private void Awake()
    {
        if (_passKey.Length <= 0) return;

        _passLength = _passKey.Length;

        for(int i = 0; i < _buttons.Length; i++)
        {
            int index = i;
            _buttons[i].onClick.AddListener(() => OnClicked(index));
        }
    }
    private void OnClicked(int input)
    {
        int need = _passKey[_curIndex]; //필요 번호

        if (need != input) { _curIndex = 0; return; } //틀리면 바로 초기화

        bool stillNeed = _curIndex < _passLength - 1 ;

        if (stillNeed) { _curIndex++; return; }

        ActiveOffButtons();
        Manager.Game.DeveModeOn();
        Debug.Log("DevModeOn");
    }
    private void ActiveOffButtons()
    {
        for(int i = 0; i< _buttons.Length;i++)
        {
            _buttons[i].gameObject.SetActive(false);
        }
    }
}
