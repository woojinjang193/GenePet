using UnityEngine;

public class PetUnit : MonoBehaviour
{
    [Header("성장단계별 configSO")]
    [SerializeField] private PetConfigSO[] _configs;

    [Header("현재 상태")]
    [SerializeField] private PetStatusCore _status = new PetStatusCore();

    [Header("틱 설정")]
    [SerializeField] private bool _enableTick = true;
    [SerializeField] private float _tickInterval = 1f;

    public PetStatusCore Status => _status;
    public PetConfigSO CurrentConfig { get; private set; }

    private float _accum;

    private void Awake()
    {
        ApplyConfigFor(_status.Growth);
    }

    private void OnEnable()
    {
        _status.OnGrowthChanged += ApplyConfigFor;
    }

    private void OnDisable()
    {
        _status.OnGrowthChanged -= ApplyConfigFor;
    }

    private void ApplyConfigFor(GrowthStatus growth)
    {
        CurrentConfig = FindConfig(growth);
        if (CurrentConfig == null)
        {
            Debug.LogWarning($"[{growth} PetConfigSO를 찾지 못함");
        }
        else
        {
            Debug.Log($"[{growth} PetConfigSO로 변경");
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
        if (!_enableTick || CurrentConfig == null)
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

    private void TickOnce()
    {
        _status.AddStat(PetStat.Hunger, -CurrentConfig.HungerDecreasePerSec);
        _status.AddStat(PetStat.Cleanliness, -CurrentConfig.CleanlinessDecreasePerSec);

        if (_status.GetFlag(PetFlag.IsSleeping)) //잘때
        {
            _status.AddStat(PetStat.Energy, CurrentConfig.SleepEnergyGainPerSec);
        }
        else //안잘때
        {
            _status.AddStat(PetStat.Energy, -CurrentConfig.EnergyDecreasePerSec); // 에너지 지속감소 빼야하나?
        }

        if (_status.GetFlag(PetFlag.IsSick)) //아플때
        {
            _status.AddStat(PetStat.Health, -CurrentConfig.SickHealthTick);
            _status.AddStat(PetStat.Happiness, -CurrentConfig.HappinessDecreasePerSec);
            Debug.Log($"아픔. 현재 건강 :{_status.GetStat(PetStat.Health)}, 행복도 : {_status.GetStat(PetStat.Happiness)}");
        }

        if(_status.GetStat(PetStat.Hunger) < 20f) //배 많이 고프면
        {
            _status.AddStat(PetStat.Health, -CurrentConfig.HappinessDecreasePerSec);
            Debug.Log($"배고픔. 행복도 : {_status.GetStat(PetStat.Happiness)}");
        }

        if(_status.GetFlag(PetFlag.IsSick))
        {
            Debug.Log($"아픔 : {_status.GetFlag(PetFlag.IsSick)}");
        }
    }

    // 외부에서 성장 바꾸고 싶을 때 쓰는 헬퍼(원하면 안 써도 됨)
    public void SetGrowth(GrowthStatus next)
    {
        _status.Growth = next; // 이벤트 통해 자동으로 SO 교체됨
    }
}
