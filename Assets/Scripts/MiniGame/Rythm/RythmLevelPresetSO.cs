using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New RythmLevelPresetSO", menuName = "MiniGameSO/RythmLevelPresetSO")]
public class RythmLevelPresetSO : ScriptableObject
{
    [SerializeField] private float BPM;
    [SerializeField] private RythmPatternSO[] _rythmPatternList;
    [SerializeField] private int _measureCount;

    [Header("레벨당 최소 아이템 개수")]
    public int MinItemCount = 1;
    [Header("레벨당 최대 아이템 개수")]
    public int MaxItemCount = 1;

    [Header("레벨 아이템 보상 옵션")]
    public LevelReward[] LevelRewards;
    [Header("레벨 클리어 보상 옵션")]
    public LevelReward[] LevelClearRewards;
}
[Serializable]
public struct LevelReward
{
    public RewardType RewardType;
    public int Amount;
    public float Weight;
}
