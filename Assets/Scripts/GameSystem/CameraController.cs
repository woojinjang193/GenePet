using UnityEngine;

public class CameraController : MonoBehaviour
{
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

    public void CameraZoomIn(Vector3 pos)
    {
        _isZoom = true;
        Camera.main.orthographicSize = 2.8f;
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -10f);
    }

    public void SetBackGround(Room room)
    {
        _roomRoot.SetRoom(room);
        _roomRoot.gameObject.SetActive(true); //배경 on
    }

    public void CameraZoomOut()
    {
        _isZoom = false;
        _roomRoot.gameObject.SetActive(false); //배경 off
        Camera.main.orthographicSize = 5f;
    }

    public void CameraMoveTo(Vector3 pos)
    {
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -10f);
    }
}
