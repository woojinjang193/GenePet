using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRoomController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private RoomSpriteSO _roomSprite;

    private void Awake()
    {
        if( _renderer == null )
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }
    }
    public void SetRoom(Room room)
    {
        _renderer.sprite = _roomSprite.GetRoomSprite( room );
    }
}
