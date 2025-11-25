using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandManager : MonoBehaviour
{
    [Header("방문 점수")]
    [SerializeField] private int _visitingPoint;

    [Header("비주얼로더")]
    [SerializeField] private IslandPetVisualLoader _visualLoader;
    [SerializeField] private IslandPetVisualLoader _myPetVisualLoader;

    [Header("섬 세팅")]
    [SerializeField] private GameObject _islandPet;
    [SerializeField] private GameObject _letter;
    [SerializeField] private GameObject _egg;

    [Header("UI")]
    [SerializeField] private Button _goBackHomeButton;
    [SerializeField] private GeneInfomationUI _GeneInfoUI;

    private float _lastVisitTime;
    public string IslandMyPetID { get; private set; }

    private PetSaveData _islandMypetData;
    private PetSaveData _islandPetData;

    
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
        SpawnIslandPet();
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
    private void SpawnIslandPet()
    {
        if (string.IsNullOrWhiteSpace(Manager.Save.CurrentData.UserData.Island.IslandPetSaveData.ID))
        {
            Debug.Log("섬펫 정보 없음. 랜덤펫 생성");
            Manager.Game.CreateRandomPet(false);
        }
        var data = Manager.Save.CurrentData.UserData.Island.IslandPetSaveData;
        _islandPetData = data;
        _visualLoader.LoadIslandPet(data);
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
                _islandMypetData = pet;
                _myPetVisualLoader.LoadIslandPet(pet);
                Debug.Log("저장된 마이펫 소환");
                break;
            }
        }
    }
   
    public void AddAffinity(float amount)
    {
        Manager.Save.CurrentData.UserData.Island.Affinity += amount;
    }

    public void UpdateIslandMyPetID(PetSaveData data)
    {
        _islandMypetData = data;
        IslandMyPetID = data.ID;
    }

    public void OpenGeneInfo()
    {
        //TODO: 조건분기 넣어야함
        _GeneInfoUI.gameObject.SetActive(true);
        _GeneInfoUI.Init(_islandPetData);
    }
}
