using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGameDifficultyController : MonoBehaviour
{
    [Header("난이도 상승하는 청크 단위")]
    [SerializeField] private int _difficultyStep;
    [Header("최고 난이도 레벨")]
    [SerializeField] private int _maxLevel;
    public int GetLevel(int chunkIndex)
    {
        if (_difficultyStep <= 0) return 0;

        int level = chunkIndex / _difficultyStep; //현재 레벨 = 현재 청크 인덱스 / 난이도 단위
        return Mathf.Min(level, _maxLevel); // 최대 레벨까지만 반환
    }
}
