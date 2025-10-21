using System;
using UnityEngine;

public class PetUnit : MonoBehaviour
{
    [Header("성장단계별 configSO")]
    [SerializeField] private PetConfigSO[] _configs;

    [Header("현재 상태")]
    [SerializeField] private PetStatusCore _status = new PetStatusCore();

    [Header("틱 설정")]
    [SerializeField] private float _tickInterval = 1f;

    [Header("편지")]
    [SerializeField] private GameObject _letter;

    public PetStatusCore Status => _status;
    public PetConfigSO CurrentConfig { get; private set; }

    private float _accum;
 

    public int _unHappyScore = 0;
    public int UnHappyScore => _unHappyScore;

    private void Awake()
    {
        ApplyConfigFor(_status.Growth);
    }

    private void OnEnable()
    {
        _status.OnGrowthChanged += ApplyConfigFor;
        LoadFromSaveData(Manager.Save.CurrentData.UserData.HavePet);
    }

    private void OnDisable()
    {
        _status.OnGrowthChanged -= ApplyConfigFor;
    }

    private void LoadFromSaveData(PetSaveData data)
    {
        if (data == null)
            return;

        _status.SetStat(PetStat.Hunger, data.Hunger);
        _status.SetStat(PetStat.Happiness, data.Happiness);
        _status.SetStat(PetStat.Energy, data.Energy);
        _status.SetStat(PetStat.Cleanliness, data.Cleanliness);
        _status.SetStat(PetStat.Health, data.Health);

        _status.Growth = Enum.TryParse<GrowthStatus>(data.GrowthStage, out var growthStatus) ? growthStatus : GrowthStatus.Egg;
    }

    private void ApplyConfigFor(GrowthStatus growth)
    {
        CurrentConfig = FindConfig(growth);
        if (CurrentConfig == null)
        {
            Debug.LogWarning($"[{growth}] PetConfigSO를 찾지 못함");
        }
        else
        {
            Debug.Log($"[{growth}] PetConfigSO로 변경");
            _unHappyScore = 0;
        }
    }

    private PetConfigSO FindConfig(GrowthStatus growth)
    {
        if (_configs == null)
        {
            return null;
        }
        for (int i = 0; i < _configs.Length; i++)
        {
            var config = _configs[i];
            if (config != null && config.GrowthType == growth)
            {
                return config;
            }
        }
        return null;
    }

    private void Update()
    {
        //이미 떠났으면
        if (_status.GetFlag(PetFlag.IsLeft))
        {
            return;
        }

        if (CurrentConfig == null)
        {
            return;
        }

        _accum += Time.deltaTime;
        while (_accum >= _tickInterval)
        {
            TickOnce();
            _accum -= _tickInterval;
        }
    }
    public void AddExp(float amount)
    {
        _status.AddStat(PetStat.GrowthExp, amount);
    }

