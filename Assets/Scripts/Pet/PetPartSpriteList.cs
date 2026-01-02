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
    public SpriteRenderer Tail;
    public SpriteRenderer Whiskers;

    [Header("아웃라인 파츠")]
    //public SpriteRenderer AccOut;
    public SpriteRenderer ArmOut;
    //public SpriteRenderer BlushOut;
    public SpriteRenderer BodyOut;
    public SpriteRenderer EarOut;
    public SpriteRenderer FeetOut;
    public SpriteRenderer WingOut;
    public SpriteRenderer TailOut;

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
    public Image Tail;
    public Image Whiskers;

    [Header("아웃라인 파츠")]
    public Image ArmOut;
    public Image BodyOut;
    public Image EarOut;
    public Image FeetOut;
    public Image WingOut;
    public Image TailOut;

    [Header("마스크")]
    public Image PatternMask;

    public void OffAll()
    {
        Acc.gameObject.SetActive(false);
        Arm.gameObject.SetActive(false);
        Blush.gameObject.SetActive(false);
        Body.gameObject.SetActive(false);
        Ear.gameObject.SetActive(false);
        Eye.gameObject.SetActive(false);
        Feet.gameObject.SetActive(false);
        Mouth.gameObject.SetActive(false);
        Pattern.gameObject.SetActive(false);
        Wing.gameObject.SetActive(false);
        Tail.gameObject.SetActive(false);
        Whiskers.gameObject.SetActive(false);

        ArmOut.gameObject.SetActive(false);
        BodyOut.gameObject.SetActive(false);
        EarOut.gameObject.SetActive(false);
        FeetOut.gameObject.SetActive(false);
        WingOut.gameObject.SetActive(false);
        TailOut.gameObject.SetActive(false);
    }
    public void SetBaby()
    {
        Eye.gameObject.SetActive(true);
        Body.gameObject.SetActive(true);
        Ear.gameObject.SetActive(true);
        Blush.gameObject.SetActive(true);
        Mouth.gameObject.SetActive(true);
        Tail.gameObject.SetActive(true);

        BodyOut.gameObject.SetActive(true);
        EarOut.gameObject.SetActive(true);
        TailOut.gameObject.SetActive(true);
    }
    public void SetTeen()
    {
        Blush.gameObject.SetActive(true);
        Body.gameObject.SetActive(true);
        Ear.gameObject.SetActive(true);
        Eye.gameObject.SetActive(true);
        Feet.gameObject.SetActive(true);
        Mouth.gameObject.SetActive(true);
        Tail.gameObject.SetActive(true);
        Whiskers.gameObject.SetActive(true);

        BodyOut.gameObject.SetActive(true);
        EarOut.gameObject.SetActive(true);
        FeetOut.gameObject.SetActive(true);
        TailOut.gameObject.SetActive(true);
    }
    public void SetAdult()
    {
        Acc.gameObject.SetActive(true);
        Arm.gameObject.SetActive(true);
        Blush.gameObject.SetActive(true);
        Body.gameObject.SetActive(true);
        Ear.gameObject.SetActive(true);
        Eye.gameObject.SetActive(true);
        Feet.gameObject.SetActive(true);
        Mouth.gameObject.SetActive(true);
        Pattern.gameObject.SetActive(true);
        Wing.gameObject.SetActive(true);
        Tail.gameObject.SetActive(true);
        Whiskers.gameObject.SetActive(true);

        ArmOut.gameObject.SetActive(true);
        BodyOut.gameObject.SetActive(true);
        EarOut.gameObject.SetActive(true);
        FeetOut.gameObject.SetActive(true);
        WingOut.gameObject.SetActive(true);
        TailOut.gameObject.SetActive(true);
    }

}
