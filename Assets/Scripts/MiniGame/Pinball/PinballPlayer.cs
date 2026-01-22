using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinballPlayer : MonoBehaviour
{
    public event Action<RewardType, int> OnRewardGet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            if (collision.TryGetComponent<ItemForMiniGame>(out var item))
            {
                OnRewardGet?.Invoke(item.Reward, item.Amount); // 아이템 획득 이벤트 발생
            }

            collision.gameObject.SetActive(false);         // 아이템 비활성화
        }
    }
}
