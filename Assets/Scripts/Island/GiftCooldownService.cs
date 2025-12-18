using System;
using UnityEngine;

public class GiftCooldownService
{
    private float _cooldown; //쿨타임 초 단위

    public GiftCooldownService(float cooldown)
    {
        _cooldown = cooldown; //쿨타임 저장
    }

    public bool CanGiveGift(long giftCooldownStartTime)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //현재 UTC 시간
        return now - giftCooldownStartTime > _cooldown; //쿨타임 초과 여부 반환
    }

    public long RecordGiftTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //현재 시간 저장용 반환
    }
}
