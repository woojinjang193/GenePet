using System.Collections.Generic;

public static class RewardTimeRecordService
{
    public static bool CanClaimReward(string id) //데일리 보상 용
    {
        var record = GetOrCreateRecord(id); // 레코드 확보
        string today = GetTodayKey(); // 오늘 날짜

        if (string.IsNullOrEmpty(record.LastDate)) return true; // 기록 없으면 가능(첫보상)
        if (string.Compare(record.LastDate, today) > 0) return false; // 미래 날짜면 패널티(잠금)
        if (record.LastDate == today) return false; // 오늘 이미 받음

        return true; // 과거면 가능
    }
    public static bool CanClaimReward(string id, int cooldownSec) // 쿨타임 보상용
    {
        var record = GetOrCreateRecord(id); // 레코드 확보
        long now = GetNowUnix(); // 현재 시간

        if (record.LastClaimUnix <= 0) return true; // 첫 수령이면 가능
        return (now - record.LastClaimUnix) >= cooldownSec; // 쿨타임 경과 확인
    }

    public static void MarkClaimed(string id) // 받았다고 기록
    {
        var record = GetOrCreateRecord(id);
        record.LastDate = GetTodayKey(); // 하루 1회용(그대로 두기)
        record.LastClaimUnix = GetNowUnix(); // 쿨타임용 시간 기록
        Manager.Save.SaveGame(); 
    }

    private static string GetTodayKey() // 오늘 날짜 문자열(로컬 기준)
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd"); // 날짜 형식 통일
    }
    private static long GetNowUnix() // 현재 시간(Unix seconds)
    {
        return System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // [추가] UTC 기준
    }
    private static RewardClaimRecord GetOrCreateRecord(string id) // ID 레코드 찾기/없으면 생성
    {
        var user = Manager.Save.CurrentData.UserData;

        if (user.RewardClaims == null) user.RewardClaims = new List<RewardClaimRecord>(); //없으면 리스트 생성

        for (int i = 0; i < user.RewardClaims.Count; i++)
        {
            if (user.RewardClaims[i].ID == id) return user.RewardClaims[i];
        }

        var record = new RewardClaimRecord(); // 없으면 새로 생성
        record.ID = id;
        record.LastDate = "";
        user.RewardClaims.Add(record);
        return record;
    }

}
