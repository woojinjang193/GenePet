using System;
using UnityEngine;

public enum GrowthStatus { Egg, Baby, Teen, Teen_Rebel, Adult }
public enum PetStat { Hunger, Happiness, Energy, Cleanliness, Health, Stress }
public enum PetFlag { IsSick, IsSleeping, IsLeft } // 필요없을듯

[Serializable]
public class PetStatusCore
{
    private float _hunger = 100f;
    private float _happiness = 100f;
    private float _energy = 100f;
    private float _clean = 100f;
    private float _health = 100f;
    
    private bool _isSick = false;
    private bool _isSleeping = false;
    private bool _isLeft = false;
    
    private GrowthStatus _growth = GrowthStatus.Baby;

    public event Action<PetStat, float> OnStatChanged;
    public event Action<PetFlag, bool> OnFlagChanged;
    public event Action<GrowthStatus> OnGrowthChanged;

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
            OnGrowthChanged?.Invoke(_growth);
        }
    }

    public float GetStat(PetStat stat)
    {
        switch (stat)
        {
            case PetStat.Hunger: return _hunger;
            case PetStat.Happiness: return _happiness;
            case PetStat.Energy: return _energy;
            case PetStat.Cleanliness: return _clean;
            case PetStat.Health: return _health;
            default: return 0f;
        }
    }

    public void SetStat(PetStat stat, float value)
    {
        value = Clamp100(value);
        switch (stat)
        {
            case PetStat.Hunger: 
                if (_hunger != value) 
                { 
                    _hunger = value; OnStatChanged?.Invoke(stat, _hunger); 
                } 
                break;
            case PetStat.Happiness: 
                if (_happiness != value) 
                { 
                    _happiness = value; OnStatChanged?.Invoke(stat, _happiness); 
                } 
                break;
            case PetStat.Energy: 
                if (_energy != value) 
                { 
                    _energy = value; OnStatChanged?.Invoke(stat, _energy); 
                } 
                break;
            case PetStat.Cleanliness: 
                if (_clean != value) 
                { 
                    _clean = value; OnStatChanged?.Invoke(stat, _clean); 
                } 
                break;
            case PetStat.Health: 
                if (_health != value) 
                { 
                    _health = value; OnStatChanged?.Invoke(stat, _health); 
                }
                break;
        }
    }

    public void AddStat(PetStat stat, float value)
    {
        SetStat(stat, GetStat(stat) + value);
    }

    public bool GetFlag(PetFlag flag)
    {
        switch (flag)
        {
            case PetFlag.IsSick: 
                return _isSick;
            case PetFlag.IsSleeping: 
                return _isSleeping;
            case PetFlag.IsLeft: 
                return _isLeft;
            default: 
                return false;
        }
    }

    public void SetFlag(PetFlag flag, bool on)
    {
        switch (flag)
        {
            case PetFlag.IsSick:
                if (_isSick != on) 
                { 
                    _isSick = on; OnFlagChanged?.Invoke(flag, _isSick); 
                }
                break;
            case PetFlag.IsSleeping:
                if (_isSleeping != on) 
                { 
                    _isSleeping = on; OnFlagChanged?.Invoke(flag, _isSleeping); 
                }
                break;
            case PetFlag.IsLeft:
                if (_isLeft != on) 
                { 
                    _isLeft = on; OnFlagChanged?.Invoke(flag, _isLeft); 
                }
                break;
        }
    }

    private static float Clamp100(float v) => (v < 0f) ? 0f : (v > 100f ? 100f : v);
}
