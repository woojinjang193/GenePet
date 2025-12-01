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
}
