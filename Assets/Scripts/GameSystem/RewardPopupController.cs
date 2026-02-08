using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPopupController : MonoBehaviour
{
    [SerializeField] private RewardPopUp _rewardPopUp;

    private void Awake()
    {
        if (Manager.Item != null)
            Manager.Item.OnRewardsGiven += OpenRewardPopUp;
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
            Manager.Item.OnRewardsGiven -= OpenRewardPopUp;
    }
    private void OpenRewardPopUp()
    {
        if (_rewardPopUp == null) _rewardPopUp = FindObjectOfType<RewardPopUp>();
       
        if (_rewardPopUp == null) return;
        if (!Manager.Item.HasReward()) return; // 큐 비었으면 팝업 안 열기
        _rewardPopUp.StartShowingReward();
    }
}
