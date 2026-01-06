using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemForMiniGame : MonoBehaviour
{
    [Header("아이템 타입")]
    [SerializeField] private RewardType _reward;
    [SerializeField] private int _amount;

    public RewardType Reward => _reward;
    public int Amount => _amount;
    
}
