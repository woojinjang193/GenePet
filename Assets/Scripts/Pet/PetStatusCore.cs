using System;
using UnityEngine;

[Serializable]
public class PetStatusCore
{
    public string ID { get; private set; }
    public float Hunger { get; private set; }
    public float Health { get; private set; }
    public float Cleanliness { get; private set; }
    public float Happiness { get; private set; }
    public bool IsSick { get; private set; }
    public bool IsLeft { get; private set; }

    private GrowthStatus _growth = GrowthStatus.Egg;

    private PetConfigSO _config;

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
        }
    }
 
    public void Tick(float sec)
    {
        if (_config == null) return;
        if (sec <= 0f) return;

        Hunger -= _config.HungerDecreasePerSec * sec;
        Cleanliness -= _config.CleanlinessDecreasePerSec * sec;

        _growthTimer += sec;

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

    public void SetValues(string id, float hunger, float health, float cleanliness, float happiness)
    {
        ID = id;
        Hunger = hunger;
        Health = health;
        Cleanliness = cleanliness;
        Happiness = happiness;
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

    public void SaveStatus()
    {
        foreach(var pet in Manager.Save.CurrentData.UserData.HavePetList)
        {
            if(pet.ID == ID)
            {
                pet.Hunger = Hunger;
                pet.Health = Health;
                pet.Cleanliness = Cleanliness;
                pet.Happiness = Happiness;
                pet.IsLeft = IsLeft;
                pet.IsSick = IsSick;
                pet.GrowthStage = _growth;
                break;
            }
        }
    }
    public void ResetGrowthProgress()
    {
        _growthTimer = 0f;
        _growthExp = 0f;
    }
}
