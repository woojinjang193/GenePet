using UnityEngine;

public static class MiniGameRewardPicker
{
    public static LevelReward GetRewardByWeight(LevelReward[] rewards)
    {
        float totalWeight = 0f;

        for (int i = 0; i < rewards.Length; i++)
        {
            totalWeight += rewards[i].Weight;
        }

        float rand = Random.Range(0f, totalWeight); //랜덤 숫자 뽑음
        float acc = 0f;

        for (int i = 0; i < rewards.Length; i++)
        {
            acc += rewards[i].Weight; // 각 보상의 Weight를 누적

            if (rand <= acc) //범위안에 들어오면
            {
                return rewards[i]; //보상 뽑음
            }
        }

        return rewards[rewards.Length - 1]; // 안전장치 (안뽑힐 경우)
    }
}
