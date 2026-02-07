using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float _clickTimeLimit = 0.2f;
    [SerializeField] private LayerMask _petMask;
    [SerializeField] private TutorialChunk _firstVisitTutorial;
    private float _mouseDownTime;
    private bool _blockedByUI = false;

    private CameraController _camera;
    private PetManager _petManager;

    private bool _isPinching = false; // 핀치 중엔 클릭/드래그 차단
    private bool _dragActive = false; // BeginDrag가 시작됐는지
    private void Awake()
    {
        _camera = FindObjectOfType<CameraController>();
        _petManager = FindObjectOfType<PetManager>();
    }

    private void Update()
    {
        if(HandlePinchZoom()) return; // 핀치 처리 우선(핀치면 클릭/드래그 스킵)

        HandleClick();
        HandleDragInput();
    }
    private bool HandlePinchZoom() // 핀치 줌
    {

#if UNITY_EDITOR || UNITY_STANDALONE
        _isPinching = false; // PC에선 핀치 없음
        return false;
#else
        if (_camera == null) { _isPinching = false; return false; }
        if (_camera.IsZoom) { _isPinching = false; return false; }  //줌인 중엔 핀치 금지
        if (_firstVisitTutorial != null && _firstVisitTutorial.IsRunning) { _isPinching = false; return false; } //튜토리얼 중엔 핀치 금지

        if (Input.touchCount != 2)
        {
            if (_isPinching)
            {
                _camera.EndDrag();  //핀치 끝날 때 드래그 강제 종료
                _dragActive = false;  //세션 리셋
            }
            _isPinching = false;
            return false;
        }

        // 핀치 시작 순간: 드래그 강제 종료
        if (!_isPinching)
            _camera.EndDrag();

        _isPinching = true;

        //터치 받아옴
        var t0 = Input.GetTouch(0);
        var t1 = Input.GetTouch(1);

        // UI 위 터치면 핀치 무시(두 손가락 모두 체크)
        if (EventSystem.current != null &&
            (EventSystem.current.IsPointerOverGameObject(t0.fingerId) ||
             EventSystem.current.IsPointerOverGameObject(t1.fingerId)))
        {
            return true;
        }

        //이전 프레임 위치(터치.deltaPosition 이용) deltaPosition: 이전 프레임 대비 얼마나 이동했는지
        Vector2 t0Prev = t0.position - t0.deltaPosition;
        Vector2 t1Prev = t1.position - t1.deltaPosition;

        float prevDist = (t0Prev - t1Prev).magnitude;
        float curDist = (t0.position - t1.position).magnitude;
        float delta = curDist - prevDist; // +면 확대 -면 줌아웃

        Vector2 pinchCenter = (t0.position + t1.position) * 0.5f; // 두 손가락 중앙

        _camera.ApplyPinchZoom(delta, pinchCenter); // 카메라에 적용

        return true; //핀치 처리했으니 다른 입력 막기
#endif
    }

    private void HandleClick()
    {
        if (_isPinching) return; //핀치 중 클릭 금지

        // UI 클릭 (전체 차단)
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) //ui위 터치면
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
        if (_isPinching) return; // 핀치 중 드래그 금지
        if (_firstVisitTutorial.IsRunning) return; //튜토리얼중일땐 드래그 안됨
        // UI에서 다운하면 드래그를 아예 전달하지 않음
        if (_blockedByUI) return;

        if (Input.GetMouseButtonDown(0))
        {
            _dragActive = true;
            _camera.BeginDrag(Input.mousePosition);
        }

        if (_dragActive && Input.GetMouseButton(0))
            _camera.Drag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            _dragActive = false; // 드래그 세션 종료
            _camera.EndDrag();
        }
    }

    private void TryClickPet()
    {
        if (_petManager.ZoomedPet != null) return;

        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 p = new Vector2(world.x, world.y);

        RaycastHit2D hit = Physics2D.Raycast(p, Vector2.zero, 0f, _petMask);
        if (hit.collider != null)
        {
            PetUnit pet = hit.collider.GetComponent<PetUnit>();
            if (pet != null)
            {
                //Debug.Log($"HIT PetUnit obj={pet.gameObject.name}, PetId='{pet.PetId}'");
                _petManager.ZoomInPet(pet);
                Manager.Audio.PlaySFX("ZoomPet");// SPX ZoomPet
                Debug.Log($"{pet.PetId} 줌인");
            }
        }
    }

    private bool IsPointerOverUI()
    {
    #if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    #else
    if (Input.touchCount > 0)
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    }
    return false;
    #endif
    }

}
