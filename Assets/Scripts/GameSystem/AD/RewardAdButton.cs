using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdButton : AdRequestBase
{
    [Header("보상 ID")]
    [SerializeField] private string _rewardID;

    [Header("데일리")]
    [SerializeField] private bool _isDaily = false;

    [Header("쿨타임(시간) 데일리면 무시됨")]
    [SerializeField] private int _coolTimeHour;

    [Header("tmp")]
    [SerializeField] private TMP_Text _text;

    [Header("랜덤보상 여부")]
    [SerializeField] private bool _isRandom = false;

    [Header("아이템 보상 목록")]
    [SerializeField] private List<ItemRewardEntry> _itemRewards = new();

    [Header("알 보상 목록(프리셋)")]
    [SerializeField] private List<RewardEggPresetSO> _eggPresets = new();

    private List<RewardData> _rewards = new();
    private Button _button;


    [Serializable]
    public class ItemRewardEntry
    {
        public RewardType Type = RewardType.None;
        public int Amount = 0;
    }

    private void Awake() 
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClaimRewards);
    }
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_rewardID)) { _button.interactable = false; return; } //id 없으면 리턴

        _button.interactable = CanClickButton();
    }

    public void ClaimRewards()
    {
        if (Manager.Item == null) return;

        if (RewardTimeRecordService.CanClaimReward(_rewardID)) //보상 수령 가능이면
        {
            _rewards.Clear();

            _rewards = BuildPayoutList(); //보상 리스트 생성
            if (_rewards.Count == 0) return; //보상 없으면 리턴

            base.Request(); //광고 요청
        }
        else //불가능이면
        {
            _button.interactable= false;
        }
            
    }
    //======리워드 리스트 생성=================
    private List<RewardData> BuildPayoutList()
    {
        List<RewardData> payout = new();

        int itemCount = _itemRewards.Count;
        int eggCount = _eggPresets.Count;
        int total = itemCount + eggCount;

        if (total <= 0) return payout;

        if (_isRandom) //랜덤이면
        {
            int pick = UnityEngine.Random.Range(0, total);

            if (pick < itemCount) // 아이템 영역
                TryAddItem(_itemRewards[pick], payout); // 아이템 추가
            else // 알 영역
                TryAddEgg(_eggPresets[pick - itemCount], payout); // 알 추가

            return payout;
        }

        // 전부 지급
        for (int i = 0; i < itemCount; i++) TryAddItem(_itemRewards[i], payout); // 아이템 전부
        for (int i = 0; i < eggCount; i++) TryAddEgg(_eggPresets[i], payout); // 알 전부

        return payout; //전부 지급 리스트
    }

    private void TryAddItem(ItemRewardEntry entry, List<RewardData> payout) // 아이템 리스트에 담기
    {
        if (entry == null) return;
        if (entry.Type == RewardType.None) return;
        if (entry.Amount <= 0) return;

        payout.Add(RewardData.CreateItem(entry.Type, entry.Amount)); //RewardData로 변환
    }

    private void TryAddEgg(RewardEggPresetSO preset, List<RewardData> payout) //알 변환
    {
        if (preset == null) return;
        EggData egg = EggDataGenerator.GenerateRewardEgg(preset); // 알 생성
        if (egg == null) return;

        payout.Add(RewardData.CreateEgg(egg)); // RewardData로 변환
    }
    //==================광고 후처리=============================

    protected override void OnReward()
    {
        Manager.Item.GiveMiniGameRewards(_rewards);// 보상 지급
        RewardTimeRecordService.MarkClaimed(_rewardID);
        _button.interactable = CanClickButton();
    }

    protected override void OnClosed()
    {
        Manager.Item.NotifyRewardsReady();// 팝업 신호
    }

    //===================유틸=========================
    private bool CanClickButton()
    {
        if (_isDaily) //데일리 보상이면
        {
            return RewardTimeRecordService.CanClaimReward(_rewardID);
        }
        else //쿨타임 보상이면
        {
            return RewardTimeRecordService.CanClaimReward(_rewardID, _coolTimeHour);
        }
    }
}
