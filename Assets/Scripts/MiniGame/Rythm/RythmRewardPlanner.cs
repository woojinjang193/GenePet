using System;
using System.Collections.Generic;
using UnityEngine;

public class RythmRewardPlanner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmFlowController _flow;

    // patternIndex -> 확정 보상
    private Dictionary<int, LevelReward> _fixedPatternRewards = new();

    public event Action<RewardType, int> OnGiveReward; // (type, amount)

    private void Awake()
    {
        if (_flow != null)
        {
            _flow.OnLevelStarted += BuildPlanForLevel;
        }
    }

    // 레벨 시작할 때: 보상 계획을 미리 만든다(고정)
    private void BuildPlanForLevel(RythmLevelPresetSO preset)
    {
        _fixedPatternRewards.Clear();
        if (preset == null) return;

        // 1) 이번 레벨에서 줄 보상 개수 N 확정
        int min = Mathf.Max(0, preset.MinItemCount);
        int max = Mathf.Max(min, preset.MaxItemCount);
        int n = UnityEngine.Random.Range(min, max + 1);

        if (n <= 0) return;
        if (preset.LevelRewards == null || preset.LevelRewards.Length == 0) return;

        // 2) 지급될 "패턴 번호"를 미리 뽑아 고정(중복 없이)
        // 0 ~ (TotalRythmCount-1) 범위에서 n개 선택
        List<int> indices = PickDistinctPatternIndices(_flow.TotalRythmCount, n);

        // 3) LevelRewards에서 가중치로 N개를 미리 뽑아 고정(중복 허용)
        for (int i = 0; i < indices.Count; i++)
        {
            int patternIndex = indices[i];
            LevelReward picked = PickWeighted(preset.LevelRewards);
            _fixedPatternRewards[patternIndex] = picked;
        }
    }

    // 패턴 성공 시 호출: 해당 패턴이 보상 대상이면 지급
    public void TryGivePatternReward(int patternIndex)
    {
        if (_fixedPatternRewards.TryGetValue(patternIndex, out var reward))
        {
            OnGiveReward?.Invoke(reward.RewardType, reward.Amount);
            _fixedPatternRewards.Remove(patternIndex); // 같은 패턴에서 중복 지급 방지
        }
    }

    // 레벨 마지막 패턴 성공 시 호출: 클리어 보상 1개 지급
    public void GiveClearReward(RythmLevelPresetSO preset)
    {
        if (preset == null) return;
        if (preset.LevelClearRewards == null || preset.LevelClearRewards.Length == 0) return;

        LevelReward picked = PickWeighted(preset.LevelClearRewards);
        OnGiveReward?.Invoke(picked.RewardType, picked.Amount);
    }

    // ===== 유틸 =====
    private static List<int> PickDistinctPatternIndices(int totalCount, int n)
    {
        n = Mathf.Clamp(n, 0, totalCount);
        List<int> pool = new();
        for (int i = 0; i < totalCount; i++) pool.Add(i);

        // 섞고 앞에서 n개
        for (int i = 0; i < pool.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        return pool.GetRange(0, n);
    }

    private static LevelReward PickWeighted(LevelReward[] rewards)
    {
        float total = 0f;
        for (int i = 0; i < rewards.Length; i++)
            total += Mathf.Max(0f, rewards[i].Weight);

        if (total <= 0f)
            return rewards[UnityEngine.Random.Range(0, rewards.Length)];

        float r = UnityEngine.Random.Range(0f, total);
        float acc = 0f;

        for (int i = 0; i < rewards.Length; i++)
        {
            acc += Mathf.Max(0f, rewards[i].Weight);
            if (r <= acc) return rewards[i];
        }

        return rewards[rewards.Length - 1];
    }
}
