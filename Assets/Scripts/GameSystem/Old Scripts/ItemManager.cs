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
    public event Action<RewardType, int> OnRewardGranted; //개별 보상 1개 지급 알림 <type, newvalue>
    public event Action OnGiftAmountChanged; //선물 수량 감소 알림
    public event Action<RewardType, int> OnItemConsumed; //아이템 소비 알림 <type, newvalue>

    // ================= 연출용 큐 =================
    private Queue<RewardData> _rewardQueue = new Queue<RewardData>();

    public bool IsReady {  get; private set; }

    // =========================초기화=====================================
    protected override void Awake()
    {
        base.Awake();
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
    // ===================== 팝업 오픈 트리거 =====================
    public void NotifyRewardsReady() //원하는 타이밍에 보상 팝업을 열라고 이벤트를 발생시키는 함수
    {
        OnRewardsGiven?.Invoke(); // 보상팝업 열기 신호 이벤트 발생
    }

    // ===========================골드로 아이템 구매 =========================
    public void PurchaseWithGold(ProductCatalogSO.Entry entry, int price, UserData user)
    {
        if (entry == null) return;

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
            ApplyReward(reward.RewardType, reward.RewardAmount, true);    // 실제 지급
        }

        // 외부(UI, 저장 등)에 알림 (메인씬에서만 보여줌)
        OnRewardsGiven?.Invoke();
    }
    //==================미니게임 보상=====================
    public void GiveMiniGameRewards(List<RewardData> rewards) // : 지급 + 표시용 큐 적재, 팝업 오픈은 NotifyRewardsReady로 외부에서 결정
    {
        if (rewards == null || rewards.Count == 0) return;

        var user = Manager.Save.CurrentData.UserData; //알 지급에 필요

        for (int i = 0; i < rewards.Count; i++)
        {
            RewardData reward = rewards[i];
            if (reward == null) continue;

            if (reward.Category == RewardCategory.Egg) //알은 여기서 직접 지급
            {
                if (user.EggList == null) user.EggList = new List<EggData>(); //null 방어
                if (reward.Egg != null) user.EggList.Add(reward.Egg); //알 지급(세이브 반영)

                if (reward.Egg != null) EnqueueEgg(reward.Egg); // 알도 팝업 표시를 위해 큐에 적재
                continue;
            }

            if (reward.RewardType == RewardType.None) continue; // None 보상 스킵

            ApplyReward(reward.RewardType, reward.Amount, true); // 미니게임 보상도 표시 큐에 적재
        }

        Manager.Save.SaveGame(); //매판 즉시 저장
    }

    // ==================실제 보상 적용 함수==========================
    private void ApplyReward(RewardType type, int amount, bool enqueuePopup) //enqueuePopup = 팝업 큐 적재 여부 옵션
    {
        var user = Manager.Save.CurrentData.UserData;
        if (user.Items == null) user.Items = new UserItemData(); // Items null 방어
        int newValue = 0;
        bool granted = true; // 실제로 지급됐는지 여부(지급 안되면 큐/이벤트 막기)

        switch (type)
        {
            case RewardType.RemovedAD:
                user.Items.IsAdRemoved = true;
                Debug.Log("광고 제거 적용");
                break;

            case RewardType.Energy:
                newValue = user.Energy += amount;
                Debug.Log($"에너지 +{amount}");
                break;

            case RewardType.Coin:
                newValue = user.Items.Money += amount;
                OnMoneyChanged?.Invoke(user.Items.Money);
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

            case RewardType.PetSlot:
                newValue = Mathf.Clamp(user.PetSlot += amount, 0, Manager.Game.Config.MaxPetAmount);
                Debug.Log($"펫 슬롯 +{amount}");
                break;

            case RewardType.GrowthBooster:
                newValue = user.Items.GrowthBooster += amount;
                Debug.Log($"성장 부스터 +{amount}");
                break;

            case RewardType.GeneticGlue:
                newValue = user.Items.GeneticGlue += amount;
                Debug.Log($"유전자 풀 +{amount}");
                break;

            case RewardType.Gem:
                newValue = user.Items.Gem += amount;
                Debug.Log($"잼 +{amount}");
                break;

            case RewardType.Room_Jump:
            case RewardType.Room_Rythm:
            case RewardType.Room_Pinball:
            case RewardType.Room_Poor:
            //case RewardType.Room_Cozy:
            case RewardType.Room_Something:
                {
                    if (user.Items.Rooms == null) user.Items.Rooms = new List<Room>();

                    // 매핑 실패면 지급 실패 처리
                    if (!MiniGameRewardPicker.TryGetRoomFromRewardType(type, out var room)) { granted = false; break; }

                    if (room == Room.Default) { granted = false; break; } // 실수로 Default 룸 설정했을경우 방어

                    if (user.Items.Rooms.Contains(room)) { granted = false; break; } //중복이면 지급 실패 처리

                    user.Items.Rooms.Add(room);
                    newValue = user.Items.Rooms.Count;
                    Debug.Log($"방 획득: {room}");
                    break;
                }
            default:
                granted = false; // 처리 안 된 RewardType은 지급 실패로 간주
                Debug.LogWarning($"처리되지 않은 보상 타입: {type}");
                break;
        }
        if (!granted) return; //지급 실패면 큐/이벤트/팝업 모두 스킵

        if (!enqueuePopup) return; //지급만 모드면 큐/이벤트 스킵

        _rewardQueue.Enqueue(RewardData.CreateItem(type, amount));
        OnRewardGranted?.Invoke(type, newValue);
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

            case RewardType.GeneticGlue:
                if (amount <= 0)
                {
                    break;
                }
                else
                {
                    newValue = items.GeneticGlue -= amount;
                }
                OnItemConsumed?.Invoke(type, newValue);
                break;

            case RewardType.GrowthBooster:
                if (amount <= 0)
                {
                    break;
                }
                else
                {
                    newValue = items.GrowthBooster -= amount;
                }
                OnItemConsumed?.Invoke(type, newValue);
                break;

            case RewardType.Gem:
                if (amount <= 0)
                {
                    break;
                }
                else
                {
                    newValue = items.Gem -= amount;
                }
                OnItemConsumed?.Invoke(type, newValue);
                break;
        }
    }
}
