using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _dragSpeed = 0.01f;
    [SerializeField] private float _minX = -4f;
    [SerializeField] private float _maxX = 4f;
    [SerializeField] private float _minY = -4f;
    [SerializeField] private float _maxY = 4f;

    private Vector3 _startCamPos;
    private bool _isZoom = false;
    public bool IsZoom => _isZoom;

    private void Awake()
    {
        _startCamPos = transform.position;
    }

    public void BeginDrag()
    {
        _startCamPos = transform.position;
    }

    public void DragCamera(Vector3 drag)
    {
        if (_isZoom) return;

        float moveX = drag.x * _dragSpeed * -1f;
        float moveY = drag.y * _dragSpeed * -1f;

        float newX = Mathf.Clamp(_startCamPos.x + moveX, _minX, _maxX);
        float newY = Mathf.Clamp(_startCamPos.y + moveY, _minY, _maxY);

        transform.position = new Vector3(newX, newY, _startCamPos.z);
    }

    public void CameraZoomIn(Vector3 pos)
    {
        _isZoom = true;
        Camera.main.orthographicSize = 2.8f;
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -10f);
    }

    public void CameraZoomOut()
    {
        _isZoom = false;
        Camera.main.orthographicSize = 5f;
    }
}
