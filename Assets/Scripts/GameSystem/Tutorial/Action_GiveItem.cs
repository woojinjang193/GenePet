using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GiveItem : TutorialActionBase
{
    [Header("아이템 타입")]
    [SerializeField] private RewardType _rewardType;
    [Header("개수")]
    [SerializeField] private int _amount;
    public override void Execute()
    {
        List<RewardData> reward = new();

        reward.Add(RewardData.CreateItem(_rewardType, _amount));

        Manager.Item.GiveMiniGameRewards(reward);
        Manager.Item.NotifyRewardsReady();
    }
}
