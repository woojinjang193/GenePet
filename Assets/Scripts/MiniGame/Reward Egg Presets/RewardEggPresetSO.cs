using System;
using UnityEngine;
public enum EggPresetMode { ByRarity, ById }

[CreateAssetMenu(fileName = "New RewardEggPresetSO", menuName = "MiniGameSO/RewardEggPresetSO")] 
public class RewardEggPresetSO : ScriptableObject
{
    [Header("보상 전용 알 파츠 정보")]
    [SerializeField] private PartInfoForEggPreset[] _partInfo;

    public PartInfoForEggPreset[] PartInfo => _partInfo; //읽기 전용

}

[Serializable]
public struct PartInfoForEggPreset
{
    public PartType PartType;
    public EggPresetMode Mode;

    //Mode에 따라 아래 선택입력
    public RarityType RarityType;
    public string DominantID;
}
