using System;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpGameDifficultyPreset", menuName = "SO/JumpGame")]
public class JumpGameDifficultyPreset : ScriptableObject
{
    [Header("발판 프리펩")]
    public GameObject[] PlatformPrefabs; // 난이도별 사용 가능 발판들
    [Header("Y 최소 간격")]
    public float MinGapY;
    [Header("X 간격")]
    public int DistanceBetweenPlatform = 1;

    [Header("청크당 최소 아이템 개수")]
    public int MinItemCount = 1;
    [Header("청크당 최대 아이템 개수")]
    public int MaxItemCount = 1;

    [Header("레벨 아이템 보상 옵션")]
    public LevelReward[] LevelRewards;
    [Header("레벨 클리어 보상 옵션")]
    public LevelReward[] LevelClearRewards;

    [Header("배경 이미지")]
    [Header("시작점")]
    public Sprite BackgroundStart;
    [Header("루프")]
    public Sprite BackgroundLoop;
   //[Header("끝")]
   //public Sprite BackgroundEnd;

}