    private void TickOnce() //지속적인 스텟변경
    {
        //배고픔 감소
        _status.AddStat(PetStat.Hunger, -CurrentConfig.HungerDecreasePerSec);
        //청결도 감소
        _status.AddStat(PetStat.Cleanliness, -CurrentConfig.CleanlinessDecreasePerSec);

        //=====잘때=====
        if (_status.GetFlag(PetFlag.IsSleeping))
        {
            //에너지 증가
            _status.AddStat(PetStat.Energy, CurrentConfig.SleepEnergyGainPerSec);
        }

        //=====아플때=====
        if (_status.GetFlag(PetFlag.IsSick))
        {
            //체력 감소
            _status.AddStat(PetStat.Health, -CurrentConfig.HealthDecreasePerSec);
            //행복도 감소
            _status.AddStat(PetStat.Happiness, -CurrentConfig.HappinessDecreasePerSec);
            //불행점수 증가 
            _unHappyScore += 1;
            Debug.Log($"아픔. 현재 건강 :{_status.GetStat(PetStat.Health)}, 행복도 : {_status.GetStat(PetStat.Happiness)}");
        }

        //=====배 많이 고플때=====
        if (_status.GetStat(PetStat.Hunger) <= 0f)
        {
            _status.AddStat(PetStat.Health, -CurrentConfig.HealthDecreasePerSec);
            _status.AddStat(PetStat.Happiness, -CurrentConfig.HappinessDecreasePerSec * 1.2f);
            _unHappyScore += 1;
            Debug.Log($"배고픔. 체력: {_status.GetStat(PetStat.Health)} 행복도 : {_status.GetStat(PetStat.Happiness)}");
        }
        else if (_status.GetStat(PetStat.Hunger) < 20f)
        {
            //행복도 감소
            _status.AddStat(PetStat.Happiness, -CurrentConfig.HappinessDecreasePerSec);
            Debug.Log($"배고픔. 행복도 : {_status.GetStat(PetStat.Happiness)}");
        }
        

        //=====행복도가 0일때=====
        if (_status.GetStat(PetStat.Happiness) <= 0f)
        {
            _status.AddStat(PetStat.Health, -CurrentConfig.HealthDecreasePerSec);
            _status.AddStat(PetStat.Happiness, -CurrentConfig.HappinessDecreasePerSec);
            _unHappyScore += 1;
        }

        //=====에너지가 0일때=====
        if (_status.GetStat(PetStat.Energy) <= 0f)
        {
            _status.AddStat(PetStat.Health, -CurrentConfig.HealthDecreasePerSec);
            _status.AddStat(PetStat.Happiness, -CurrentConfig.HappinessDecreasePerSec);
            _unHappyScore += 1;
        }

        //=====청결도가 0일때=====
        if (_status.GetStat(PetStat.Cleanliness) <= 0f)
        {
            _status.AddStat(PetStat.Health, -CurrentConfig.HealthDecreasePerSec);
            _status.AddStat(PetStat.Happiness, -CurrentConfig.HappinessDecreasePerSec);
            _unHappyScore += 1;
        }

        if (CurrentConfig.canGrow)
        {
            _status.AddStat(PetStat.GrowthTimer, 1);

            if (_status.GetStat(PetStat.GrowthTimer) >= CurrentConfig.TimeToGrow && _status.GetStat(PetStat.GrowthExp) >= CurrentConfig.ExpToGrow)
            {
                EvolveToNextStage();
            }
        }

        if(_status.GetStat(PetStat.Health) <= 0f)
        {
            _status.SetFlag(PetFlag.IsLeft, true);
            _letter.SetActive(true);
            Manager.Game.SetLeftReason(FindReason());
            transform.position = new Vector3(-10, 0, 0);
            Debug.Log("펫 떠남");
        }
    }
    private void EvolveToNextStage()
    {
        GrowthStatus next = GetNextGrowth(_status.Growth);

        if (next == _status.Growth)
        {
            Debug.Log($"[{name}] 더 이상 성장할 단계 없음");
            return;
        }

        _status.Growth = next;
        _status.SetStat(PetStat.GrowthTimer, 0);
        _status.SetStat(PetStat.GrowthExp, 0);
        Debug.Log($"[{name}] 성장! >> {next}");
    }

    private GrowthStatus GetNextGrowth(GrowthStatus current)
    {
        switch (current)
        {
            case GrowthStatus.Egg: 
                return GrowthStatus.Baby;

            case GrowthStatus.Baby:
                if (_unHappyScore >= 100)
                {
                    return GrowthStatus.Teen_Rebel;
                }
                else
                {
                    return GrowthStatus.Teen;
                }

            case GrowthStatus.Teen: 
                return GrowthStatus.Adult;

            case GrowthStatus.Teen_Rebel: 
                return GrowthStatus.Adult;

            default: 
                return current;
        }
    }
    public void UpdatePetSaveData(PetSaveData data)
    {
        data.Hunger = _status.GetStat(PetStat.Hunger);
        data.Happiness = _status.GetStat(PetStat.Happiness);
        data.Energy = _status.GetStat(PetStat.Energy);
        data.Cleanliness = _status.GetStat(PetStat.Cleanliness);
        data.Health = _status.GetStat(PetStat.Health);
        data.GrowthStage = _status.Growth.ToString();
    }

    private LeftReason FindReason()
    {
        if(_status.GetStat(PetStat.Hunger) <= 0f)
        {
            return LeftReason.Hunger;
        }
        if (_status.GetStat(PetStat.Cleanliness) <= 0f)
        {
            return LeftReason.Dirty;
        }
        if (_status.GetStat(PetStat.Happiness) <= 0f)
        {
            return LeftReason.Unhappy;
        }
        return LeftReason.Unhappy;
    }    
}
