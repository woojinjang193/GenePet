using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinballRouletteZone : MonoBehaviour
{
    private bool isSent = false;
    public event Action OnRouletteStart;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(!isSent) //한게임에 한번만 발생하도록 처리
            {
                isSent = true;
                OnRouletteStart?.Invoke();
            }
        }
    }
    public void ResetFlag()
    {
        isSent = false;
    }
}
