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

    [Header("포만도 초당 감소")]
    public float HungerDecreasePerSec;

    [Header("행복도 초당 감소")]
    public float HappinessDecreasePerSec;

    [Header("청결도 초당 감소")]
    public float CleanlinessDecreasePerSec;

    [Header("놀아주기시 오르는 행복도 수치")]
    public float PlayHappinessGain;

    [Header("체력 초당 감소")]
    public float HealthDecreasePerSec;

    [Header("체력 초당 증가")]
    public float HealthIncreasePerSec;

    [Header("체력 증가 기준 포만도")]
    public float HungerAmountHealthIncrease;

    [Header("병걸리는 청결도 기준")]
    public float CleanlinessAmountGetSick;

    [Header("기준 청결도 이하일때 병걸리는 시간")]
    public float TimeToGetSick;
}
