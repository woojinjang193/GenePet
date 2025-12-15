using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _icon;
    
    public void Init(Sprite sprite)
    {
        gameObject.SetActive(true);
        _icon.sprite = sprite;
    }

    public void GiftGiven()
    {
        _icon.sprite = null;
        gameObject.SetActive(false);
    }
}
