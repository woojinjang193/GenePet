using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "GameSO/ItemSO")]
public class ItemsSO : ScriptableObject //보상 아이템 이미지 모음 SO
{
    [SerializeField] public Sprite CoinSprite;
    [SerializeField] public Sprite RemoveAdSprite;
    [SerializeField] public Sprite IslandTicketSprite;
    [SerializeField] public Sprite MissingPosterSprite;
    [SerializeField] public Sprite GeneticScissorsSprite;
    [SerializeField] public Sprite geneticTesterSprite;
    [SerializeField] public Sprite SnackSprite;
    [SerializeField] public Sprite Energy;
}
