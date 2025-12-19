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

    public event Action<float> OnCleanlinessChanged;
    public event Action<bool> OnSick;
    public event Action<bool> OnHealthReducing;

    private bool _wasHealthReducing = false;


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

        bool isReducing = false;

        //성장 타이머
        _growthTimer += sec;

        //기본 감소
        Hunger -= _config.HungerDecreasePerSec * sec;
        Cleanliness -= _config.CleanlinessDecreasePerSec * sec;

        //체력 감소 조건
        if (Hunger < 0f)
        {
            Health -= _config.HealthDecreasePerSec * sec;
            isReducing = true;
        }
        if(IsSick)
        {
            Health -= _config.HealthDecreasePerSec * sec;
            isReducing = true;
        }

        if (_wasHealthReducing != isReducing) // 상태 변화가 일어났을 때만 이벤트 발송 (PetUnit 에서 구독)
        {
            _wasHealthReducing = isReducing;
            OnHealthReducing?.Invoke(isReducing);
        }

        //청결도 조건
        if (Cleanliness < 50f)
        {
            OnCleanlinessChanged?.Invoke(Cleanliness); //PetUnit 에서 구독
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
                    OnSick?.Invoke(true); //자연병
                    Debug.Log("자연스럽게 아픔");
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
                OnCleanlinessChanged?.Invoke(Cleanliness);
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
                OnCleanlinessChanged?.Invoke(Cleanliness);
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
                OnCleanlinessChanged?.Invoke(Cleanliness);
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
                    OnSick?.Invoke(on);
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

    public void SetGrowthExp(float value)
    {
        _growthExp = value;
    }
}
