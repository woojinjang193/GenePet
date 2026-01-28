using UnityEngine;

[CreateAssetMenu(fileName = "RoomSpriteSO", menuName = "GameSO/RoomSpriteSO")]
public class RoomSpriteSO : ScriptableObject
{
    [Header("방 배경")]
    public Sprite Default;
    public Sprite Room_Jump;
    public Sprite Room_Rythm;
    public Sprite Room_Pinball;
    public Sprite Room_Cozy;
    public Sprite Room_Poor;
    public Sprite Room_Something;

    public Sprite GetRoomSprite(Room room)
    {
        switch (room)
        {
            case Room.Room_Jump: return Room_Jump;
            case Room.Room_Rythm: return Room_Rythm;
            case Room.Room_Pinball: return Room_Pinball;
            case Room.Room_Cozy: return Room_Cozy;
            case Room.Room_Poor: return Room_Poor;
            case Room.Room_Something: return Room_Something;
                //case Room.Room6: return Room1;
        }
        return Default;
    }

}
