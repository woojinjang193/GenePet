using UnityEngine;

[CreateAssetMenu(fileName = "New GameConfigSO", menuName = "SO/GameConfigSO")]
public class GameConfig : ScriptableObject
{
    [Header("최대 소유 펫 수")]
    public int MaxPetArmount;
    [Header("최대 소유 알 수")]
    public int MaxEggAmount;
}
