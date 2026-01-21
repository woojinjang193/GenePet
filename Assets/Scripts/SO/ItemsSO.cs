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
    [SerializeField] public Sprite Gift4;

    [Header("펫 아이템")]
    [SerializeField] public Sprite MissingPosterSprite;
    [SerializeField] public Sprite GeneticScissorsSprite;
    [SerializeField] public Sprite geneticTesterSprite;

    [Header("방 배경")]
    public Sprite Default;
    public Sprite Room1;
    public Sprite Room2;
    public Sprite Room3;
    public Sprite Room4;
    public Sprite Room5;
    public Sprite Room6;

    public Sprite GetRoomSprite(Room room)
    {
        switch (room)
        {
            case Room.Room1: return Room1;
            case Room.Room2: return Room2;
            case Room.Room3: return Room3;
            case Room.Room4: return Room4;
            case Room.Room5: return Room5;
                //case Room.Room6: return Room1;
        }
        return Default;
    }

    public Sprite GetGiftSprite(Gift gift)
    {
        switch (gift)
        {
            case Gift.Gift1: return Gift1;
            case Gift.Gift2: return Gift2;
            case Gift.Gift3: return Gift3;
            case Gift.Gift4: return Gift4;
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
            case RewardType.Gift4: return Gift4;
        }
        Debug.LogError("스프라이트 반환 안됨");
        return null;
    }
}
