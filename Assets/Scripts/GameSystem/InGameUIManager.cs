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
    [Header("리워드 UI")]
    [SerializeField] private RewardPopUp _rewardUI;
    [Header("소지금 텍스트")]
    [SerializeField] private TMP_Text _moneyText;

    [Header("의존")]
    [SerializeField] private CameraController _camera;
    [SerializeField] private PetManager _petManager;

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
        if (_rewardUI == null)
        {
            _rewardUI = FindObjectOfType<RewardPopUp>();
        }

        _moneyText.text = Manager.Save.CurrentData.UserData.Items.Money.ToString(); // 소지금 초기화

        //메인씬 돌아왔을때 보상 있으면 실행
        if (Manager.Item.RewardQueue.Count > 0)
        {
            ShowReward();
        }

        Manager.Item.OnMoneyChanged += UpdateMoney;
        Manager.Item.OnRewardGranted += ShowReward;
    }
    private void OnDestroy()
    {
        Manager.Item.OnMoneyChanged -= UpdateMoney;
        Manager.Item.OnRewardGranted -= ShowReward;
    }

    // 펫 줌인 시
    public void OnZoomInPet()
    {
        _zoomOutButton.gameObject.SetActive(true);
        _mainUI.SetActive(false);
        _zoomedUI.SetActive(true);
    }

    // 펫 줌아웃 시
    public void OnZoomOutPet()
    {
        _zoomOutButton.gameObject.SetActive(false);
        _mainUI.SetActive(true);
        _zoomedUI.SetActive(false);
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
        if (_petManager.ZoomedUnit != pet)
            return;

        OpenLetterPanel(reason);
    }
    private void OpenLetterPanel(LeftReason reason) //편지 UI 오픈
    {
        _letterPanel.gameObject.SetActive(true);
        _letterPanel.WriteLetter(reason);
    }

    public void UpdateEnergyBar(int newValue) //에너지 바 업데이트
    {
        _energySlider.SetEnergy(newValue);
    }

    public void ShowReward() //메인씬 보상용
    {
        if (_rewardUI.gameObject.activeSelf) return;

        _rewardUI.gameObject.SetActive(true);
        _rewardUI.ShowNext();
    }
    private void UpdateMoney(int value)
    {
        _moneyText.text = value.ToString();
    }
}