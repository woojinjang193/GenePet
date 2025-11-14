using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PetManager : MonoBehaviour
{
    [Header("펫 프리팹")]
    [SerializeField] private GameObject _petPrefab;

    [Header("성장 단계별 Config")]
    [SerializeField] private PetConfigSO[] _configs;

    [Header("Tick 설정")]
    [SerializeField] private float _tickInterval = 1f;

    private float _accum;
    private List<PetUnit> _activePets = new List<PetUnit>();

    private Dictionary<GrowthStatus, PetConfigSO> _configMap = new Dictionary<GrowthStatus, PetConfigSO>();

    private void Awake()
    {
        _accum = 0f;

        foreach (var cfg in _configs)
        {
            if (cfg != null)
            {
                _configMap[cfg.GrowthType] = cfg;
            }
        }

        LoadPetListFromSave();
    }

    private void LoadPetListFromSave()
    {
        var saveList = Manager.Save.CurrentData.UserData.HavePetList;

        if (saveList != null && saveList.Count > 0)
        {
            foreach (var pet in saveList)
            {
                SpawnPet(pet);
            }
        }
    }

    public void SpawnPet(PetSaveData save)
    {
        if (_petPrefab == null)
        {
            Debug.LogWarning("프리팹 없음");
            return;
        }
        PetUnit unit = Instantiate(_petPrefab).GetComponent<PetUnit>();
        unit.Init(save); //unit 초기화
        RegisterPet(unit); //펫 매니저에 등록
    }

    public void RegisterPet(PetUnit unit)
    {
        if (!_activePets.Contains(unit))
        {
            _activePets.Add(unit);

            if (_configMap.TryGetValue(unit.Status.Growth, out var cfg))
            {
                unit.SetConfig(cfg);
            }
        }
    }

    private void Update()
    {
        if (_activePets.Count == 0) return;

        _accum += Time.deltaTime;

        while (_accum >= _tickInterval)
        {
            RunTick(_tickInterval);
            _accum -= _tickInterval;
        }
    }
    private void RunTick(float sec)
    {
        for (int i = 0; i < _activePets.Count; i++)
        {
            var unit = _activePets[i];
            unit.Status.Tick(sec);

            if (unit.TryGrow()) //업데이트에서 체크하기 싫음
            {
                if (_configMap.TryGetValue(unit.Status.Growth, out var cfg))
                {
                    unit.SetConfig(cfg);
                }
            }
        }
    }

    public void ApplyOfflineTime(int offlineSec)
    {
        if (offlineSec <= 0) return;

        RunTick(offlineSec);
    }
}
