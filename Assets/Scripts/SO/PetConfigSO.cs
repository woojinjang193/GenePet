using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConfigSO", menuName = "SO/PetConfigSO")]
public class PetConfigSO : ScriptableObject
{
    [Header("성장상태")]
    public GrowthStatus GrowthType;

    [Header("성장가능 여부")]
    public bool canGrow;

    [Header("다음 성장까지 걸리는 시간")]
    public float TimeToGrow;

    [Header("다음 성장까지 필요한 경험치")]
    public float ExpToGrow;

    [Header("배고픔 초당 감소")]
    public float HungerDecreasePerSec;

    [Header("행복도 초당 감소")]
    public float HappinessDecreasePerSec;

    [Header("에너지 초당 감소")]
    public float EnergyDecreasePerSec;

    [Header("청결도 초당 감소")]
    public float CleanlinessDecreasePerSec;

    [Header("스트레스 초당 증가")]
    public float StressIncreasePerSec;

    [Header("스트레스 초당 감소")]
    public float StressDecreasePerSec;

    [Header("먹이주기시 오르는 배고픔 수치")]
    public float FeedHungerGain;

    [Header("놀아주기시 오르는 행복도 수치")]
    public float PlayHappinessGain;

    [Header("씻기시 오르는 청결도 수치")]
    public float CleanGain;

    [Header("잠자기시 오르는 에너지 초당 수치")]
    public float SleepEnergyGainPerSec;

    [Header("놀아주기시 에너지 비용")]
    public float PlayEnergyCost;

    [Header("치료시 체력회복량")]
    public float HealAmount;

    [Header("아플때 체력 초당 감소")]
    public float HealthDecreasePerSec;
}
