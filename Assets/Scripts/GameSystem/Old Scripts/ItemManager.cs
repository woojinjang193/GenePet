using System.Collections.Generic;
using UnityEngine;
public class ItemManager : Singleton<ItemManager>
{
    // 보상 지급 완료 이벤트
    public event System.Action<RewardType, int> OnRewardGranted;

    public void GiveReward(ProductCatalogSO.Entry entry)
    {
        if (entry == null) return;

        for (int i = 0; i < entry.Rewards.Count; i++) // 보상 개수만큼 반복
        {
            ProductCatalogSO.RewardEntry reward = entry.Rewards[i]; // 현재 보상
            ApplyReward(reward.RewardType, reward.RewardAmount);    // 실제 지급
        }
    }

    // 실제 보상 적용 함수
    private void ApplyReward(RewardType type, int amount)
    {
        switch (type)
        {
            case RewardType.RemovedAD:
                // 광고 제거 영구 플래그 저장
                Manager.Save.CurrentData.UserData.Items.IsAdRemoved = true;
                Debug.Log("광고 제거 적용");
                break;

            case RewardType.Energy:
                var user = Manager.Save.CurrentData.UserData;
                user.Energy += amount;

                var uiManager = FindObjectOfType<InGameUIManager>();    
                if(uiManager != null)
                {
                    uiManager.UpdateEnergyBar(user.Energy); // UI 갱신
                }
                Debug.Log($"에너지 +{amount}");
                break;

            case RewardType.Coin:
                Manager.Save.CurrentData.UserData.Items.Money += amount;
                Debug.Log($"코인 +{amount}");
                break;

            case RewardType.Snack:
                // 보상 여기
                Manager.Save.CurrentData.UserData.Items.Snack += amount;
                Debug.Log($"스낵 +{amount}");
                break;

            case RewardType.MissingPoster:
                // 보상 여기
                Manager.Save.CurrentData.UserData.Items.MissingPoster += amount;
                Debug.Log($"MissingPoster +{amount}");
                break;

            case RewardType.GeneticScissors:
                // 보상 여기
                Manager.Save.CurrentData.UserData.Items.GeneticScissors += amount;
                Debug.Log($"GeneticScissors +{amount}");
                break;

            case RewardType.GeneticTester:
                // 보상 여기
                Manager.Save.CurrentData.UserData.Items.geneticTester += amount;
                Debug.Log($"GeneticScissors +{amount}");
                break;
        }
        // 외부(UI, 저장 등)에 알림
        OnRewardGranted?.Invoke(type, amount);
    }
}
