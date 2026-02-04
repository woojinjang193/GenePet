using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("줌아웃 버튼")]
    [SerializeField] private Button _zoomOutButton;
    [Header("메인 UI")]
    [SerializeField] private GameObject _mainUI;
    [Header("줌 UI")]
    [SerializeField] private GameObject _zoomedUI;
    [Header("편지 UI")]
    [SerializeField] private LetterPanel _letterPanel;
    [Header("에너지 슬라이더")]
    [SerializeField] private EnergySlider _energySlider;
    [Header("골드 소지량")]
    [SerializeField] private TMP_Text _goldAmount;

    [Header("줌인 UI 컨트롤러")]
    [SerializeField] private ZoomInUiController _zoomedUIController;
    [Header("펫소환 버튼")]
    [SerializeField] private Button _spawnPetButton;

    [Header("의존")]
    [SerializeField] private CameraController _camera;
    [SerializeField] private PetManager _petManager;

    [Header("유아이 예약")]
    [SerializeField] private GameObject[] _reservedUiPanels;

    private void Awake()
    {
        if(_camera == null)
        {
            _camera = FindObjectOfType<CameraController>();
        }
        if( _petManager == null)
        {
            _petManager = FindObjectOfType<PetManager>();
        }
        if (_zoomOutButton != null)
        {
            _zoomOutButton.onClick.AddListener(OnClickZoomOut);
            _zoomOutButton.gameObject.SetActive(false);
        }
        if (_energySlider == null)
        {
            _energySlider = FindObjectOfType<EnergySlider>();
        }

        //예약된 UI판넬이 있을때
        if(Manager.Game.ReservedUI != UIPanel.None)
        {
            OpenReservedUI(Manager.Game.ReservedUI); //UI 열어줌
        }

        Manager.Item.OnItemConsumed += AmountChange;
        Manager.Item.OnItemConsumed += AmountChange;
    }

    private void Start()
    {
        CheckTutorialFlag(); //튜토리얼 체크
        Manager.Audio.PlayBGM("BGM_Test"); //비지엠 재생
    }
    private void OnEnable()
    {
        _goldAmount.text = Manager.Save.CurrentData.UserData.Items.Money.ToString();

    }
    private void OnDestroy()
    {
        if(Manager.Item != null)
        {
            Manager.Item.OnItemConsumed -= AmountChange;
            Manager.Item.OnItemConsumed -= AmountChange;
        }
    }
    // 펫 줌인 시
    public void OnZoomInPet()
    {
        _zoomOutButton.gameObject.SetActive(true);
        _mainUI.SetActive(false);
        _zoomedUI.SetActive(true);
       // _zoomedUIController.
        _spawnPetButton.interactable = false;
    }

    // 펫 줌아웃 시
    public void OnZoomOutPet()
    {
        _zoomOutButton.gameObject.SetActive(false);
        _mainUI.SetActive(true);
        _zoomedUI.SetActive(false);
        _zoomedUIController.CancelSubscribe();
        _spawnPetButton.interactable = true;
    }

    // 줌아웃 버튼 클릭 이벤트
    private void OnClickZoomOut()
    {
        if (_petManager != null)
        {
            _petManager.ZoomOutPet();
        }
    }
    public void TryOpenLetter(PetUnit pet, LeftReason reason) //편지오픈 조건 검사
    {
        if (_petManager.ZoomedUnit != pet) return;

        GrowthStatus growth = _petManager.ZoomedUnit.Status.Growth;
        PetSaveData saveData = _petManager.ZoomedUnit.SaveData;

        OpenLetterPanel(reason, saveData, growth);
    }
    private void OpenLetterPanel(LeftReason reason, PetSaveData saveData, GrowthStatus growth) //편지 UI 오픈
    {
        _letterPanel.gameObject.SetActive(true);
        _letterPanel.SetLetter(reason, saveData, growth);
    }

    public void UpdateEnergyBar(int newValue) //에너지 바 업데이트
    {
        _energySlider.SetEnergy(newValue);
    }

    public void MiniGameStartButtonClicked(int index) //미니게임 시작버튼 클릭
    {
        if (_petManager.ZoomedPet == null)
        {
            Debug.LogError("펫 정보 없음");
            return;
        }
        var pet = _petManager.ZoomedPet;

        Manager.Mini.StartMiniGame(pet, index);
    }

    //==============예약된 UI 판넬 열어주는 유틸======================
    private void OpenReservedUI(UIPanel reservedUI)
    {
        switch (reservedUI)
        {
            case UIPanel.Shop: _reservedUiPanels[0].SetActive(true); break;
        }
    }
    //==============메인씬에서 판넬 열어주는 유틸======================
    public void OpenUiPanel(UIPanel reservedUI)
    {
        switch (reservedUI)
        {
            case UIPanel.Shop: _reservedUiPanels[0].SetActive(true); break;
        }
    }

    private void AmountChange(RewardType type, int newValue)
    {
        if(type == RewardType.Coin)
        {
            _goldAmount.text = newValue.ToString();
        }
    }

    private void CheckTutorialFlag()
    {
        if (!Manager.Save.CurrentData.UserData.tutorialFlags.FirstVisit)
        {
            var tutorial = FindObjectOfType<TutorialController>();
            if(tutorial != null)
            {
                tutorial.TryStartTutorial(TutorialTriggerKey.FirstVisit);
            }
        }
    }
}