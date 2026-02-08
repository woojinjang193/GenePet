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
        Debug.Log("[획득] 리워드 팝업 이벤트 수신");
        if (_rewardPopUp == null) _rewardPopUp = FindObjectOfType<RewardPopUp>();
       
        if (_rewardPopUp == null) return;
        Debug.Log("[획득] 팝업 있는거 확인 ");
        if (!Manager.Item.HasReward()) return; // 큐 비었으면 팝업 안 열기
        Debug.Log("[획득] 큐 안빔 ");
        _rewardPopUp.StartShowingReward();
        Debug.Log("[획득] 팝업 오픈요청 보냄 ");
    }
}
