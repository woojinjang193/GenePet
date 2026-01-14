using UnityEngine;

public abstract class MiniGameEffectSO : ScriptableObject
{
    public abstract void Apply(MiniGameContext context, float happiness01);
}
