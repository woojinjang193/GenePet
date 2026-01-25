using System.Collections.Generic;
using UnityEngine;

public sealed class RewardReservation
{
    public int RemainingEggSlots { get; private set; } // 남은 알 슬롯
    public bool AnyRoomReserved { get; private set; } // 이번 라운드/스폰에 방을 이미 하나 예약했는지

    private readonly HashSet<Room> _ownedRooms = new(); // 유저가 이미 가진 방 캐시
    private readonly HashSet<Room> _reservedRooms = new(); // 이번 라운드/스폰에서 예약된 방

    public void ResetFromUser(UserData user, int maxEggAmount) // 라운드 시작 시 유저 상태로 초기화
    {
        _ownedRooms.Clear(); // 캐시 초기화
        _reservedRooms.Clear(); // 예약 초기화
        AnyRoomReserved = false; // 방 예약 가드 초기화

        int curEgg = user.EggList.Count;
        RemainingEggSlots = Mathf.Max(0, maxEggAmount - curEgg); // 남은 슬롯 계산

        if (user?.Items?.Rooms != null) //null 방어
        {
            for (int i = 0; i < user.Items.Rooms.Count; i++) _ownedRooms.Add(user.Items.Rooms[i]); //소유 방 캐시
        }
    }

    public bool CanPick(RewardType type) // 이 보상이 지금 등장 가능한지
    {
        if (type == RewardType.Egg) return RemainingEggSlots > 0; // 알 슬롯 있으면 가능

        if (MiniGameRewardPicker.TryGetRoomFromRewardType(type, out var room)) // 방 타입이면
        {
            if (AnyRoomReserved) return false; // 방 1개 제한이면 여기서 차단
            if (room == Room.Default) return false; // 매핑 실수 방어
            if (_ownedRooms.Contains(room)) return false; // 이미 소유면 불가
            if (_reservedRooms.Contains(room)) return false; // 이미 예약했으면 불가
            return true;
        }

        return true; // 나머지 보상들은 제한 없음
    }

    public void Reserve(RewardType type) // 뽑힌 보상을 예약 처리(다음 뽑기에서 제한 반영)
    {
        if (type == RewardType.Egg)
        {
            RemainingEggSlots = Mathf.Max(0, RemainingEggSlots - 1); // 슬롯 1개 예약
            return;
        }

        if (MiniGameRewardPicker.TryGetRoomFromRewardType(type, out var room))
        {
            if (room == Room.Default) return; // 디폴트면 리턴
            AnyRoomReserved = true; // 방 1개 제한 예약
            _reservedRooms.Add(room); // 중복 방지용
        }
    }
}
