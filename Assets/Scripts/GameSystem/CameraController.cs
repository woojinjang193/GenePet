using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _zoomCamera; //줌 전용 카메라
    [SerializeField] private int _zoomPetLayer;        //줌인 펫 레이어
    [SerializeField] private int _defaultPetLayer;     //원래 펫 레이어
    private GameObject _currentZoomPet;                //현재 줌인된 펫

    [SerializeField] private BackgroundRoomController _roomRoot;
    [SerializeField] private float _dragSpeed = 0.01f;
    [SerializeField] private float _minX = -4f;
    [SerializeField] private float _maxX = 4f;
    [SerializeField] private float _minY = -4f;
    [SerializeField] private float _maxY = 4f;
    [SerializeField] private float _dragThreshold = 10f;

    private bool _isZoom = false;
    public bool IsZoom => _isZoom;

    private bool _isDragging = false;
    public bool IsDragging => _isDragging;

    private Vector3 _dragStartMousePos;
    private Vector3 _startCamPos;

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
        _isZoom = true;

        _currentZoomPet = petRoot; //줌인된 펫 저장

        _defaultPetLayer = petRoot.layer; //기존 레이어 저장
        SetLayerRecursively(petRoot, _zoomPetLayer);

        _zoomCamera.gameObject.SetActive(true); // 줌 카메라 ON

        Camera.main.orthographicSize = 2.8f;
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -10f);
        _zoomCamera.transform.position = new Vector3(pos.x, pos.y, -10f);
    }
    public void CameraZoomOut()
    {
        _isZoom = false;

        if (_currentZoomPet != null) //줌인된 펫이 있으면
        {
            SetLayerRecursively(_currentZoomPet, _defaultPetLayer); //레이어 원복
            _currentZoomPet = null;
        }

        _zoomCamera.gameObject.SetActive(false);

        _roomRoot.gameObject.SetActive(false); //배경 off
        Camera.main.orthographicSize = 5f;
    }
    public void SetBackGround(Room room)
    {
        _roomRoot.SetRoom(room);
        _roomRoot.gameObject.SetActive(true); //배경 on
    }
    public void CameraMoveTo(Vector3 pos)
    {
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -10f);
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
