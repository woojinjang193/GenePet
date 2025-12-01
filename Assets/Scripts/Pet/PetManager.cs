using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PetManager : MonoBehaviour
{
    [Header("펫 프리팹")]
    [SerializeField] private GameObject _petPrefab;

    [Header("성장 단계별 Config")]
    [SerializeField] private PetConfigSO[] _configs;

    [Header("Tick 설정")]
    [SerializeField] private float _tickInterval = 1f;

    [Header("스폰 포지션")]
    [SerializeField] private Transform[] _Positions;

    [Header("스테이터스 UI")]
    [SerializeField] private float _statusUpdateDuration;
    [SerializeField] private StatusUI _StatusUI;

    private float _accum;
    private float _statusUpdateTimer;

    private CameraController _camera;
    private InGameUIManager _uiManager;
    private List<PetUnit> _activePets = new List<PetUnit>();
    public List<PetUnit> ActivePets => _activePets;

    public PetSaveData ZoomedPet { get; private set; }
    public PetUnit ZoomedUnit { get; private set; }

    private Dictionary<GrowthStatus, PetConfigSO> _configMap = new Dictionary<GrowthStatus, PetConfigSO>();

    private void Awake()
    {
        _accum = 0f;
        _camera = FindObjectOfType<CameraController>();
        _uiManager = FindObjectOfType<InGameUIManager>();

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
                //Debug.Log($"세이브에 있는 펫 {pet.ID} 스폰");
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

        int index = _activePets.Count;

        if (index >= _Positions.Length)
        {
            Debug.LogWarning("더이상 스폰 불가능. 최대 수에 도달");
        }

        PetUnit unit = Instantiate(_petPrefab, _Positions[index]).GetComponent<PetUnit>();

        unit.Init(save);

        PetVisualController visual = unit.GetComponent<PetVisualController>();
        if (visual != null)
        {
            visual.Init(save, unit);
        }

        RegisterPet(unit);
    }

    public void RegisterPet(PetUnit unit)
    {
        if (!_activePets.Contains(unit))
        {
            _activePets.Add(unit);

            if (_configMap.TryGetValue(unit.Status.Growth, out var cfg))
            {
                unit.SetConfig(cfg);
                Debug.Log($"{unit.Status.ID}의 성장 상태: {cfg.name}");
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

        _statusUpdateTimer += Time.deltaTime;

        if (ZoomedUnit != null && _statusUpdateTimer >= _statusUpdateDuration) // 줌된 펫 있을 때만
        {
            _StatusUI.UpdateGauges(ZoomedUnit.Status); // UI 갱신
            _statusUpdateTimer = 0f; // 타이머 리셋
        }
    }

    private void RunTick(float sec)
    {
        for (int i = 0; i < _activePets.Count; i++)
        {
            var unit = _activePets[i];
            unit.Status.Tick(sec);

            if (unit.TryGrow())
            {
                if (_configMap.TryGetValue(unit.Status.Growth, out var cfg))
                {
                    unit.SetConfig(cfg);
                }
            }
        }
    }

    private void OnDisable()
    {
        SaveAllStatus();
        Debug.Log("펫 스테이터스 저장 완료");
        Manager.Save.SaveGame();
    }

    private void SaveAllStatus()
    {
        if (Manager.Save.CurrentData == null)
        {
            Debug.LogWarning("변경된 데이터 없음"); return;
        }

        var saveList = Manager.Save.CurrentData.UserData.HavePetList;

        for (int i = 0; i < _activePets.Count; i++)
        {
            var unit = _activePets[i];
            var status = unit.Status;

            // 같은 ID 찾기
            for (int j = 0; j < saveList.Count; j++)
            {
                if (saveList[j].ID == status.ID)
                {
                    var pet = saveList[j];

                    pet.Hunger = status.Hunger; 
                    pet.Health = status.Health;
                    pet.Cleanliness = status.Cleanliness;
                    pet.Happiness = status.Happiness;

                    pet.IsLeft = status.IsLeft;
                    pet.IsSick = status.IsSick;
                    pet.GrowthStage = status.Growth;

                    break;
                }
            }
        }
    }

    public void ZoomInPet(PetUnit unit)
    {
        _statusUpdateTimer = 0f;

        ZoomedUnit = unit;

        string id = unit.PetId;

        if (ZoomedPet != null) return;

        if (_camera == null)
        {
            Debug.LogError("카메라 컨트롤러 없음");
            return;
        }

        //카메라 줌인
        for (int i = 0; i < _activePets.Count; i++)
        {
            var pet = _activePets[i];

            if (pet.Status.ID == id)
            {
                Vector3 pos = pet.gameObject.transform.position;
                _camera.CameraZoomIn(pos);
                if (_uiManager != null) _uiManager.OnZoomInPet();
                break;
            }
        }

        //선택된 펫 정보 캐싱
        var petlist = Manager.Save.CurrentData.UserData.HavePetList;
        for (int i = 0; i < petlist.Count; i++)
        {
            if (petlist[i].ID == id)
            {
                ZoomedPet = petlist[i];
                break;
            }
        }

        _StatusUI.UpdateGauges(ZoomedUnit.Status);
    }

    public void ZoomOutPet()
    {
        _statusUpdateTimer = 0f;

        if (_camera != null)
        {
            _camera.CameraZoomOut(); // 카메라 원상 복귀
        }

        ZoomedPet = null;
        ZoomedUnit = null;

        if (_uiManager != null)
        {
            _uiManager.OnZoomOutPet(); // UI 버튼 비활성화
        }
    }

    public void ApplyOfflineTime(int offlineSec)
    {
        if (offlineSec <= 0) return;

        RunTick(offlineSec);
    }

    public void RemovePet()
    {
        if (ZoomedPet.ID == ZoomedUnit.PetId )
        {
            Destroy(ZoomedUnit.gameObject);
            _activePets.Remove(ZoomedUnit);
            Manager.Save.RemovePetData(ZoomedPet.ID);
            ZoomOutPet();
            return;
        }
        //string targetID = ZoomedPet.ID;
        //
        //for (int i = 0; i < _activePets.Count; i++)
        //{
        //    var activePet = _activePets[i];
        //    
        //    if (activePet.PetId == targetID)
        //    {
        //        Destroy(activePet.gameObject);
        //        _activePets.RemoveAt(i);
        //        Manager.Save.RemovePetData(targetID);
        //        ZoomOutPet();
        //        return;
        //    }
        //}
    }
}
