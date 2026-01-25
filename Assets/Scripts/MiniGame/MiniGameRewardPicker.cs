using System.Collections.Generic;
using UnityEngine;

public static class MiniGameRewardPicker
{
    private static LevelReward _none = new LevelReward { RewardType = RewardType.None, Amount = 0, Weight = 0f }; //안전 기본값

    //조건에 따라 보상 후보 필터링
    public static List<LevelReward> BuildAvailableRewards(LevelReward[] rewards, UserData user, int maxEggAmount, RewardReservation reservation) // [추가]
    {
        List<LevelReward> result = new();
        if (rewards == null || rewards.Length == 0) return result; //리워드 비어있으면 리턴

        for (int i = 0; i < rewards.Length; i++)
        {
            if (IsRewardAvailable(rewards[i], user, maxEggAmount, reservation)) //획득 가능하면
            {
                result.Add(rewards[i]); //리스트에 추가
            }
        }
        return result;
    }
    private static bool IsRewardAvailable(LevelReward reward, UserData user, int maxEggAmount, RewardReservation reservation) //보상 등장 가능 여부
    {
        if (user == null) return false; //유저 없으면 리턴
        if (reward.RewardType == RewardType.None) return false;

        if (reservation != null && !reservation.CanPick(reward.RewardType)) return false; // 예약 상태로 먼저 차단

        // 알: 보유 가능할 때만 등장
        if (reward.RewardType == RewardType.Egg)
        {
            int curEgg = (user.EggList == null) ? 0 : user.EggList.Count;
            return curEgg < maxEggAmount; //알 얻을 자리 있으면 true
        }

        // 방 이미 소유중이면 등장 X
        if (TryGetRoomFromRewardType(reward.RewardType, out Room room))
        {
            if (user.Items == null) return true; // Items가 null이면 미소유로 보고 등장 허용
            if (user.Items.Rooms == null) return true;
            return !user.Items.Rooms.Contains(room); // 방 소유중 아니면 true
        }

        return true; //다른 아이템들은 그냥 획득 가능
    }
    public static bool TryGetRoomFromRewardType(RewardType type, out Room room) //RewardType > Room 매핑
    {
        room = Room.Default;
        switch (type)
        {
            case RewardType.Room_Jump: room = Room.Room_Jump; return true;
            case RewardType.Room_Rythm: room = Room.Room_Rythm; return true;
            case RewardType.Room_Pinball: room = Room.Room_Pinball; return true;
            case RewardType.Room_Poor: room = Room.Room_Poor; return true;
            case RewardType.Room_Cozy: room = Room.Room_Cozy; return true;
            case RewardType.Room_Something: room = Room.Room_Something; return true;
            default: return false;
        }
    }
    private static LevelReward GetRewardByWeight(IList<LevelReward> rewards) ////배열/리스트 모두 받음
    {
        if (rewards == null || rewards.Count == 0) return _none; // 풀이 비었으면 기본 반환

        float totalWeight = 0f;
        for (int i = 0; i < rewards.Count; i++) totalWeight += rewards[i].Weight; //토탈 웨이트 누적

        if (totalWeight <= 0f) return _none; //전부 0이면 None 반환

        float rand = Random.Range(0f, totalWeight); //랜덤 숫자 뽑음
        float acc = 0f;

        for (int i = 0; i < rewards.Count; i++) //해당 아이템 찾기
        {
            acc += rewards[i].Weight;
            if (rand <= acc) return rewards[i];
        }

        return rewards[rewards.Count - 1]; //걸리는 구간 없으면 마지막 아이템 반환
    }

    // =====================랜덤 보상 뽑기 (알/방 보상 뽑혔을때 풀에서 제거)================
    public static LevelReward GetRandomReward(List<LevelReward> pool, RewardReservation reservation)
    {
        LevelReward reward = GetRewardByWeight(pool); // 웨이트기반 뽑기

        if (reward.RewardType == RewardType.None) return reward; // None이면 예약/제거 안 함

        reservation?.Reserve(reward.RewardType); // 예약 반영

        if (reward.RewardType == RewardType.Egg) //뽑힌 보상이 알이면
            pool.RemoveAll(reward => reward.RewardType == RewardType.Egg); //알 전부 제거

        if (TryGetRoomFromRewardType(reward.RewardType, out _)) //방이 뽑혔으면
            pool.RemoveAll(reward => TryGetRoomFromRewardType(reward.RewardType, out _)); //방 전부 제거

        return reward;
    }
}
