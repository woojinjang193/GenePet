using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private IslandPetVisualController _visualController;

    [Header("비주얼로더")]
    [SerializeField] private IslandPetVisualLoader _visualLoader;
    [SerializeField] private IslandPetVisualLoader _myPetVisualLoader;

    [Header("섬 세팅")]
    [SerializeField] private GameObject _islandPet;
    [SerializeField] private GameObject _letter;
    [SerializeField] private EggObj _egg;

    [Header("알 낳는 호감도")]
    [SerializeField] private int _LayEggAt = 100;
    [Header("떠나는 호감도")]
    [SerializeField] private int _leaveAt = -50;
    [Header("방문 보상 받는 청결도 기준")]
    [SerializeField] private int _getPointCleanlinessAbove = 50;

    [Header("UI")]
    [SerializeField] private Button _goBackHomeButton;
    [SerializeField] private GeneInfomationUI _GeneInfoUI;
    [SerializeField] private GameObject _geneInfoButton;

    [Header("플러스 아이콘")]
    [SerializeField] private GameObject _plusIcon;

    public Action OnIslandMyPetChange;

    public string IslandMyPetID { get; private set; }

    public PetSaveData IslandMypetData { get; private set; }
    public PetSaveData IslandPetData { get; private set; }

    private PetBreed _breedManager;

    private float _visitingPoint;
    private bool _isMarried;
    private bool _isLeft;

    public bool IsMarried => _isMarried;
    public bool IsLeft => _isLeft;

    private void Awake()
    {
        _visitingPoint = Manager.Game.Config.VisitingAffinityGain;
        _isMarried = Manager.Save.CurrentData.UserData.Island.IsMarried;
        _isLeft = Manager.Save.CurrentData.UserData.Island.IsLeft;
        _breedManager = GetComponent<PetBreed>();
        _goBackHomeButton.onClick.AddListener(OnClickedGoHome);

        TrySpawnMyPet();

        if (_isLeft)
        {
            LeaveIslandPet(); // 없앨지 고민중
            return;
        }
        else if (_isMarried)
        {
            LayEggAndLeave();
            return;
        }
        SpawnIslandPet();
        VisitReward();
    }
    private void OnClickedGoHome()
    {
        SceneManager.LoadScene("InGameScene");
    }
    public void LeaveIslandPet() //섬펫 떠남처리
    {
        _islandPet.SetActive(false);
        _letter.SetActive(true);
    }
    public void LayEggAndLeave()
    {
        _islandPet.SetActive(false);
        _letter.SetActive(false);

        if (IslandPetData == null || IslandMypetData == null)
        {
            Debug.Log("펫 데이터 부족으로 교배 못함");
            return;
        }

        EggData egg = TryToBreed();
        if (egg != null)
        {
            _egg.gameObject.SetActive(true);
            _geneInfoButton.SetActive(false); //유전자 정보 버튼 끔
            _egg.Init(egg);
        }
    }
    private void VisitReward()
    {
        if (string.IsNullOrWhiteSpace(IslandMyPetID) || IslandMypetData.IsLeft)
        {
            Debug.Log("섬펫 없어서 호감도 안오름");
            return;
        }

        if (!_isLeft && !_isMarried)
        {
            if (Manager.Save.CurrentData.UserData.Island.Affinity >= _LayEggAt)
            {
                LayEggAndLeave();
                _isMarried = true;
                return;
            }
        }

        if (!CanGetReward()) { return; } //쿨타임 돌았는지 확인

        //방문시 호감도 증가
        if (IslandMypetData.Cleanliness > _getPointCleanlinessAbove) //방문시 청결도가 _getPointAbove 위일 경우 방문점수 받음
        {
            ChangeAffinity(_visitingPoint);
            Debug.Log($"청결도:{IslandMypetData.Cleanliness}. 호감도 {_visitingPoint}증가");
        }
    }
    private void SpawnIslandPet()
    {
        if (string.IsNullOrWhiteSpace(Manager.Save.CurrentData.UserData.Island.IslandPetSaveData.ID))
        {
            Debug.Log("섬펫 정보 없음. 랜덤펫 생성");
            Manager.Game.CreateRandomPet(false);
        }
        var data = Manager.Save.CurrentData.UserData.Island.IslandPetSaveData;
        IslandPetData = data;
        _visualLoader.LoadIslandPet(data);
    }
    public void TrySpawnMyPet() //전에 설정해놓은 펫 있으면 그걸로 소환, 없으면 소환 안함
    {
        IslandMyPetID = Manager.Save.CurrentData.UserData.Island.IslandMyPetID;
        //bool canSpawn = false;

        if (string.IsNullOrWhiteSpace(IslandMyPetID))
        {
            Debug.Log("세팅된 펫 없음");
            return;
        }

        var petList = Manager.Save.CurrentData.UserData.HavePetList;
        foreach (PetSaveData pet in petList)
        {
            if (pet.ID == IslandMyPetID) //아이디 일치하고 안떠난 상태라면
            {
                //canSpawn = true;
                IslandMypetData = pet;
                break;
            }
        }

        if (!IslandMypetData.IsLeft) //소환 가능한 상태라면
        {
            _myPetVisualLoader.LoadIslandPet(IslandMypetData);
            Debug.Log("저장된 마이펫 소환");
        }
        else
        {
            _myPetVisualLoader.LoadIslandPet(null); // 떠난 상태라면
        }

        _plusIcon.SetActive(false); //플러스 아이콘 꺼줌
    }
    // ================== 호감도 변경 ========================
    public void ChangeAffinity(float amount)
    {
        float beforeAffinity = Manager.Save.CurrentData.UserData.Island.Affinity; //전 호감도
        float newValue = Manager.Save.CurrentData.UserData.Island.Affinity += amount; //변화한 호감도

        // 호감도 변화 연출함수 호출
        bool isUp = false;
        if (beforeAffinity < newValue) isUp = true;

        _visualController.ShowAffinityChangeEffect(isUp);

        CheckMarryOrLeave(newValue); // 떠남/알남 체크
        Debug.Log($"호감도 변동:{amount}. 현재 호감도 {Manager.Save.CurrentData.UserData.Island.Affinity}");
    }
    // ================ 외부 호출 ==================
    public void UpdateIslandMyPetID(PetSaveData data)
    {
        Manager.Save.CurrentData.UserData.Island.Affinity = 0; //호감도 초기화
        Manager.Save.CurrentData.UserData.Island.LastVisitTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //방문시간 초기화
        Debug.Log("호감도, 방문시간 초기화");

        IslandMypetData = data;
        IslandMyPetID = data.ID;

        OnIslandMyPetChange?.Invoke();
    }
    // ====================버튼으로 호출 =========================
    public void OpenGeneInfo() //유전자 정보 UI (섬펫용)
    {
        if (IslandPetData == null) { return; }

        _GeneInfoUI.gameObject.SetActive(true);
        _GeneInfoUI.Init(IslandPetData);
    }
    public void OpenGeneInfoForMyPet()  //유전자 정보 UI (마이펫용)
    {
        if (IslandMypetData == null || IslandMypetData.IsLeft) { return; }

        _GeneInfoUI.gameObject.SetActive(true);
        _GeneInfoUI.Init(IslandMypetData);
    }
    //======== 교배시도=============
    public EggData TryToBreed()
    {
        if (IslandPetData == null || IslandMypetData == null)
        {
            Debug.LogError("펫 데이터 부족으로 교배 못함");
            return null;
        }

        var egg = _breedManager.BreedPet(IslandMypetData, IslandPetData);
        if (egg != null)
        {
            Manager.Save.RecordIslandPet(IslandPetData);
            //Manager.Save.RemoveIslandPet();
        }
        return egg;
    }
    // ============= 보상 쿨타임 경과 체크 =================
    private bool CanGetReward()
    {
        var coolTime = Manager.Game.Config.VisitingAffinityCooldown;
        var last = Manager.Save.CurrentData.UserData.Island.LastVisitTime;
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        int offlineSec = (int)(now - last);

        if (offlineSec > coolTime) //쿨타임 지났으면
        {
            Manager.Save.CurrentData.UserData.Island.LastVisitTime = now; //지금 시간 마지막 방문 시간으로 설정
            return true;
        }

        return false;
    }
    // ==============호감도 갱신후 섬펫 상태 체크=============
    private void CheckMarryOrLeave(float newValue)
    {
        if (newValue >= _LayEggAt) //알 낳기 충분한 호감도면
        {
            _isMarried = true;
            _visualController.ShowLayOrLeaveEffect(true);
            LayEggAndLeave();
        }

        if (newValue <= _leaveAt) //떠날 호감도면
        {
            _isLeft = true;
            _visualController.ShowLayOrLeaveEffect(false);
            LeaveIslandPet();
        }
    }
}
