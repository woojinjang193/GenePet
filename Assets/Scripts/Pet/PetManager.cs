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

    [Header("스폰 포지션")]
    [SerializeField] private Transform[] _Positions;

    [Header("줌아웃 버튼")]
    [SerializeField] private GameObject _zoomOutButton;

    private float _accum;
    private CameraController _camera;
    private List<PetUnit> _activePets = new List<PetUnit>();

    public PetSaveData ZoomedPet {  get; private set; }

    private Dictionary<GrowthStatus, PetConfigSO> _configMap = new Dictionary<GrowthStatus, PetConfigSO>();

    private void Awake()
    {
        _accum = 0f;
        _camera = FindObjectOfType<CameraController>();
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
                Debug.Log($"세이브에 있는 펫 {pet.ID} 스폰");
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

        if(index >= _Positions.Length)
        {
            Debug.LogWarning("더이상 스폰 불가능. 최대 수에 도달");
        }

        PetUnit unit = Instantiate(_petPrefab, _Positions[index]).GetComponent<PetUnit>();
        
        unit.Init(save); //unit 초기화

        PetVisualController visual = unit.GetComponent<PetVisualController>();
        if (visual != null)
        {
            visual.Init(save, unit);
        }

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

    public void ZoomInPet(string id)
    {
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
                _zoomOutButton.SetActive(true);
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
    }
    public void ZoomOutPet()
    {
        _zoomOutButton.SetActive(false);
        ZoomedPet = null;
    }
    public void ApplyOfflineTime(int offlineSec)
    {
        if (offlineSec <= 0) return;

        RunTick(offlineSec);
    }
}
