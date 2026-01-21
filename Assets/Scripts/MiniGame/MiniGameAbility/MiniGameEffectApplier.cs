using UnityEngine;

// 미니게임 효과 적용기
public static class MiniGameEffectApplier
{
    public static MiniGameContext Apply(MiniGamePersonalityEffectSO table, PersonalityType personality, float happiness01)
    {
        // 컨텍스트 생성
        MiniGameContext context = new MiniGameContext();

        // 성격에 맞는 효과 찾기
        MiniGameEffectSO effect = table.GetEffect(personality);

        // 효과가 있으면 적용
        if (effect != null)
        {
            effect.Apply(context, happiness01);
        }

        return context; // 결과 반환
    }
}
