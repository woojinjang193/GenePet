using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BodySO", menuName = "SO/BodySO")]
public class BodySO : ScriptableObject
{
    public string ID;
    public string DisplayName;
    public RarityType Rarity;
    public Sprite sprite;
}
