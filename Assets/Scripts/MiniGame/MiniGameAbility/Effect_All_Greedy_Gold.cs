using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MiniGameEffect/Jump/Greedy/GoldMultiplier")]
public class Effect_All_Greedy_Gold : MiniGameEffectSO
{
    [SerializeField] private float _maxMultiplier = 1.5f; // 최대 배율

    public override void Apply(MiniGameContext context, float happiness01)
    {
        // 행복도 0~1 보정
        float t = Mathf.Clamp01(happiness01);

        // 1 ~ 최대배율 계산
        float multiplier = Mathf.Lerp(1f, _maxMultiplier, t);

        // 골드 배율 적용
        context.GoldMultiplier *= multiplier;
    }
}
