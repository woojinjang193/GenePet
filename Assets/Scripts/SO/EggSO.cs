using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EggSO", menuName = "SO/EggSO")]
public class EggSO : ScriptableObject
{
    public Sprite CommonSprite;
    public Sprite RareSprite;
    public Sprite EpicSprite;
    public Sprite LegendarySprite;

    public Sprite GetEggSprite(RarityType type)
    {
        switch (type)
        {
            case RarityType.Common: return CommonSprite;
            case RarityType.Rare: return RareSprite;
            case RarityType.Epic: return EpicSprite;
            case RarityType.Legendary: return LegendarySprite;
                default: return null;
        }
    }
}
