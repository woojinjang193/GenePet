using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New PinballLevelPresetSO", menuName = "MiniGameSO/PinballLevelPresetSO")]
public class PinballGamePresetSO : ScriptableObject
{
    [Header("벽돌 배치 프리팹")]
    [SerializeField] private GameObject[] _brickMaps;
    [Header("지하 배치 프리팹")]
    [SerializeField] private GameObject[] _rouletteMaps;

    [Header("노멀 아이템 벽돌 개수")]
    [SerializeField] private int _minNormalItemBrickAmount;
    [SerializeField] private int _maxNormalItemBrickAmount;

    [Header("색 벽돌 개수")]
    [SerializeField] [Min(3)] private int _minColorBrickAmount; //3개 이상
    [SerializeField] private int _maxColorBrickAmount;

    [Header("노멀 벽돌 HP")]
    [SerializeField] private int _normalHP;
    [Header("노멀 아이템 벽돌 HP")]
    [SerializeField] private int _normalItemHP;
    [Header("컬러 아이템 벽돌 HP")]
    [SerializeField] private int _colorHP;

    [Header("노멀 벽돌 아이템들")]
    [SerializeField] private LevelReward[] _normalBrickItems;

    [Header("색깔 벽돌 아이템들")]
    [SerializeField] private LevelReward[] _colorBrickItems;

    [Header("레벨 랜덤 보상")]
    [SerializeField] private LevelReward[] _levelClearRewards;

    public GameObject[] BrickMaps => _brickMaps;
    public GameObject[] RouletteMaps => _rouletteMaps;

    public int MinColorBrickAmount => _minColorBrickAmount;
    public int MaXColorBrickAmount => _maxColorBrickAmount;
    public int MinNormalItemBrickAmount => _minNormalItemBrickAmount;
    public int MaXNormalItemBrickAmount => _maxNormalItemBrickAmount;

    public int NormalHP => _normalHP;
    public int NormalItemHP => _normalItemHP;
    public int ColorHP => _colorHP;

    public LevelReward[] NormalBrickItems => _normalBrickItems;
    public LevelReward[] ColorBrickItems => _colorBrickItems;
    public LevelReward[] LevelClearRewards => _levelClearRewards;

    
}
