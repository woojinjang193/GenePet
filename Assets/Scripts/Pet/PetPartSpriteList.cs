using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PetPartSpriteList
{
    [Header("베이스 파츠")]
    public SpriteRenderer Acc;
    public SpriteRenderer Arm;
    public SpriteRenderer Blush;
    public SpriteRenderer Body;
    public SpriteRenderer Ear;
    public SpriteRenderer Eye;
    public SpriteRenderer Feet;
    public SpriteRenderer Mouth;
    public SpriteRenderer Pattern;
    public SpriteRenderer Wing;

    [Header("아웃라인 파츠")]
    //public SpriteRenderer AccOut;
    public SpriteRenderer ArmOut;
    //public SpriteRenderer BlushOut;
    public SpriteRenderer BodyOut;
    public SpriteRenderer EarOut;
    public SpriteRenderer FeetOut;
    public SpriteRenderer WingOut;

    [Header("마스크")]
    public SpriteMask PatternMask;
}
[System.Serializable]
public class PetPartImageList
{
    [Header("베이스 파츠")]
    public Image Acc;
    public Image Arm;
    public Image Blush;
    public Image Body;
    public Image Ear;
    public Image Eye;
    public Image Feet;
    public Image Mouth;
    public Image Pattern;
    public Image Wing;

    [Header("아웃라인 파츠")]
    public Image ArmOut;
    public Image BodyOut;
    public Image EarOut;
    public Image FeetOut;
    public Image WingOut;

    [Header("마스크")]
    public Image PatternMask;
}
