using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _dragSpeed = 0.01f;
    [SerializeField] private float _minX = 0f;
    [SerializeField] private float _maxX = 5f;

    private Vector3 _startMousePos;
    private Vector3 _startCamPos;
    private bool _isDragging = false;
    private bool _isZoom = false;

    void Update()
    {
        if (_isZoom) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
            {
                _isDragging = false;
                return;
            }

            _isDragging = true;
            _startMousePos = Input.mousePosition;
            _startCamPos = transform.position;
        }

        if (Input.GetMouseButtonUp(0))
            _isDragging = false;

        if (_isDragging)
        {
            Vector3 diff = Input.mousePosition - _startMousePos;
            float moveX = diff.x * _dragSpeed * -1f;

            float newX = Mathf.Clamp(_startCamPos.x + moveX, _minX, _maxX);

            transform.position = new Vector3(newX, _startCamPos.y, _startCamPos.z);
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        return results.Count > 0;
    }

    public void CameraZoomIn(Vector3 pos)
    {
        _isZoom = true;
        Camera.main.orthographicSize = 2.8f;
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -10f);
    }

    public void CameraZoomOut()
    {
        Camera.main.orthographicSize = 5f;
        _isZoom = false;
    }
}
