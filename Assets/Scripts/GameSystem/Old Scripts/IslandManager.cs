using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandManager : MonoBehaviour
{
    [Header("방문 점수")]
    [SerializeField] private int _visitingPoint;

    [Header("프리팹")]
    [SerializeField] private GameObject _petPrefab;
    [SerializeField] private Transform _petTransform;
    [Header("섬 세팅")]
    [SerializeField] private GameObject _islandPet;
    [SerializeField] private GameObject _letter;
    [SerializeField] private GameObject _egg;
    [Header("돌아가기 버튼")]
    [SerializeField] private Button _goBackHomeButton;

    private float _lastVisitTime;
    public string IslandMyPetID { get; private set; }
    
    private bool _isMarried;
    private bool _isLeft;

    private void Awake()
    {
        _isMarried = Manager.Save.CurrentData.UserData.Island.IsMarried;
        _isLeft = Manager.Save.CurrentData.UserData.Island.IsLeft;
        _goBackHomeButton.onClick.AddListener(OnClickedGoHome);

        if (_isLeft)
        {
            LeaveIslandPet();
            return;
        }
        else if (_isMarried)
        {
            LayEggAndLeave();
            return;
        }
        TrySpawnPet();

        VisitReward();
    }
    private void OnClickedGoHome()
    {
        SceneManager.LoadScene("InGameScene");
    }
    public void LeaveIslandPet()
    {
        _islandPet.SetActive(false);
        _letter.SetActive(true);
    }
    public void LayEggAndLeave()
    {
        _islandPet.SetActive(false);
        _letter.SetActive(false);
        _egg.SetActive(true);
    }
    private void VisitReward()
    {
        //시간 제한 둬야함
        if (!_isLeft && !_isMarried)
        {
            
            if (Manager.Save.CurrentData.UserData.Island.Affinity >= 100)
            {
                LayEgg();
                return;
            }
            //방문시 호감도 증가
            Manager.Save.CurrentData.UserData.Island.Affinity += _visitingPoint;

            Debug.Log($"방문 포인트 +{_visitingPoint}. 현재 호감도 {Manager.Save.CurrentData.UserData.Island.Affinity}");
        }
    }
    private void LayEgg()
    {
        _islandPet.SetActive(false);
        _egg.SetActive(true);
        _isMarried = true;
    }
    public void TrySpawnPet() //전에 설정해놓은 펫 있으면 그걸로 소환, 없으면 소환 안함
    {
        IslandMyPetID = Manager.Save.CurrentData.UserData.Island.IslandMyPetID;

        if (string.IsNullOrWhiteSpace(IslandMyPetID))
        {
            Debug.Log("세팅된 펫 없음");
            return;
        }

        var petList = Manager.Save.CurrentData.UserData.HavePetList;
        foreach (var pet in petList)
        {
            if (pet.ID == IslandMyPetID)
            {
                SpawnPet(pet);
                break;
            }
        }
    }
    private void SpawnPet(PetSaveData pet)
    {
        if (_petPrefab == null)
        {
            Debug.LogError("프리팹 없음");
            return;
        }

        PetUnit unit = Instantiate(_petPrefab, _petTransform).GetComponent<PetUnit>();

        unit.Init(pet); //unit 초기화

        PetVisualController visual = unit.GetComponent<PetVisualController>();
        if (visual != null)
        {
            visual.Init(pet, unit);
        }
    }

    public void AddAffinity(float amount)
    {
        Manager.Save.CurrentData.UserData.Island.Affinity += amount;
    }

}
