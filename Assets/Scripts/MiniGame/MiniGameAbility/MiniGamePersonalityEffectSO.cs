using System;
using System.Collections.Generic;
using UnityEngine;

// 미니게임 기준 성격 효과 테이블
[CreateAssetMenu(menuName = "MiniGameEffect/MiniGamePersonalityTable")]
public class MiniGamePersonalityEffectSO : ScriptableObject
{
    public MiniGame miniGame; // 대상 미니게임

    public List<PersonalityEffectPair> effects; // 성격별 효과

    // 성격에 맞는 효과 찾기
    public MiniGameEffectSO GetEffect(PersonalityType personality)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].personality == personality)
                return effects[i].effect;
        }
        return null; // 효과 없음
    }
}

// 성격-효과 묶음
[Serializable]
public class PersonalityEffectPair
{
    public PersonalityType personality;   // 성격
    public MiniGameEffectSO effect;        // 효과
}
