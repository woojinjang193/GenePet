using UnityEngine;

[CreateAssetMenu(fileName = "New GameConfigSO", menuName = "SO/GameConfigSO")]
public class GameConfig : ScriptableObject
{
    [Header("최대 소유 펫 수")]
    public int MaxPetAmount;
    [Header("최대 소유 알 수")]
    public int MaxEggAmount;
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
