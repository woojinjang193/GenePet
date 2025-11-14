using UnityEngine;

[CreateAssetMenu(fileName = "New ConfigSO", menuName = "SO/PetConfigSO")]
public class PetConfigSO : ScriptableObject
{
    [Header("성장상태")]
    public GrowthStatus GrowthType;

    [Header("다음 성장까지 걸리는 시간")]
    public float TimeToGrow;

    [Header("다음 성장까지 필요한 경험치")]
    public float ExpToGrow;

    [Header("배고픔 초당 감소")]
    public float HungerDecreasePerSec;

    [Header("행복도 초당 감소")]
    public float HappinessDecreasePerSec;

    [Header("청결도 초당 감소")]
    public float CleanlinessDecreasePerSec;

    [Header("놀아주기시 오르는 행복도 수치")]
    public float PlayHappinessGain;

    [Header("체력 초당 감소")]
    public float HealthDecreasePerSec;
}
