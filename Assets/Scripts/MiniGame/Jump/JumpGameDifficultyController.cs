using UnityEngine;

public struct DifficultyResult
{
    public int Level;
    public bool IsLastChunkOfLevel;
    public bool IsFirstChunkOfLevel;
    public JumpGameDifficultyPreset Preset;
}
public class JumpGameDifficultyController : MonoBehaviour
{
    [Header("난이도 상승하는 청크 단위")]
    [SerializeField] private int _difficultyStep;
    [Header("최고 난이도 레벨")]
    [SerializeField] private int _maxLevel;
    [Header("레벨 프리셋")]
    [SerializeField] private JumpGameDifficultyPreset[] _presets;

    private void Awake()
    {
        _maxLevel = _presets.Length;
    }
    public DifficultyResult GetLevel(int chunkIndex)
    {
        DifficultyResult result = new();

        if (_difficultyStep <= 0) //_difficultyStep설정 이상할때
        {
            result.Level = 0;
            result.IsLastChunkOfLevel = false;
            result.IsFirstChunkOfLevel = false;
            return result;
        }

        int level = chunkIndex / _difficultyStep;
        int clampedLevel = Mathf.Min(level, _maxLevel);

        result.Level = clampedLevel;
        result.Preset = _presets[clampedLevel];

        bool isLast = (chunkIndex + 1) % _difficultyStep == 0; //나머지가 0일때
        bool isFirst = chunkIndex % _difficultyStep == 0; // 현재 청크 인덱스가 난이도 단계의 시작이면 레벨의 첫 번째 청크

        result.Level = Mathf.Min(level, _maxLevel); //맥스레벨 이상은 맥스레벨로 고정

        if (result.Level >= _maxLevel)
        {
            result.IsLastChunkOfLevel = false; //맥스레벨일땐 마지막청크 아님
            result.IsFirstChunkOfLevel = false; //맥스레벨일땐 마지막청크 아님

        }
        else
        {
            result.IsLastChunkOfLevel = isLast;
            result.IsFirstChunkOfLevel = isFirst;
        }

        return result;
    }
}

