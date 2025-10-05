using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HornSO", menuName = "SO/HornSO")]
public class HornSO : ScriptableObject
{
    public string ID;
    public string DisplayName;
    public RarityType Rarity;
    public Sprite sprite;
}
