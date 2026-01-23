using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PinballHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private FlipperMoveRotation _targetLever;
    private void OnDisable()
    {
        if (_targetLever != null) _targetLever.SetPressed(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_targetLever != null) _targetLever.SetPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_targetLever != null) _targetLever.SetPressed(false);
    }

    // 버튼 밖으로 손가락/마우스가 나가도 레버가 계속 눌린 채가 되는 것 방지
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_targetLever != null) _targetLever.SetPressed(false);
    }
}
