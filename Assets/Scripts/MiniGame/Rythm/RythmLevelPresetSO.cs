using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New RythmLevelPresetSO", menuName = "MiniGameSO/RythmLevelPresetSO")]
public class RythmLevelPresetSO : ScriptableObject
{
    [Header("BPM")]
    [SerializeField] public float _bpm;

    [Header("레벨 BGM")]
    [SerializeField] private AudioClip _bgmClip;

    [Header("이 레벨에 등장하는 리듬 패턴")]
    [SerializeField] private RythmPatternSO[] _rythmPatternList;

    [Header("마디 수")]
    [SerializeField] private int _measureCount;

    [Header("오디오")]
    [SerializeField] private AudioClip _normalBeatClip;
    [SerializeField] private AudioClip _inputLastBeatClip;

    [Header("레벨당 최소 아이템 개수")]
    public int MinItemCount = 1;
    [Header("레벨당 최대 아이템 개수")]
    public int MaxItemCount = 1;

    [Header("레벨 아이템 보상 옵션")]
    public LevelReward[] LevelRewards;
    [Header("레벨 클리어 보상 옵션")]
    public LevelReward[] LevelClearRewards;

    public float BPM => _bpm;
    public int MeasureCount => _measureCount;
    public RythmPatternSO[] RythmPatternList => _rythmPatternList;
    public AudioClip NormalBeatClip => _normalBeatClip;
    public AudioClip InputLastBeatClip => _inputLastBeatClip;
    public AudioClip BGMClip => _bgmClip;
}
[Serializable]
public struct LevelReward
{
    public RewardType RewardType;
    public int Amount;
    public float Weight;
}
