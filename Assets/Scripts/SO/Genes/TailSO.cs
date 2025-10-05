using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TailSO", menuName = "SO/TailSO")]
public class TailSO : ScriptableObject
{
    public string ID;
    public string DisplayName;
    public RarityType Rarity;
    public Sprite sprite;
}
