using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchArea : MonoBehaviour, IPointerDownHandler//, IPointerUpHandler
{
    [SerializeField] private RythmGameManager _rythmManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        _rythmManager.OnPlayerInput();
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //
    //}
}
