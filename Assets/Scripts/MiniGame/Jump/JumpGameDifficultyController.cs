using UnityEngine;

public struct DifficultyResult
{
    public int Level;
    public bool IsLastChunkOfLevel;
}
public class JumpGameDifficultyController : MonoBehaviour
{
    [Header("난이도 상승하는 청크 단위")]
    [SerializeField] private int _difficultyStep;
    [Header("최고 난이도 레벨")]
    [SerializeField] private int _maxLevel;

    public DifficultyResult GetLevel(int chunkIndex)
    {
        DifficultyResult result = new();

        if (_difficultyStep <= 0) //_difficultyStep설정 이상할때
        {
            result.Level = 0;
            result.IsLastChunkOfLevel = false;
            return result;
        }

        int level = chunkIndex / _difficultyStep;
        bool isLast = (chunkIndex + 1) % _difficultyStep == 0; //나머지가 0일때

        result.Level = Mathf.Min(level, _maxLevel); //맥스레벨 이상은 맥스레벨로 고정

        if (result.Level >= _maxLevel)
        {
            result.IsLastChunkOfLevel = false; //맥스레벨일땐 마지막청크 아님
        }
        else
        {
            result.IsLastChunkOfLevel = isLast;
        }

        return result;
    }
}

