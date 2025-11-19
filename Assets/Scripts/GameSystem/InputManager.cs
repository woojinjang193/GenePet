using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float _dragThreshold = 10f;
    [SerializeField] private float _clickTimeLimit = 0.2f;
    [SerializeField] private LayerMask _petMask;

    private Vector3 _mouseDownPos;
    private float _mouseDownTime;
    private bool _isDragging = false;
    private bool _blockedByUI = false;

    private CameraController _camera;
    private PetManager _petManager;

    private void Awake()
    {
        _camera = FindObjectOfType<CameraController>();
        _petManager = FindObjectOfType<PetManager>();
    }
    private void Update()
    {
        // 마우스 다운
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
            {
                _blockedByUI = true;
                return;
            }

            _blockedByUI = false;

            _mouseDownPos = Input.mousePosition;
            _mouseDownTime = Time.time;
            _isDragging = false;

            _camera.BeginDrag();
        }

        if (_blockedByUI)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _blockedByUI = false;
            }
            return;
        }

        // 마우스 유지(드래그 검사)
        if (Input.GetMouseButton(0))
        {
            float dist = (Input.mousePosition - _mouseDownPos).magnitude;

            if (dist > _dragThreshold)
            {
                _isDragging = true;
                Vector3 drag = Input.mousePosition - _mouseDownPos;
                _camera.DragCamera(drag);
            }
        }

        // 마우스 업 (클릭 판단)
        if (Input.GetMouseButtonUp(0))
        {
            if (!_isDragging && !_camera.IsZoom)
            {
                float heldTime = Time.time - _mouseDownTime;

                if (heldTime <= _clickTimeLimit)
                {
                    TryClickPet();
                }
            }
        }
    }

    private void TryClickPet()
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 p = new Vector2(world.x, world.y);

        RaycastHit2D hit = Physics2D.Raycast(p, Vector2.zero, 0f, _petMask);
        if (hit.collider != null)
        {
            PetUnit pet = hit.collider.GetComponent<PetUnit>();
            if (pet != null)
            {
                _petManager.ZoomInPet(pet.Status.ID);
                Debug.Log($"{pet.Status.ID} 줌인");
            }
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }
}
