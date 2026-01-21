using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New PinballLevelPresetSO", menuName = "MiniGameSO/PinballLevelPresetSO")]
public class PinballGameLevelPresetSO : ScriptableObject
{
    [Header("벽돌 배치")]
    [SerializeField] private GameObject _brickMap;

    [Header("노멀 아이템 벽돌 개수")]
    [SerializeField] private int _minNormalItemBrickAmount;
    [SerializeField] private int _maxNormalItemBrickAmount;

    [Header("노멀 벽돌 아이템들")]
    [SerializeField] private LevelReward[] _normalBrickItems;

    [Header("색 벽돌 개수")]
    [SerializeField] [Min(3)] private int _minColorBrickAmount; //3개 이상
    [SerializeField] private int _maxColorBrickAmount;

    [Header("색깔 벽돌 아이템들")]
    [SerializeField] private LevelReward[] _colorBrickItems;

    [Header("레벨 랜덤 보상")]
    [SerializeField] private LevelReward[] _levelClearRewards;

    public GameObject BrickMap => _brickMap;
    public int MinColorBrickAmount => _minColorBrickAmount;
    public int MaXColorBrickAmount => _maxColorBrickAmount;
    public int MinNormalItemBrickAmount => _minNormalItemBrickAmount;
    public int MaXNormalItemBrickAmount => _maxNormalItemBrickAmount;
    public LevelReward[] NormalBrickItems => _normalBrickItems;
    public LevelReward[] ColorBrickItems => _colorBrickItems;
    public LevelReward[] LevelClearRewards => _levelClearRewards;

    
}
