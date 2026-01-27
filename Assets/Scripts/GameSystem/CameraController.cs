using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Zoom 대상 레이어")]
    [SerializeField] private int _zoomPetLayer;   //줌인 펫 레이어
    [SerializeField] private int _defaultPetLayer;  //원래 펫 레이어
    private GameObject _currentZoomPet;  //현재 줌인된 펫

    [Header("Zoom 시 카메라 렌더(보일 레이어만)")]
    [SerializeField] private LayerMask _zoomVisibleMask; //줌 중 MainCamera가 렌더할 레이어 마스크(UI/ZoomPet/배경/줌오브젝트만)
    private int _backupCullingMask;  // 줌 전 MainCamera cullingMask 백업

    [Header("Zoom 시 카메라 사이즈")]
    [SerializeField] private float _zoomOrthoSize = 2.8f; // 줌인 시 OrthoSize
    private float _backupOrthoSize;                        // 줌 전 OrthoSize 백업
    private Vector3 _backupCamPos;

    [Header("배경")]
    [SerializeField] private BackgroundRoomController _roomRoot;

    [Header("드래그 설정")]
    [SerializeField] private float _dragSpeed = 0.01f;
    [SerializeField] private float _minX = -7f;
    [SerializeField] private float _maxX = 7f;
    [SerializeField] private float _minY = -1.6f;
    [SerializeField] private float _maxY = 1.6f;
    [SerializeField] private float _dragThreshold = 10f;

    private Camera _mainCam;

    private bool _isZoom = false;
    public bool IsZoom => _isZoom;

    private bool _isDragging = false;
    public bool IsDragging => _isDragging;

    private Vector3 _dragStartMousePos;
    private Vector3 _startCamPos;

    private void Awake()
    {
        _mainCam = GetComponent<Camera>();
    }
    public void BeginDrag(Vector3 mousePos)
    {
        if (_isZoom) return;

        _isDragging = false;
        _dragStartMousePos = mousePos;
        _startCamPos = transform.position;
    }

    public void Drag(Vector3 mousePos)
    {
        if (_isZoom) return;

        Vector3 drag = mousePos - _dragStartMousePos;
        float dist = drag.magnitude;

        if (!_isDragging && dist > _dragThreshold)
            _isDragging = true;

        if (!_isDragging) return;

        float moveX = drag.x * _dragSpeed * -1f;
        float moveY = drag.y * _dragSpeed * -1f;

        float newX = Mathf.Clamp(_startCamPos.x + moveX, _minX, _maxX);
        float newY = Mathf.Clamp(_startCamPos.y + moveY, _minY, _maxY);

        transform.position = new Vector3(newX, newY, _startCamPos.z);
    }

    public void EndDrag()
    {
        _isDragging = false;
    }

    public void CameraZoomIn(Vector3 pos, GameObject petRoot)
    {
        if (_mainCam == null || !_mainCam.orthographic) return; // 안전 체크

        _isZoom = true;

        // ===== 카메라 상태 백업 =====
        _backupCamPos = _mainCam.transform.position;   // 위치 백업
        _backupOrthoSize = _mainCam.orthographicSize;  // 사이즈 백업
        _backupCullingMask = _mainCam.cullingMask;  // 마스크 백업

        // ===== 줌 대상 펫만 줌 레이어로 =====
        _currentZoomPet = petRoot;   // 줌인된 펫 저장
        _defaultPetLayer = petRoot.layer;    // 기존 레이어 저장
        SetLayerRecursively(petRoot, _zoomPetLayer); // 줌 레이어로 변경

        // ===== 줌 중엔 필요한 레이어만 렌더 =====
        _mainCam.cullingMask = _zoomVisibleMask;  // UI/ZoomPet/배경/줌오브젝트만 보이게

        // ===== 카메라 줌 연출 =====
        _mainCam.orthographicSize = _zoomOrthoSize;  // 줌 사이즈 적용
        _mainCam.transform.position = new Vector3(pos.x, pos.y, _backupCamPos.z); // 줌 위치로 이동(z는 유지)
    }
    public void CameraZoomOut()
    {
        if (_mainCam == null) return; //안전 체크

        _isZoom = false;

        // ===== 펫 레이어 원복 =====
        if (_currentZoomPet != null)
        {
            SetLayerRecursively(_currentZoomPet, _defaultPetLayer); //레이어 원복
            _currentZoomPet = null;
        }

        // ===== 카메라 상태 원복 =====
        _mainCam.cullingMask = _backupCullingMask;   // 렌더 마스크 복구
        _mainCam.orthographicSize = _backupOrthoSize;// OrthoSize 복구
        _mainCam.transform.position = _backupCamPos; // 위치 복구

        _roomRoot.gameObject.SetActive(false);       //배경 off
    }
    public void SetBackGround(Room room)
    {
        _roomRoot.SetRoom(room);
        _roomRoot.gameObject.SetActive(true); //배경 on
    }
    public void CameraMoveTo(Vector3 pos) //새로운 알 스폰시 카메라 이동
    {
        if(pos.y < _minY)
        {
            pos.y = _minY;
        }
        if (_mainCam != null)
        {
            _mainCam.transform.position = new Vector3(pos.x, pos.y, -10f);
        }
    }
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer; //현재 오브젝트 레이어 변경

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
