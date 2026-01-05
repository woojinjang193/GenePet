using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class ItemManager : Singleton<ItemManager>
{
    private ItemsSO _ItemsSO;  // 아이템 이미지/데이터 SO
    public ItemsSO ItemImages => _ItemsSO;

    // ================= 이벤트 =================
    public event Action OnRewardsGiven; // 한 묶음 보상 지급 완료 알림, 보상 팝업 열기용
    public event Action<int> OnMoneyChanged; //현재 소지 골드 변경 알림
    public event Action<RewardType, int> OnRewardGranted; //개별 보상 1개 지급 알림
    public event Action OnGiftAmountChanged; //선물 수량 감소 알림
    public event Action<RewardType, int> OnItemConsumed; //아이템 소비 알림

    // ================= 연출용 큐 =================
    private Queue<RewardData> _rewardQueue = new Queue<RewardData>();

    public bool IsReady {  get; private set; }

    // =========================초기화=====================================
    protected override void Awake()
    {
        var handle = Addressables.LoadAssetAsync<ItemsSO>("ItemSO");
        handle.Completed += OnItemSOLoaded;
    }
    private void OnItemSOLoaded(AsyncOperationHandle<ItemsSO> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _ItemsSO = handle.Result;
            Debug.Log("ItemSO 로드 완료");
            IsReady = true;
        }
        else
        {
            Debug.LogError("GameConfig 로드 실패");
        }
    }

    // ===========================골드로 아이템 구매 =========================
    public void PurchaseWithGold(ProductCatalogSO.Entry entry, int price)
    {
        if (entry == null) return;

        var user = Manager.Save.CurrentData.UserData;

        user.Items.Money -= price;                 // 골드 차감
        OnMoneyChanged?.Invoke(user.Items.Money); // UI 알림

        GiveReward(entry); // 보상 지급
    }

    // ======================보상지급================================================
    public void GiveReward(ProductCatalogSO.Entry entry)
    {
        if (entry == null) return;

        for (int i = 0; i < entry.Rewards.Count; i++) // 보상 개수만큼 반복
        {
            var reward = entry.Rewards[i]; // 현재 보상
            ApplyReward(reward.RewardType, reward.RewardAmount);    // 실제 지급
        }

        // 외부(UI, 저장 등)에 알림 (메인씬에서만 보여줌)
        OnRewardsGiven?.Invoke();
    }
    public void GiveMiniGameRewards(List<RewardData> rewards) //미니게임 리워드 지급
    {
        if (rewards == null || rewards.Count == 0) return;

        for (int i = 0; i < rewards.Count; i++)
        {
            RewardData reward = rewards[i]; 
            if (reward == null) continue;

            if (reward.Category == RewardCategory.Egg)
            {
                EnqueueEgg(reward.Egg);
                continue;
            }

            // 실제 지급(세이브 값 변경 + 큐 적재 + 이벤트 발사)
            ApplyReward(reward.RewardType, reward.Amount);
        }

        OnRewardsGiven?.Invoke();
    }

    // ==================실제 보상 적용 함수==========================
    private void ApplyReward(RewardType type, int amount)
    {
        var user = Manager.Save.CurrentData.UserData;
        int newValue = 0;

        switch (type)
        {
            case RewardType.RemovedAD:
                // 광고 제거 영구 플래그 저장
                user.Items.IsAdRemoved = true;
                Debug.Log("광고 제거 적용");
                break;

            case RewardType.Energy:
                newValue = user.Energy += amount;
                Debug.Log($"에너지 +{amount}");
                break;

            case RewardType.Coin:
                newValue = user.Items.Money += amount;
                OnMoneyChanged?.Invoke(user.Items.Money); //소지금 변경 이벤트
                Debug.Log($"코인 +{amount}");
                break;

            case RewardType.Snack:
                newValue = user.Items.Snack += amount;
                Debug.Log($"스낵 +{amount}");
                break;

            case RewardType.IslandTicket:
                newValue = user.Items.IslandTicket += amount;
                Debug.Log($"티켓 +{amount}");
                break;

            case RewardType.MissingPoster:
                newValue = user.Items.MissingPoster += amount;
                Debug.Log($"포스터 +{amount}");
                break;

            case RewardType.GeneticScissors:
                newValue = user.Items.GeneticScissors += amount;
                Debug.Log($"유전자가위 +{amount}");
                break;

            case RewardType.GeneticTester:
                newValue = user.Items.geneticTester += amount;
                Debug.Log($"유전자 테스터 +{amount}");
                break;

            case RewardType.MasterGift:
                newValue = user.Items.MasterGift += amount;
                Debug.Log($"만능 선물 +{amount}");
                break;

            case RewardType.Gift1:
                newValue = user.Items.Gift1 += amount;
                Debug.Log($"선물1 +{amount}");
                break;

            case RewardType.Gift2:
                newValue = user.Items.Gift2 += amount;
                Debug.Log($"선물2 +{amount}");
                break;

            case RewardType.Gift3:
                newValue = user.Items.Gift3 += amount;
                Debug.Log($"선물3 +{amount}");
                break;

            case RewardType.Gift4:
                newValue = user.Items.Gift4 += amount;
                Debug.Log($"선물4 +{amount}");
                break;

            case RewardType.PetSlot:
                newValue = Mathf.Clamp(user.PetSlot += amount, 0, Manager.Game.Config.MaxPetAmount); //초과 방어
                Debug.Log($"펫 슬롯 +{amount}");
                break;
        }
        //큐에 추가 
        _rewardQueue.Enqueue(RewardData.CreateItem(type, amount));

        //유아이 업데이트용
        OnRewardGranted?.Invoke(type, newValue); // 바로 업데이트 해야하는 유아이 있으면 구독하면 됨
    }

    // ========================보상 큐==============================
    public bool HasReward()
    {
        return _rewardQueue.Count > 0;
    }
    public bool TryDequeueReward(out RewardData reward)
    {
        if (_rewardQueue.Count == 0)
        {
            reward = null;
            return false;
        }

        reward = _rewardQueue.Dequeue();
        return true;
    }
    public void EnqueueEgg(EggData egg) //알 보상 큐 적재 전용
    {
        Debug.Log($"알 큐에 들어옴 {egg}");
        _rewardQueue.Enqueue(RewardData.CreateEgg(egg));
    }
    public void ClearRewardQueue() //보상 다 보여준 뒤 정리 (필요없으면 삭제)
    {
        _rewardQueue.Clear();
    }

    // ========================아이템 사용==============================
    public void UseGift(Gift gift)
    {
        var item = Manager.Save.CurrentData.UserData.Items;

        switch (gift)
        {
            case Gift.Gift1: if (item.Gift1 <= 0) { return; }; item.Gift1--; break;
            case Gift.Gift2: if (item.Gift2 <= 0) { return; }; item.Gift2--; break;
            case Gift.Gift3: if (item.Gift3 <= 0) { return; }; item.Gift3--; break;
            case Gift.Gift4: if (item.Gift4 <= 0) { return; }; item.Gift4--; break;
            case Gift.MasterGift: if (item.MasterGift <= 0) { return; }; item.MasterGift--; break;
        }
        OnGiftAmountChanged?.Invoke();
    }

    public void AddOrSubtractMoney(int amount) //돈 액수만 빠르게 변화시킬때
    {
        var user = Manager.Save.CurrentData.UserData;

        user.Items.Money += amount;

        OnMoneyChanged?.Invoke(user.Items.Money); // UI 알림
    }

    public void UseItem(RewardType type, int amount)
    {
        var items = Manager.Save.CurrentData.UserData.Items;
        int newValue;

        switch (type)
        {
            case RewardType.Snack: 
                if (amount <= 0) 
                {
                    break; 
                }
                else
                {
                    newValue = items.Snack -= amount;
                }
                OnItemConsumed?.Invoke(type, newValue);
                break;

            case RewardType.GeneticScissors:
                if (amount <= 0)
                {
                    break;
                }
                else
                {
                    newValue = items.GeneticScissors -= amount;
                }
                OnItemConsumed?.Invoke(type, newValue);
                break;
        }
    }
}
