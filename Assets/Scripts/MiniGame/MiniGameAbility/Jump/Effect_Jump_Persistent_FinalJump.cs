using UnityEngine;

[CreateAssetMenu(menuName = "MiniGameEffect/Jump/Persistent/FinalJump")]
public class Effect_Jump_Persistent_FinalJump : MiniGameEffectSO
{
    [Header("마지막 점프 최소/최대 파워")]
    [SerializeField] private float _minPower = 10f; // 최소 파워
    [SerializeField] private float _maxPower = 20f; // 최대 파워

    public override void Apply(MiniGameContext context, float happiness01)
    {
        // 행복도 0~1 보정
        float t = Mathf.Clamp01(happiness01);

        // 행복도에 따른 파워 계산
        float power = Mathf.Lerp(_minPower, _maxPower, t);

        // 파워 적용
        context.Jump_FinalJumpPower = power;
    }
}
