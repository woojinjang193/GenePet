using System.Collections.Generic;
using UnityEngine;

public static class MiniGameRewardPicker
{
    //조건에 따라 보상 후보 필터링
    public static List<LevelReward> BuildAvailableRewards(LevelReward[] rewards, UserData user, int maxEggAmount) 
    {
        List<LevelReward> result = new();
        if (rewards == null || rewards.Length == 0) return result; //리워드 비어있으면 리턴

        for (int i = 0; i < rewards.Length; i++)
        {
            if (IsRewardAvailable(rewards[i], user, maxEggAmount)) //획득 가능하면
            {
                result.Add(rewards[i]); //리스트에 추가
            }
        }
        return result;
    }
    private static bool IsRewardAvailable(LevelReward reward, UserData user, int maxEggAmount) //보상 등장 가능 여부
    {
        if (user == null) return false; //유저 없으면 리턴

        // 알: 보유 가능할 때만 등장
        if (reward.RewardType == RewardType.Egg)
        {
            int curEgg = user.EggList.Count; 
            return curEgg < maxEggAmount; //알 얻을 자리 있으면 true
        }

        // 방 이미 소유중이면 등장 X
        if (TryGetRoomFromRewardType(reward.RewardType, out Room room))
        {
            if (user.Items == null) return false;
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
    public static LevelReward GetRewardByWeight(IList<LevelReward> rewards) ////배열/리스트 모두 받음
    {
        LevelReward none = new LevelReward { RewardType = RewardType.None, Amount = 0, Weight = 0f }; //안전 기본값

        if (rewards == null || rewards.Count == 0) return none; //추가됨!!

        float totalWeight = 0f;
        for (int i = 0; i < rewards.Count; i++) totalWeight += rewards[i].Weight; //토탈 웨이트 누적

        if (totalWeight <= 0f) return none; //전부 0이면 None 반환

        float rand = Random.Range(0f, totalWeight); //랜덤 숫자 뽑음
        float acc = 0f;

        for (int i = 0; i < rewards.Count; i++) //해당 아이템 찾기
        {
            acc += rewards[i].Weight;
            if (rand <= acc) return rewards[i];
        }

        return rewards[rewards.Count - 1]; //걸리는 구간 없으면 마지막 아이템 반환
    }
}
