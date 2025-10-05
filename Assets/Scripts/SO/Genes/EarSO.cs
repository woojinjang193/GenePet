using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EarSO", menuName = "SO/EarSO")]
public class EarSO : ScriptableObject
{
    public string ID;
    public string DisplayName;
    public RarityType Rarity;
    public Sprite sprite;
}
