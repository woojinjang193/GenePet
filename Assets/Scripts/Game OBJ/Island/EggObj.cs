using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggObj : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    public void Init(EggData egg)
    {
        _sprite.sprite = egg.Image;
    }
}
