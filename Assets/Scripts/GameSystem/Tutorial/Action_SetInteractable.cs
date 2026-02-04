using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 여러 Button의 interactable을 한 번에 켜거나 끄는 액션
public class Action_SetInteractable : TutorialActionBase
{
    [Header("Targets")]
    [SerializeField] private List<Button> _buttons = new(); // 제어할 버튼 목록

    [Header("Value")]
    [SerializeField] private bool _interactable = false; //설정할 값

    public override void Execute()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            var btn = _buttons[i];
            if (btn == null) continue;
            btn.interactable = _interactable;
        }
    }
}
