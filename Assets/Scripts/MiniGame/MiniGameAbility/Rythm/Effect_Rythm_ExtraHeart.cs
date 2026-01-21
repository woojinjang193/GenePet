using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MiniGameEffect/Rythm/Calm/ExtraHeart")]
public class Effect_Rythm_ExtraHeart : MiniGameEffectSO
{
    [Header("최대증가 하트 개수")]
    [SerializeField] private int MaxAdditionalHeart = 3;
    public override void Apply(MiniGameContext context, float happiness01)
    {
        // 행복도 0~1 보정
        float t = Mathf.Clamp01(happiness01);

        int extraAmount = 0;

        if (t >= 0.95f) //행복도 95 이상이면 최대치
        {
            extraAmount = MaxAdditionalHeart;
        }
        else
        {
            extraAmount = Mathf.FloorToInt(Mathf.Lerp(1f, MaxAdditionalHeart, t));
        }

        context.ExtraHeart += extraAmount;
    }
}
