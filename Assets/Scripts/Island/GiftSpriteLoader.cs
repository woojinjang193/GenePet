using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftSpriteLoader : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] _gifts;

    private void Awake()
    {
        var sprite = Manager.Item.ItemImages;

        _gifts[0].sprite = sprite.Gift1;
        _gifts[1].sprite = sprite.Gift2;
        _gifts[2].sprite = sprite.Gift3;
        _gifts[3].sprite = sprite.Gift4;
        _gifts[4].sprite = sprite.MasterGiftSprite;
    }
}
