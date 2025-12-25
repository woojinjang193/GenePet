using System;
using System.Collections.Generic;
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

    [Header("스테이터스 UI")]
    [SerializeField] private float _statusUpdateDuration;
    [SerializeField] private StatusUI _StatusUI;

    private float _accum;
    private float _energyTimer; // 에너지 회복 누적시간
    private bool _isQuitting = false;

    private CameraController _camera;
    private InGameUIManager _uiManager;
    private List<PetUnit> _activePets = new List<PetUnit>();
    public List<PetUnit> ActivePets => _activePets;
    public PetSaveData ZoomedPet { get; private set; }
    public PetUnit ZoomedUnit { get; private set; }

    private LetterPanel _letterPanel; //이벤트 구독용
    private Dictionary<GrowthStatus, PetConfigSO> _configMap = new Dictionary<GrowthStatus, PetConfigSO>();

    public Action OnPetSpawned;
    public Action OnPetRemoved;
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
    private void Start()
    {
        _letterPanel = FindObjectOfType<LetterPanel>(true);
        _letterPanel.OnClickMissingPoster += PetComeBack;

        ApplyOfflineTimeFromSave();
    }
    private void OnDisable()
    {
        if (_isQuitting)
        {
            Debug.Log("종료중이라 리턴");
            return;
        }
        

        if (Manager.Save == null || Manager.Save.CurrentData == null || Manager.Save.CurrentData.UserData == null)
        {
            Debug.LogError("저장 불가: saveManager 준비 전");
            return;
        }

        SaveAllStatus();
        Debug.Log("펫 스테이터스 저장 완료");

        // 메인 씬을 떠나는 지금 시간기록
        Manager.Save.SavePlayTime();

        // 상태 + 시간까지 포함해서 저장
        Manager.Save.SaveGame();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("앱 백그라운드 전환. 즉시 저장");

            SaveAllStatus();
            Manager.Save.SavePlayTime();
            Manager.Save.SaveGame();

            return;
        }

        // 복귀 시 오프라인 시간 적용
        if (!pause)
            ApplyOfflineTimeFromSave();
    }


    private void OnDestroy()
    {
        _letterPanel.OnClickMissingPoster -= PetComeBack;
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
        int index = -1;

        if (_petPrefab == null)
        {
            Debug.LogWarning("프리팹 없음");
            return;
        }

        for (int i = 0; i < _Positions.Length; i++) //빈자리 확인
        {
            if(_Positions[i].childCount == 0)
            {
                index = i;
                break;
            }
        }

        if (index < 0 || index >= _Positions.Length)
        {
            Debug.LogWarning("스폰 불가능. 빈자리 없음");
            Debug.LogWarning("스폰 불가능. 빈자리 없음");
            return;
        }

        PetUnit unit = Instantiate(_petPrefab, _Positions[index]).GetComponent<PetUnit>();

        unit.Init(save, this);

        PetVisualController visual = unit.GetComponent<PetVisualController>();
        if (visual != null)
        {
            visual.Init(save, unit);
        }

        if (save.IsLeft) { PetLeft(unit); } //이미 떠난펫일경우 바로 떠남 처리

        RegisterPet(unit);

        OnPetSpawned?.Invoke();
    }
    public void RegisterPet(PetUnit unit)
    {
        if (!_activePets.Contains(unit))
        {
            _activePets.Add(unit);

            if (_configMap.TryGetValue(unit.Status.Growth, out var cfg))
            {
                unit.SetConfig(cfg);
                Debug.Log($"{unit.PetId}의 성장 상태: {cfg.name}");
            }
        }
    }
    private void Update()
    {
        _accum += Time.deltaTime; // 타이머

        while (_accum >= _tickInterval)
        {
            RecoverEnergy(_tickInterval); // 에너지 증가

            if (_activePets.Count > 0) //펫이 있다면
            {
                RunTick(_tickInterval); // 틱
            }
            
            if (ZoomedUnit != null) // 줌된 펫 있을 때만
            {
                _StatusUI.UpdateGauges(ZoomedUnit.Status); // UI 갱신
            }
            _accum -= _tickInterval; //타이머 1초 빼기
        }     
    }
    private void RunTick(float sec)
    {
        RecoverEnergy(sec); // 플레이어 에너지 회복

        if(_activePets.Count <= 0 || _activePets ==  null) return;

        for (int i = 0; i < _activePets.Count; i++)
        {
            var unit = _activePets[i];
            unit.Status.Tick(sec);

            if (unit.Status.IsLeft && !unit.LeftHandled)
            {
                //Debug.Log("펫떠남 호출 ");
                PetLeft(unit);
            }
            if (unit.TryGrow())
            {
                if (_configMap.TryGetValue(unit.Status.Growth, out var cfg))
                {
                    unit.SetConfig(cfg);
                }
            }
        }
    }

    private void RecoverEnergy(float sec)
    {
        _energyTimer += sec; // 시간 누적

        float need = Manager.Game.Config.EnergyRecoveringTime; // 1 오르는 시간

        if (_energyTimer >= need)
        {
            int amount = (int)(_energyTimer / need); // 오를 수 있는 양 계산
            _energyTimer %= need; // 남은 시간만 저장

            AddEnergy(amount); // 실제 증가 처리
        }
    }

    private void AddEnergy(int amount)
    {
        var user = Manager.Save.CurrentData.UserData;

        user.Energy = Mathf.Clamp(user.Energy + amount, 0, Manager.Game.Config.MaxEnergy);

        _uiManager.UpdateEnergyBar(user.Energy); // UI 갱신
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

            if (unit == null) continue;

            var status = unit.Status;

            // 같은 ID 찾기
            for (int j = 0; j < saveList.Count; j++)
            {
                if (saveList[j].ID == unit.PetId)
                {
                    var pet = saveList[j];

                    pet.Hunger = status.Hunger; 
                    pet.Health = status.Health;
                    pet.Cleanliness = status.Cleanliness;
                    pet.Happiness = status.Happiness;

                    pet.IsLeft = status.IsLeft;
                    pet.IsSick = status.IsSick;
                    pet.GrowthStage = status.Growth;
                    pet.GrowthExp = status.GrowthExp;

                    break;
                }
            }
        }

        Debug.Log("<color=green>펫 데이터 저장완료</color>");

    }
    public void ZoomInPet(PetUnit unit)
    {
        ZoomedUnit = unit;
        ZoomedUnit.ZoomThisPet(true);

        string id = unit.PetId;

        if (ZoomedPet != null) return;

        if (_camera == null)
        {
            Debug.LogError("카메라 컨트롤러 없음");
            return;
        }

        var petlist = Manager.Save.CurrentData.UserData.HavePetList;   //선택된 펫 정보 캐싱
        for (int i = 0; i < petlist.Count; i++)
        {
            if (petlist[i].ID == id)
            {
                ZoomedPet = petlist[i];
                break;
            }
        }

        //카메라 줌인
        for (int i = 0; i < _activePets.Count; i++)
        {
            var pet = _activePets[i];

            if (pet.PetId == id)
            {
                Vector3 pos = pet.gameObject.transform.position;
                _camera.CameraZoomIn(pos);
                if (_uiManager != null) _uiManager.OnZoomInPet();
                break;
            }
        }

        _camera.SetBackGround(ZoomedPet.RoomType); //배경정보 넘겨줌
        _StatusUI.UpdateGauges(ZoomedUnit.Status);
    }
    public void ZoomOutPet()
    {
        if (_camera != null)
        {
            _camera.CameraZoomOut(); // 카메라 원상 복귀
        }

        ZoomedUnit.ZoomThisPet(false);
        ZoomedPet = null;
        ZoomedUnit = null;

        if (_uiManager != null)
        {
            _uiManager.OnZoomOutPet(); // UI 버튼 비활성화
        }
    }
    private void ApplyOfflineTimeFromSave()
    {
        long last = Manager.Save.CurrentData.UserData.LastPlayedUnixTime;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        int offlineSec = (int)(now - last);

        if (offlineSec > 0)
        {
            ApplyOfflineTime(offlineSec);
        }

        // 이거 필요한가?
        //Manager.Save.CurrentData.UserData.LastPlayedUnixTime = now;
    }

    private void ApplyOfflineTime(int offlineSec)
    {
        Debug.Log($"오프라인 틱 적용양: {offlineSec}");
        if (offlineSec <= 0) return;

        RunTick(offlineSec);

        for (int i = 0; i < _activePets.Count; i++) //성장 가능하면 성장시킴
        {
            var unit = _activePets[i];

            // 성장할 수 있을 때까지 반복
            while (unit.TryGrow())
            {
                if (_configMap.TryGetValue(unit.Status.Growth, out var cfg))
                {
                    unit.SetConfig(cfg);
                }
            }
        }
    }
    public void RemovePet()
    {
        if (ZoomedPet.ID == ZoomedUnit.PetId )
        {
            Destroy(ZoomedUnit.gameObject);
            _activePets.Remove(ZoomedUnit);
            Manager.Save.RemovePetData(ZoomedPet.ID);
            ZoomOutPet();
            OnPetRemoved?.Invoke();
            return;
        }
    }
    public void UpdateStatus() //스테이터스 게이지 업데이트
    {
        _StatusUI.UpdateGauges(ZoomedUnit.Status);
    }
    private void PetLeft(PetUnit pet)
    {
        LeftReason reason = FineReasonForLeaving(pet.Status);

        pet.LeftHandled = true;

        Debug.Log("펫 떠남");

        pet.gameObject.TryGetComponent<PetVisualController>(out PetVisualController petvisul);

        if(petvisul)
        {
            petvisul.LetterOn(reason);
        }
        else
        {
            Debug.LogError("펫 비주얼 컨트롤러 못찾음");
        }
    }
    private void PetComeBack()
    {
        if(ZoomedUnit == null) return;

        GameConfig gameConfig = Manager.Game.Config;

        ZoomedUnit.gameObject.TryGetComponent<PetVisualController>(out PetVisualController petvisul); //비주얼 로더 찾기

        if (petvisul)
        {
            petvisul.SetSprite(ZoomedUnit.Status.Growth);

            ZoomedUnit.Status.SetValues(PetStat.Hunger, gameConfig.ComeBackHunger);
            ZoomedUnit.Status.SetValues(PetStat.Cleanliness, gameConfig.ComeBackCleanliness);
            ZoomedUnit.Status.DecreaseStat(PetStat.Happiness, gameConfig.ComeBackHappiness);
            ZoomedUnit.Status.SetValues(PetStat.Health, gameConfig.ComeBackHealth);
            _StatusUI.UpdateGauges(ZoomedUnit.Status);

            ZoomedUnit.Status.SetFlag(PetFlag.IsLeft, false);
            ZoomedUnit.LeftHandled = false;
        }
        else
        {
            Debug.LogError("펫 비주얼 컨트롤러 못찾음");
        }
    }
    private LeftReason FineReasonForLeaving(PetStatusCore stats)
    {
        if(stats.Hunger <= 0f)
        {
            return LeftReason.Hunger;
        }
        if (stats.Cleanliness <= 0f)
        {
            return LeftReason.Dirty;
        }
        if(stats.Happiness <= 0f)
        {
            return LeftReason.Unhappy;
        }
        if(stats.IsSick)
        {
            return LeftReason.Sick;
        }
        return LeftReason.NoReason;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("_isQuitting = true");

        SaveAllStatus();
        Manager.Save.SavePlayTime();
        Manager.Save.SaveGame();

        _isQuitting = true;
    }
}
