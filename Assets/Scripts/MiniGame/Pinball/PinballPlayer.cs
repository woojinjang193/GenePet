using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinballPlayer : MonoBehaviour
{
    private bool _hasGotReward = false;
    public event Action<RewardType, int> OnRewardGet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            if (_hasGotReward) return; //이미 보상 받았으면 리턴

            if (collision.TryGetComponent<ItemForMiniGame>(out var item))
            {
                OnRewardGet?.Invoke(item.Reward, item.Amount); // 아이템 획득 이벤트 발생
                _hasGotReward =true;
            }

            //collision.gameObject.SetActive(false);         // 아이템 비활성화
        }
    }
    public void FlagReset()
    {
        _hasGotReward = false;
    }
}
