using System;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;

public class RythmRewardPlanner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmFlowController _flow;

    private RythmGameManager _rythmManager;
    // patternIndex -> 확정 보상
    private readonly Dictionary<int, LevelReward> _fixedPatternRewards = new();

    public event Action<RewardType, int, bool> OnGiveReward; // (type, amount, isLast)

    private void Awake()
    {
        if (_flow != null)
        {
            _flow.OnLevelStarted += BuildPlanForLevel;
        }
    }
    public void InjectManager(RythmGameManager rythmManager) //외부에서 주입
    {
        _rythmManager = rythmManager;
    }
    // 레벨 시작할 때: 보상 계획을 미리 만든다(고정)
    private void BuildPlanForLevel(RythmLevelPresetSO preset)
    {
        if (preset == null) return;

        _fixedPatternRewards.Clear();

        // 1) 이번 레벨에서 줄 보상 개수 N 확정
        int min = Mathf.Max(0, preset.MinItemCount);
        int max = Mathf.Max(min, preset.MaxItemCount);
        int n = UnityEngine.Random.Range(min, max + 1);

        if (n <= 0) return;
        if (preset.LevelRewards == null || preset.LevelRewards.Length == 0) return;

        //예약 상태 가져오기(알/방 중복 차단용)
        var reservation = (_rythmManager != null) ? _rythmManager.RewardReservation : null;

        // 패턴 보상 풀
        var rewardPool = Manager.Mini.GetAvailableRewardPool(preset.LevelRewards, reservation);
        if (rewardPool == null || rewardPool.Count == 0) return; //풀 없으면 계획 생성 불가

        // 2) 지급될 "패턴 번호"를 미리 뽑아 고정(중복 없이)
        List<int> indices = PickDistinctPatternIndices(_flow.TotalRythmCount, n);

        // 3) 풀에서 N개를 미리 뽑아 patternIndex에 고정
        //None이 나오면 해당 패턴은 보상 지정하지 않음(일반고기 이벤트랑 충돌 방지)
        for (int i = 0; i < indices.Count; i++)
        {
            if (rewardPool.Count == 0) break; //풀이 비면 중단

            int patternIndex = indices[i];
            LevelReward picked = MiniGameRewardPicker.GetRandomReward(rewardPool, reservation);

            if (picked.RewardType == RewardType.None) break; //weight0/실패면 더 이상 계획 생성하지 않음

            _fixedPatternRewards[patternIndex] = picked;
        }
    }

    // 패턴 성공 시 호출: 해당 패턴이 보상 대상이면 지급
    public void TryGivePatternReward(int patternIndex)
    {
        if (_fixedPatternRewards.TryGetValue(patternIndex, out var reward))
        {
            OnGiveReward?.Invoke(reward.RewardType, reward.Amount, false);
            _fixedPatternRewards.Remove(patternIndex); // 같은 패턴에서 중복 지급 방지
        }
        else
        {
            OnGiveReward?.Invoke(RewardType.None, 0, false); //일반고기 잡기위한 이벤트
        }
    }

    // 레벨 마지막 패턴 성공 시 호출: 클리어 보상 1개 지급
    public void GiveClearReward(RythmLevelPresetSO preset)
    {
        if (preset == null) return;
        if (preset.LevelClearRewards == null || preset.LevelClearRewards.Length == 0) return;

        var reservation = (_rythmManager != null) ? _rythmManager.RewardReservation : null; //예약 상태 가져오기

        var clearPool = Manager.Mini.GetAvailableRewardPool(preset.LevelClearRewards, reservation);
        if (clearPool == null || clearPool.Count == 0) return; //풀 없으면 지급 불가

        LevelReward picked = MiniGameRewardPicker.GetRandomReward(clearPool, reservation);

        OnGiveReward?.Invoke(picked.RewardType, picked.Amount, true);
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
}
