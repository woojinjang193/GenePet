using System.Collections.Generic;

public static class RewardTimeRecordService
{
    private static readonly Dictionary<string, string> _cachedDateById = new();

    //==================외부 호출용======================================
    public static bool TryBeginAdClaim(string id) // 버튼 눌렀을때 
    {
        bool can = CanClaimDailyReward(id); // 데일리 체크
        if (!can) return false; //불가면 캐싱 안 함

        _cachedDateById[id] = GetTodayKey(); // 광고 시작 시점 날짜 캐싱
        return true;
    }
    public static bool CanClaimDailyReward(string id) //데일리 보상 용 (버튼 활성화용)
    {
        var record = GetOrCreateRecord(id); // 레코드 확보
        string today = GetTodayKey(); // 오늘 날짜
        int limit = Manager.Game.Config.GetLimitByID(id);

        if (limit <= 0) return false; // 설정 없거나 0이면 못 받게(버그 방지)

        if (string.IsNullOrEmpty(record.LastDate)) return true; // 기록 없으면 가능(첫보상)
        if (string.Compare(record.LastDate, today) > 0) return false; // 미래 날짜면 패널티(잠금)

        EnsureDailyReset(record, today); // 날짜 바뀌면 카운트 리셋

        if (record.LastDate == today && record.TodayCount >= limit) return false; // 오늘 이미 리미트 받음

        return true; // 마지막 수령 날짜가 과거거나 오늘 보상 아직 리밋 아니면 true
    }
    public static bool CanClaimCoolTimeReward(string id) // 쿨타임 보상용  (버튼 활성화용)
    {
        var record = GetOrCreateRecord(id); // 레코드 확보
        long now = GetNowUnix(); // 현재 시간
        int coolTimeSec = Manager.Game.Config.GetCoolTimeByID(id);

        if (record.LastClaimUnix <= 0) return true; // 첫 수령이면 가능
        return (now - record.LastClaimUnix) >= coolTimeSec; // 쿨타임 경과 확인
    }
    public static void MarkClaimed(string id, bool isDaily) //캐싱 날짜 기준으로 저장
    {
        var record = GetOrCreateRecord(id);

        string dateKey = GetTodayKey(); //기본은 현재 날짜
        if (_cachedDateById.TryGetValue(id, out var cached)) dateKey = cached; //캐싱 있으면 캐싱날짜 날짜 우선

        EnsureDailyReset(record, dateKey); //캐싱 날짜 기준으로 리셋 보장

        record.LastDate = dateKey; //캐싱 날짜로 저장
        record.LastClaimUnix = GetNowUnix();
        if(isDaily) record.TodayCount++; //데일리 일때만 증가
        Manager.Save.SaveGame();

        _cachedDateById.Remove(id); //캐시 정리
    }
    //==================헬퍼======================================
    private static void EnsureDailyReset(RewardClaimRecord record, string today) //날짜 바뀌면 카운트 리셋
    {
        //빈 데이터 아니고 미래가 아니면
        if (!string.IsNullOrEmpty(record.LastDate) && string.Compare(record.LastDate, today) < 0)
            record.TodayCount = 0;
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
    //====================쿨타임 계산용=======================
    public static int GetRemainingCooldownSec(string id) //쿨타임 남은 시간(초) 반환
    {
        var record = GetOrCreateRecord(id); // 레코드 확보
        long now = GetNowUnix(); // 현재 시간
        int cooldownSec = Manager.Game.Config.GetCoolTimeByID(id);

        if (record.LastClaimUnix <= 0) return 0; // 첫 수령 전이면 즉시 가능

        long elapsed = now - record.LastClaimUnix; // 필요한 시간
        long remain = cooldownSec - elapsed; // 남은 시간(초)
        return (int)System.Math.Max(0, remain);
    }

    public static string FormatRemainingHMS(int remainSec) // HH:MM:SS 포맷
    {
        int h = remainSec / 3600;
        int m = (remainSec % 3600) / 60;
        int s = remainSec % 60; 
        return $"{h:00}:{m:00}:{s:00}";
    }
}
