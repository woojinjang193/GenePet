using System;
using UnityEngine;

[Serializable]
public class PetStatusCore
{
    public float Hunger { get; private set; }
    public float Health { get; private set; }
    public float Cleanliness { get; private set; }
    public float Happiness { get; private set; }
    public bool IsSick { get; private set; }
    public bool IsLeft { get; private set; }

    private GrowthStatus _growth;

    private PetConfigSO _config;

    private float _sickTimer;

    private float _growthTimer;
    private float _growthExp;

    public float GrowthTimer => _growthTimer;
    public float GrowthExp => _growthExp;

    public void SetConfig(PetConfigSO config)
    {
        _config = config;
    }
    public GrowthStatus Growth
    {
        get => _growth;
        set
        {
            if (_growth == value)
            {
                return;
            }
            _growth = value;
            Debug.Log($"ID: 성장 단계 {_growth}로 세팅");
        }
    }
    public void Tick(float sec)
    {
        if (IsLeft) return;
        if (_config == null) return;
        if (sec <= 0f) return;

        //성장 타이머
        _growthTimer += sec;

        //기본 감소
        Hunger -= _config.HungerDecreasePerSec * sec;
        Cleanliness -= _config.CleanlinessDecreasePerSec * sec;

        //체력 감소 조건
        if(Hunger < 0f)
        {
            Health -= _config.HealthDecreasePerSec * sec;
        }
        if(IsSick)
        {
            Health -= _config.HealthDecreasePerSec * sec;
        }

        //아픔 조건
        if(!IsSick)
        {
            if (Cleanliness < _config.CleanlinessAmountGetSick)
            {
                _sickTimer += sec;

                if (_sickTimer >= _config.TimeToGetSick)
                {
                    IsSick = true;
                    _sickTimer = 0f;
                }  
            }
            else
            {
                _sickTimer = 0f;
            }
        }

        //체력 증가 조건
        if(Hunger > _config.HungerAmountHealthIncrease)
        {
            if(Cleanliness > 90f)
            {
                Health += _config.HealthIncreasePerSec * sec * 1.5f; //깨끗하고 배부르면 더 빨리 오름
            }
            else
            {
                Health += _config.HealthIncreasePerSec * sec;
            }   
        }

        //떠남처리
        if (Health <= 0f) 
        {
            IsLeft = true;
        }

        Clamp();
    }
    public void IncreaseStat(PetStat stat, float value)
    {
        switch (stat)
        {
            case PetStat.Health:
                Health += value;
                break;
            case PetStat.Cleanliness:
                Cleanliness += value;
                break;
            case PetStat.Hunger:
                Hunger += value;
                break;
            case PetStat.Happiness:
                Happiness += value;
                break;
        }
        Clamp();
    }
    public void DecreaseStat(PetStat stat, float value)
    {
        switch (stat)
        {
            case PetStat.Health:
                Health -= value;
                break;
            case PetStat.Cleanliness:
                Cleanliness -= value;
                break;
            case PetStat.Hunger:
                Hunger -= value;
                break;
            case PetStat.Happiness:
                Happiness -= value;
                break;
        }
        Clamp();
    }
    public void SetValues(PetStat stat, float value)
    {
        switch (stat)
        {
            case PetStat.Health:
                Health = value;
                break;
            case PetStat.Cleanliness:
                Cleanliness = value;
                break;
            case PetStat.Hunger:
                Hunger = value;
                break;
            case PetStat.Happiness:
                Happiness = value;
                break;
        }
        Clamp();
    }
    public void SetFlag(PetFlag flag, bool on)
    {
        switch (flag)
        {
            case PetFlag.IsSick:
                if (IsSick != on) 
                {
                    IsSick = on; 
                }
                break;
            case PetFlag.IsLeft:
                if (IsLeft != on) 
                {
                    IsLeft = on; 
                }
                break;
        }
    }
    private void Clamp()
    {
        Hunger = Mathf.Clamp(Hunger, 0, 100);
        Health = Mathf.Clamp(Health, 0, 100);
        Cleanliness = Mathf.Clamp(Cleanliness, 0, 100);
        Happiness = Mathf.Clamp(Happiness, 0, 100);
    }
    public void ResetGrowthProgress()
    {
        float passedTime = _config.TimeToGrow;
        float reachedExp = _config.ExpToGrow;

        _growthTimer -= passedTime;
        _growthExp -= reachedExp;
    }
}
