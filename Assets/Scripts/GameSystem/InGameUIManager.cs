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

        if (Manager.Item.RewardQueue.Count > 0)
        {
            //Debug.Log("리워드 UI 오픈");
            _rewardUI.gameObject.SetActive(true);
        }
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
    public void TryOpenLetter(PetUnit pet, LeftReason reason)
    {
        if (_petManager.ZoomedUnit != pet)
            return;

        OpenLetterPanel(reason);
    }
    private void OpenLetterPanel(LeftReason reason)
    {
        _letterPanel.gameObject.SetActive(true);
        _letterPanel.WriteLetter(reason);
    }

    public void UpdateEnergyBar(int newValue)
    {
        _energySlider.SetEnergy(newValue);
    }

    //public void ShowReward(Sprite image)
    //{
    //    _rewardUI.gameObject.SetActive(true);
    //    _rewardUI.SetImage(image);
    //}
}
