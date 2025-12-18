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

}
