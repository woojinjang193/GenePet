using System.Collections;
using System.Collections.Generic;
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
}
