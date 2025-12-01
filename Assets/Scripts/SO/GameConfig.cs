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
}
