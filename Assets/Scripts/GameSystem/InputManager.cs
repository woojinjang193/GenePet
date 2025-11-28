using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float _clickTimeLimit = 0.2f;
    [SerializeField] private LayerMask _petMask;

    private float _mouseDownTime;
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
        HandleClick();
        HandleDragInput();
    }

    private void HandleClick()
    {
        // UI 클릭 (전체 차단)
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
            {
                _blockedByUI = true;
                return;
            }

            _blockedByUI = false;
            _mouseDownTime = Time.time;
        }

        if (_blockedByUI)
        {
            if (Input.GetMouseButtonUp(0))
                _blockedByUI = false;
            return;
        }

        // 드래그 중이면 클릭 취소
        if (_camera.IsDragging)
        {
            if (Input.GetMouseButtonUp(0))
                return;
        }

        // 클릭 판정
        if (Input.GetMouseButtonUp(0))
        {
            float held = Time.time - _mouseDownTime;
            if (held <= _clickTimeLimit)
                TryClickPet();
        }
    }

    private void HandleDragInput()
    {
        // UI에서 다운하면 드래그를 아예 전달하지 않음
        if (_blockedByUI) return;

        if (Input.GetMouseButtonDown(0))
            _camera.BeginDrag(Input.mousePosition);

        if (Input.GetMouseButton(0))
            _camera.Drag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            _camera.EndDrag();
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
