using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdButton : MonoBehaviour
{
    private Button _button;

    [Header("옵션")]
    [SerializeField] private bool _isRandom = false; 

    [Header("아이템 보상 목록")]
    [SerializeField] private List<ItemRewardEntry> _itemRewards = new();

    [Header("알 보상 목록(프리셋)")]
    [SerializeField] private List<RewardEggPresetSO> _eggPresets = new();

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

    public void ClaimRewards()
    {
        if (Manager.Item == null) return;

        List<RewardData> payout = BuildPayoutList();
        if (payout.Count == 0) return;

        Manager.AD.ShowRewardedAd(payout);
    }

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
}
