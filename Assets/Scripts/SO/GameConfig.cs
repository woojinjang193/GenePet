using UnityEngine;

[CreateAssetMenu(fileName = "New GameConfigSO", menuName = "SO/GameConfigSO")]
public class GameConfig : ScriptableObject
{
    [Header("유저 설정")]
    [Header("최대 소유 펫 수")]
    public int MaxPetAmount;
    [Header("최대 소유 알 수")]
    public int MaxEggAmount;
    [Header("맥스 에너지")]
    public int MaxEnergy;
    [Header("에너지 1오르는데 걸리는 시간")]
    public float EnergyRecoveringTime;

    [Header("섬 설정")]
    [Header("섬 방문 호감도 쿨타임")]
    public float VisitingAffinityCooldown;
    [Header("섬 방문 호감도 양")]
    public float VisitingAffinityGain;
    [Header("선물주기 가능 쿨타임")]
    public float GiftCooldown;

    [Header("알SO")]
    public EggSO EggRaritySO;

    [Header("가출후 데려온 후 스텟")]
    [Header("포만감")]
    public float ComeBackHunger;
    [Header("청결도")]
    public float ComeBackCleanliness;
    [Header("행복도(감소량)")]
    public float ComeBackHappiness;
    [Header("체력")]
    public float ComeBackHealth;
}
