using System;
using System.Collections.Generic;
using UnityEngine;
public class ItemManager : Singleton<ItemManager>
{
    // 보상 지급 완료 이벤트
    public event Action OnRewardGranted;

    public event Action<int> OnMoneyChanged;

    private Queue<object> _rewardQueue = new Queue<object>();
    public Queue<object> RewardQueue => _rewardQueue;
    public void GiveReward(ProductCatalogSO.Entry entry)
    {
        if (entry == null) return;

        for (int i = 0; i < entry.Rewards.Count; i++) // 보상 개수만큼 반복
        {
            var reward = entry.Rewards[i]; // 현재 보상
            ApplyReward(reward.RewardType, reward.RewardAmount);    // 실제 지급
        }
    }
    public void PurchaseWithGold(ProductCatalogSO.Entry entry, int price)
    {
        var user = Manager.Save.CurrentData.UserData;

        user.Items.Money -= price;                 // 골드 차감
        OnMoneyChanged?.Invoke(user.Items.Money); // UI 알림

        GiveReward(entry); // 보상 지급
    }


    // 실제 보상 적용 함수
    private void ApplyReward(RewardType type, int amount)
    {
        var user = Manager.Save.CurrentData.UserData;

        switch (type)
        {
            case RewardType.RemovedAD:
                // 광고 제거 영구 플래그 저장
                user.Items.IsAdRemoved = true;
                Debug.Log("광고 제거 적용");
                break;

            case RewardType.Energy:
                user.Energy += amount;

                var uiManager = FindObjectOfType<InGameUIManager>();    
                if(uiManager != null)
                {
                    uiManager.UpdateEnergyBar(user.Energy); // UI 갱신
                }
                Debug.Log($"에너지 +{amount}");
                break;

            case RewardType.Coin:
                user.Items.Money += amount;
                OnMoneyChanged?.Invoke(user.Items.Money); //소지금 변경 이벤트
                Debug.Log($"코인 +{amount}");
                break;

            case RewardType.Snack:
                // 보상 여기
                user.Items.Snack += amount;
                Debug.Log($"스낵 +{amount}");
                break;

            case RewardType.MissingPoster:
                // 보상 여기
                user.Items.MissingPoster += amount;
                Debug.Log($"MissingPoster +{amount}");
                break;

            case RewardType.GeneticScissors:
                // 보상 여기
                user.Items.GeneticScissors += amount;
                Debug.Log($"GeneticScissors +{amount}");
                break;

            case RewardType.GeneticTester:
                // 보상 여기
                user.Items.geneticTester += amount;
                Debug.Log($"GeneticScissors +{amount}");
                break;
        }
        //큐에 추가 (메인씬에서 보여줄예정)
        _rewardQueue.Enqueue((type, amount));

        // 외부(UI, 저장 등)에 알림
        OnRewardGranted?.Invoke();
    }
    public void EnqueueEgg(EggData egg) //알 보상 큐 적재 전용
    {
        Debug.Log($"알 큐에 들어옴 {egg}");
        _rewardQueue.Enqueue(egg);
    }
    public void ClearRewardQueue() //보상 다 보여준 뒤 정리
    {
        _rewardQueue.Clear();
    }
}
