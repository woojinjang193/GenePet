using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EyeSO", menuName = "SO/EyeSO")]
public class EyeSO : ScriptableObject
{
    public string ID;
    public string DisplayName;
    public RarityType Rarity;
    public Sprite sprite;
}
