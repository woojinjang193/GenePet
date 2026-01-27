using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "GameSO/ItemSO")]
public class ItemsSO : ScriptableObject //보상 아이템 이미지 모음 SO
{
    [Header("유저 아이템")]
    [SerializeField] public Sprite CoinSprite;
    [SerializeField] public Sprite RemoveAdSprite;
    [SerializeField] public Sprite SnackSprite;
    [SerializeField] public Sprite Energy;
    [SerializeField] public Sprite PetSlot;

    [Header("섬 아이템")]
    [SerializeField] public Sprite IslandTicketSprite;
    [SerializeField] public Sprite MasterGiftSprite;
    [SerializeField] public Sprite Gift1;
    [SerializeField] public Sprite Gift2;
    [SerializeField] public Sprite Gift3;

    [Header("펫 아이템")]
    [SerializeField] public Sprite MissingPosterSprite;
    [SerializeField] public Sprite GeneticScissorsSprite;
    [SerializeField] public Sprite geneticTesterSprite;

    [Header("알SO")]
    [SerializeField] public EggSO EggRaritySO;

    [Header("미니게임 보상 알")]
    [SerializeField] public Sprite RewardEgg;

    [Header("방 배경(아이템)")]
    public Sprite Default;
    public Sprite Room_Jump;
    public Sprite Room_Rythm;
    public Sprite Room_Pinball;
    public Sprite Room_Cozy;
    public Sprite Room_Poor;
    public Sprite Room_Something;

    public Sprite GetGiftSprite(Gift gift)
    {
        switch (gift)
        {
            case Gift.Gift1: return Gift1;
            case Gift.Gift2: return Gift2;
            case Gift.Gift3: return Gift3;
            default: return null;
        }
    }

    public Sprite GetItemSprite(RewardType type)
    {
        switch (type)
        {
            case RewardType.Energy: return Energy;
            case RewardType.Coin: return CoinSprite;
            case RewardType.RemovedAD: return RemoveAdSprite;
            case RewardType.IslandTicket: return IslandTicketSprite;
            case RewardType.GeneticScissors: return GeneticScissorsSprite;
            case RewardType.GeneticTester: return geneticTesterSprite;
            case RewardType.MissingPoster: return MissingPosterSprite;
            case RewardType.Snack: return SnackSprite;
            case RewardType.MasterGift: return MasterGiftSprite;
            case RewardType.PetSlot: return PetSlot;
            case RewardType.Gift1: return Gift1;
            case RewardType.Gift2: return Gift2;
            case RewardType.Gift3: return Gift3;

            case RewardType.Room_Jump: return Room_Jump;
            case RewardType.Room_Rythm: return Room_Rythm;
            case RewardType.Room_Pinball: return Room_Pinball;
            case RewardType.Room_Cozy: return Room_Cozy;
            case RewardType.Room_Poor: return Room_Poor;
            case RewardType.Room_Something: return Room_Something; 

            case RewardType.Egg: return RewardEgg;
        }
        Debug.LogError("스프라이트 반환 안됨");
        return null;
    }
}
