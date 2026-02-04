using System;
using UnityEngine;

public static class EnergyRecoveryService
{
    public static int SyncNow() // 에너지 정산 후 증가량반환(증가 시에만 UI 갱신용)
    {
        if (Manager.Save == null || Manager.Save.CurrentData == null) return 0;
        if (Manager.Game == null || Manager.Game.Config == null) return 0;

        var user = Manager.Save.CurrentData.UserData;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (user.LastEnergyUnixTime == 0) //첫 계산이면 기준시각만 저장
        {
            user.LastEnergyUnixTime = now;
            return 0;
        }

        float recoverSec = Manager.Game.Config.EnergyRecoveringTime; //회복 주기
        if (recoverSec <= 0f)
        {
            user.LastEnergyUnixTime = now;
            return 0;
        }

        long elapsed = now - user.LastEnergyUnixTime; //경과시간
        if (elapsed <= 0) return 0;

        int gain = (int)(elapsed / recoverSec); // 증가 량
        if (gain <= 0) return 0; 

        int maxEnergy = Manager.Game.Config.MaxEnergy; 
        int before = user.Energy; // 실제 증가량 계산용(클램프 때문에)
        user.Energy = Mathf.Clamp(user.Energy + gain, 0, maxEnergy);

        user.LastEnergyUnixTime += (long)(gain * recoverSec);//쓴 시간만큼만 전진(잔여 초 유지)

        return user.Energy - before; //실제 증가한 양 반환
    }
}
