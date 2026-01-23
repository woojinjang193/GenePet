using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PinballBrickSpriteSO", menuName = "MiniGameSO/Pinball/PinballBrickSpriteSO")]
public class PinballBrickSpriteSO : ScriptableObject
{
    [Header("직사각형")]
    [SerializeField] public Sprite[] Rectangle;
    [Header("정사각형")]
    [SerializeField] public Sprite[] Square;
    [Header("별")]
    [SerializeField] public Sprite[] Star;
    [Header("하트")]
    [SerializeField] public Sprite[] Heart;
    [Header("다이아몬드")]
    [SerializeField] public Sprite[] Diamond;
    [Header("웨이브")]
    [SerializeField] public Sprite[] Wave;

    public Sprite GetSprite(BrickType type, int remainHP)
    {
        int syncIndex = remainHP - 1; // 남은 HP 가 1이면 0번 인덱스 스프라이트 줘야함

        switch(type)
        {
            case BrickType.Rectangle: return Rectangle[syncIndex];
            case BrickType.Square: return Square[syncIndex];
            case BrickType.Star: return Star[syncIndex];
            case BrickType.Heart: return Heart[syncIndex];
            case BrickType.Diamond: return Diamond[syncIndex];
            case BrickType.Wave: return Wave[syncIndex];
        }
        return null; //타입 없으면 null 반환
    }
}
