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
    private float _energyRecoveringTime; // 에너지 1 오르는 시간
    private int _maxEnergy; //유저 맥스 에너지

    private CameraController _camera;
    private InGameUIManager _uiManager;
    private List<PetUnit> _activePets = new List<PetUnit>();
    public List<PetUnit> ActivePets => _activePets;
    public PetSaveData ZoomedPet { get; private set; }
    public PetUnit ZoomedUnit { get; private set; }

    private LetterPanel _letterPanel; //이벤트 구독용
    private Dictionary<GrowthStatus, PetConfigSO> _configMap = new Dictionary<GrowthStatus, PetConfigSO>();

    public event Action OnPetSpawned;
    public event Action OnPetRemoved;

    public event Action<PetUnit> OnPetComeBack;
    public event Action<PetUnit> OnPetLeft;
    private void Awake()
    {
        _accum = 0f;
        _camera = FindObjectOfType<CameraController>();
        _uiManager = FindObjectOfType<InGameUIManager>();
        _energyRecoveringTime = Manager.Game.Config.EnergyRecoveringTime;
        _maxEnergy = Manager.Game.Config.MaxEnergy;

        foreach (var cfg in _configs)
        {
            if (cfg != null)
            {
                _configMap[cfg.GrowthType] = cfg;
            }
        }

        LoadPetListFromSave(); //펫 데이터 로드
        ApplyOfflineTimeFromSave(); //오프라인 시간 적용
    }
    private void OnEnable()
    {
        _letterPanel = FindObjectOfType<LetterPanel>(true);
        _letterPanel.OnClickMissingPoster += PetComeBack;

        Manager.Save.OnAppPaused += OnAppPaused; //백그라운드시 저장용
    }
    private void OnDisable()
    {
        if(Manager.Save == null)
        {
            Debug.LogWarning("[PetManager] 세이브 매니저 없어서 저장 안함 (종료일땐 괜찮음)");
            return;
        }

        // 메인 씬을 떠날때 시간, 펫상태 저장
        SaveAllStatus();
        Manager.Save.SaveMainSceneLeaveTime();
        Debug.Log("메인씬 떠남저장 완료");

        // 상태 저장
        Manager.Save.SaveGame();

        //-----------이벤트 해제-------------------
        if (_letterPanel != null)
            _letterPanel.OnClickMissingPoster -= PetComeBack;

        Manager.Save.OnAppPaused -= OnAppPaused;

    }
    private void OnApplicationPause(bool pause)
    {
        // 복귀 시 오프라인 시간 적용
        if (!pause)
            ApplyOfflineTimeFromSave();
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
        if (save == null) return;

        if (string.IsNullOrWhiteSpace(save.ID))
        {
            var list = Manager.Save?.CurrentData?.UserData?.HavePetList; //세이브 리스트 참조
            string newId;

            do
            {
                newId = Guid.NewGuid().ToString(); // [추가] 새 GUID 생성
            }
            while (list != null && list.Exists(pet => pet != null && pet != save && pet.ID == newId)); //충돌 방지(만약을 위해)

            save.ID = newId; // 저장데이터에 ID 할당
            Debug.LogWarning($"PetSaveData.ID가 비어있어서 새 GUID 할당: {save.ID}");
        }

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

        if (save.GrowthStage == GrowthStatus.Egg) //새로 스폰된 펫일 경우
        {
            _camera.CameraMoveTo(_Positions[index].position); //카메라 이동
        }
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

            if (_activePets != null && _activePets.Count > 0) //펫이 있다면
            {
                RunTick(_tickInterval); // 틱
            }
            
            if (ZoomedUnit != null) // 줌된 펫 있을 때만
            {
                RequestGaugeUpdate(); // UI 갱신
            }
            _accum -= _tickInterval; //타이머 1초 빼기
        }     
    }
    private void RunTick(float sec)
    {
        if (_activePets == null || _activePets.Count <= 0) return; 

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
        if(_energyRecoveringTime <= 0f) return;

        _energyTimer += sec; // 시간 누적

        if (_energyTimer >= _energyRecoveringTime)
        {
            int amount = (int)(_energyTimer / _energyRecoveringTime); // 오를 수 있는 양 계산
            _energyTimer %= _energyRecoveringTime; // 남은 시간만 저장

            AddEnergy(amount); // 실제 증가 처리
        }
    }

    private void AddEnergy(int amount)
    {
        var user = Manager.Save.CurrentData.UserData;

        user.Energy = Mathf.Clamp(user.Energy + amount, 0, _maxEnergy);

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

            // ID 비어있으면 저장 매칭 불가 > 즉시 스킵 + 에러 로그
            if (string.IsNullOrEmpty(unit.PetId))
            {
                Debug.LogError($"[SaveAllStatus] PetId가 비어있어 저장 불가. obj={unit.name}, inst={unit.GetInstanceID()}");
                continue;
            }

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
    //======================펫 줌 인/아웃==================================
    public void ZoomInPet(PetUnit unit)
    {
        if (unit == null) return;

        string id = unit.PetId;

        if (string.IsNullOrWhiteSpace(id)) //id 유무 검사
        {
            Debug.LogError($"[ZoomInPet] PetId가 비어있음. obj={unit.name}, inst={unit.GetInstanceID()}");
            return;
        }
        if (ZoomedPet != null) return;

        ZoomedUnit = unit;
        ZoomedUnit.ZoomThisPet(true);

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

        if (ZoomedPet == null)
        {
            Debug.LogError($"[ZoomInPet] SaveList에서 ID '{id}' 를 찾지 못함. obj={unit.name}");
            ZoomedUnit.ZoomThisPet(false); //줌 표시 롤백
            ZoomedUnit = null; //상태 롤백
            return;
        }

        //카메라 줌인
        for (int i = 0; i < _activePets.Count; i++)
        {
            var pet = _activePets[i];

            if (pet.PetId == id)
            {
                Vector3 pos = pet.gameObject.transform.position;
                _camera.CameraZoomIn(pos, pet.gameObject);
                if (_uiManager != null) _uiManager.OnZoomInPet();
                break;
            }
        }

        _camera.SetBackGround(ZoomedPet.RoomType); //배경정보 넘겨줌
        RequestGaugeUpdate();
    }
    public void ZoomOutPet()
    {
        if (_camera != null)
        {
            _camera.CameraZoomOut(); // 카메라 원상 복귀
        }

        ZoomedUnit.ZoomThisPet(false);
        
        if (_uiManager != null)
        {
            _uiManager.OnZoomOutPet(); // UI 버튼 비활성화
        }

        ZoomedPet = null;
        ZoomedUnit = null;
    }
    private void ApplyOfflineTimeFromSave()
    {
        long last = Manager.Save.CurrentData.UserData.LastPetSavedUnixTime;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if(last == 0) //첫 사용자면
        {
            Manager.Save.CurrentData.UserData.LastPetSavedUnixTime = now; //세이브에 현재시간 마지막 메인씬 시간으로 기록
            return;
        }

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

        RecoverEnergy(offlineSec);
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
        RequestGaugeUpdate();
    }
    // =================펫 떠남처리===========================
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

        OnPetLeft?.Invoke(pet);
    }
    //==================================펫 돌아왔을때 처리===============================================
    private void PetComeBack()
    {
        if(ZoomedUnit == null) return;

        GameConfig gameConfig = Manager.Game.Config;

        ZoomedUnit.gameObject.TryGetComponent<PetVisualController>(out PetVisualController petvisul); //비주얼 로더 찾기

        if (petvisul)
        {
            ZoomedUnit.Status.SetValues(PetStat.Hunger, gameConfig.ComeBackHunger);
            ZoomedUnit.Status.SetValues(PetStat.Cleanliness, gameConfig.ComeBackCleanliness);
            ZoomedUnit.Status.DecreaseStat(PetStat.Happiness, gameConfig.ComeBackHappiness);
            ZoomedUnit.Status.SetValues(PetStat.Health, gameConfig.ComeBackHealth);
            ZoomedUnit.Status.SetFlag(PetFlag.IsLeft, false);
            ZoomedUnit.LeftHandled = false;

            SaveAllStatus();//세이브 데이터에 저장

            petvisul.SetSprite(ZoomedUnit.Status.Growth);
            RequestGaugeUpdate();

            OnPetComeBack?.Invoke(ZoomedUnit);
        }
        else
        {
            Debug.LogError("펫 비주얼 컨트롤러 못찾음");
        }
    }
    private LeftReason FineReasonForLeaving(PetStatusCore stats) //TODO: 떠남조건 확인하기
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
    }

    //게이지 업데이트 요청하는 유틸
    private void RequestGaugeUpdate()
    {
        if (ZoomedUnit == null) return;

        GrowthStatus growth = ZoomedUnit.Status.Growth;
        float requiredEXP = _configs[(int)growth].ExpToGrow;
        _StatusUI.UpdateGauges(ZoomedUnit.Status, requiredEXP);
    }

    //==================부스터 적용 유틸 함수=================
    public bool ApplyGrowthBooster(PetUnit unit) // 부스터 적용(성장+config+UI)
    {
        if (unit == null) return false; // null 방어

        bool grown = unit.ForceGrowOneStage(); // 강제 성장
        if (!grown) return false; // 실패면 중단

        if (_configMap.TryGetValue(unit.Status.Growth, out var cfg)) //새 성장단계 config 적용
        {
            unit.SetConfig(cfg); // 스탯 감소/증가 속도 등 갱신
        }

        if (ZoomedUnit == unit) RequestGaugeUpdate(); // 줌중이면 게이지 갱신

        return true; // 성공
    }
    //앱 백그라운드 이벤트로 호출
    private void OnAppPaused()
    {
        SaveAllStatus();
        Manager.Save.SaveMainSceneLeaveTime();
    }
}
