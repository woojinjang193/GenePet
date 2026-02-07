using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private CameraResolution _cameraResolution; 

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
    [SerializeField] private float _dragThreshold = 10f;

    [Header("핀치 줌 설정")] // 핀치 줌 파라미터
    [SerializeField] private float _pinchZoomSpeed = 0.005f; // 픽셀 델타 > OrthoSize 변화량
    [SerializeField] private float _minOrthoSize = 3.5f;     // 평상시 최소 줌(작을수록 확대)
    [SerializeField] private float _maxOrthoSize = 7.0f;     // 평상시 최대 줌(클수록 축소)

    [Header("카메라 경계")]
    [SerializeField] private BoxCollider2D _cameraBounds; // 화면이 넘어가지 않을 월드 경계 콜라이더

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

        Vector3 targetPos = new Vector3(_startCamPos.x + moveX, _startCamPos.y + moveY, _startCamPos.z); //목표 위치
        transform.position = ClampCameraPosition(targetPos); //콜라이더 경계로만 클램프
    }

    public void EndDrag()
    {
        _isDragging = false;
    }
    public void ApplyPinchZoom(float pinchDeltaPixels, Vector2 pinchCenterScreen) //핀치 줌(두 손가락 중앙 기준)
    {
        if (_isZoom) return; // 줌인 중엔 금지
        if (_mainCam == null || !_mainCam.orthographic) return; // Ortho 전용

        // 줌 전: 핀치 중앙이 가리키는 월드 좌표(평면 z=0 기준)
        Vector3 worldBefore = ScreenToWorldOnZPlane(pinchCenterScreen, 0f);

        // 거리 증가(+delta) = 확대 의도 => OrthoSize는 감소 방향
        float targetSize = _mainCam.orthographicSize - (pinchDeltaPixels * _pinchZoomSpeed);
        targetSize = Mathf.Clamp(targetSize, _minOrthoSize, _maxOrthoSize);

        //사이즈 변경
        _mainCam.orthographicSize = targetSize;

        // 줌 후: 같은 스크린 지점이 가리키는 월드 좌표
        Vector3 worldAfter = ScreenToWorldOnZPlane(pinchCenterScreen, 0f);

        // 핀치 중앙이 같은 월드 지점을 계속 가리키도록 카메라 위치 보정
        Vector3 deltaWorld = worldBefore - worldAfter;
        Vector3 targetPos = transform.position + new Vector3(deltaWorld.x, deltaWorld.y, 0f); //보정 후 목표 위치
        transform.position = ClampCameraPosition(targetPos); //새 줌 사이즈 기준으로 경계 재클램프
    }
    private Vector3 ScreenToWorldOnZPlane(Vector2 screenPos, float planeZ) //z=0 평면 교차 월드 좌표 계산
    {
        Ray ray = _mainCam.ScreenPointToRay(screenPos);
        float t = (planeZ - ray.origin.z) / ray.direction.z;
        return ray.origin + ray.direction * t;
    }
    private Vector3 ClampCameraPosition(Vector3 pos) // 현재 orthographicSize/aspect 기준으로 "화면이 경계 밖으로 안 나가게" 위치 클램프
    {
        if (_mainCam == null || !_mainCam.orthographic) return pos;
        if (_cameraBounds == null) return pos; // 콜라이더 없으면 그대로

        Bounds b = _cameraBounds.bounds; // 월드 경계 AABB

        float halfH = _mainCam.orthographicSize;     //화면 반높이
        float halfW = halfH * _mainCam.aspect;       //화면 반너비

        float minX = b.min.x + halfW; // 카메라 중심이 갈 수 있는 최소 X
        float maxX = b.max.x - halfW; // 카메라 중심이 갈 수 있는 최대 X
        float minY = b.min.y + halfH; // 카메라 중심이 갈 수 있는 최소 Y
        float maxY = b.max.y - halfH; // 카메라 중심이 갈 수 있는 최대 Y

        // 경계가 화면보다 작으면(뒤집힘) 중앙 고정
        float x = (minX > maxX) ? b.center.x : Mathf.Clamp(pos.x, minX, maxX);
        float y = (minY > maxY) ? b.center.y : Mathf.Clamp(pos.y, minY, maxY);

        return new Vector3(x, y, pos.z);
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
        float zoomSize = _cameraResolution.GetAspectFixedOrthoSize(_zoomOrthoSize); // 줌 사이즈도 화면비 보정
        _mainCam.orthographicSize = zoomSize;

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
        if (_mainCam != null)
        {
            Vector3 targetPos = new Vector3(pos.x, pos.y, -10f); // 목표 위치
            _mainCam.transform.position = ClampCameraPosition(targetPos); //경계 기반으로 클램프
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
