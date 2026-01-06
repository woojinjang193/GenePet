using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private JumpMiniGame _game; // Jump 게임 참조
    [SerializeField] private int _direction;     // -1 = 왼쪽, 1 = 오른쪽

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_game)
        //Debug.Log($"버튼눌림 : {_direction}");
        _game.OnPressButton(_direction); // 누름 시작
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log($"버튼떼짐 : {_direction}");
        _game.OnReleaseButton();         // 떼기
    }
}
