using System;
using UnityEngine;

public class JumpPlayerController : MonoBehaviour
{
    [SerializeField] private JumpMiniGame _jumpGame;
    [SerializeField] private Rigidbody2D _rigid;

    [Header("비주얼 트랜스 폼")]
    [SerializeField] private Transform _visualRoot; //위치보정용
    [SerializeField] private Transform _body; // 스케일 조절용


    [Header("점프 차지 연출")]
    [SerializeField] private float _jumpSquashY = 0.7f;
    [SerializeField] private float _jumpRecoverSpeed = 12f;
    [SerializeField] private float _jumpStretchX = 1.2f;

    [Header("착지 연출")]
    [SerializeField] private float _landSquashY = 0.7f;
    [SerializeField] private float _landRecoverSpeed = 7f;
    [SerializeField] private float _landStretchX = 1.2f;

    [Header("착지 낙하속도 기준")]
    [SerializeField] private float _minLandSpeed = 2f;    // 약한 착지 기준 속도
    [SerializeField] private float _maxLandSpeed = 9f;   // 강한 착지 기준 속도

    [Header("착지 판정 (CircleCast)")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius = 0.2f; // 원 반지름
    [SerializeField] private float _groundCheckDistance = 0.22f; // 아래 검사 거리
    [SerializeField] private float _groundNormalMinY = 0.6f; // 아래쪽 노멀 기준  0 = 벽, 1 = 평지, 0.5 = 60%

    private int _defaultLayer; //플레이어 레이어
    public bool IsGrounded { get; private set; }

    private Vector3 _originScale;  // 원래 스케일
    private bool _isCharging; // 차지 중인지
    private bool _isSquashed; // 납작 상태 여부
    private float _currentRecoverSpeed; //현재 복원 스피드
    private bool _pendingGrounded; // 착지 대기 플래그

    public Action<float> OnPlayerGrounded; // 착지 이벤트

    private void Awake()
    {
        _originScale = _body.localScale;  // 원본 저장
        _defaultLayer = gameObject.layer;
    }

    private void Update()
    {
        if (_isSquashed && !_isCharging)
        {
            // 현재 스케일을 원본으로 보간
            Vector3 newScale = Vector3.Lerp(
                _body.localScale,
                _originScale,
                Time.deltaTime * _currentRecoverSpeed
            );

            _body.localScale = newScale;

            // 스케일에 맞춰 항상 바닥 기준 위치 보정
            float deltaY = _originScale.y - newScale.y;
            _visualRoot.localPosition = new Vector3(0f, -deltaY * 0.5f, 0f);


            // 복원 완료 판정
            if (Mathf.Abs(newScale.y - _originScale.y) < 0.01f)
            {
                _body.localScale = _originScale;
                _visualRoot.localPosition = Vector3.zero;
                _isSquashed = false;

                if (_pendingGrounded)  //복원 완료 후 바닥 판정 true
                {
                    IsGrounded = true;
                    _pendingGrounded = false;
                    OnPlayerGrounded?.Invoke(transform.position.y);
                    Debug.Log("바닥 True");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        CheckGroundByCircleCast(); // 착지 판정 실행
    }
    // ================= 착지 판정 (CircleCast) =================
    private void CheckGroundByCircleCast()
    {
        if (_rigid.velocity.y > 0f) return; // 상승 중이면 착지 판정 안 함

        RaycastHit2D hit = Physics2D.CircleCast(          // 원형 레이캐스트
            transform.position,  //시작위치
            _groundCheckRadius, //원 반지름
            Vector2.down, //쏘는 방향
            _groundCheckDistance, //쏘는 거리
            _groundLayer //레이어 마스크 
        );

        //if (!hit)     // 바닥 안 닿았으면
        //{
        //    IsGrounded = false;   // 공중 상태
        //    return;
        //}

        if (hit.normal.y < _groundNormalMinY) return;  // 옆면 / 경사 심하면 무시

        if (IsGrounded || _pendingGrounded) return; //이미 바닥이거나 팬딩중이면 리턴

        float fallSpeed = Mathf.Abs(_rigid.velocity.y); //낙하 스피드
        _rigid.velocity = Vector2.zero;   // 속도 제거 (튕김 방지)

        _pendingGrounded = true;
        IsGrounded = false;

        Debug.Log($"낙하스피드 : {fallSpeed}");
        PlayLandingSquash(fallSpeed);  // 착지 연출 실행  
    }

    // ================= 착지 연출 =================
    private void PlayLandingSquash(float fallSpeed) 
    {
        // 낙하 속도를 0~1로 정규화
        float t = Mathf.InverseLerp(
            _minLandSpeed,   // 이 속도 이하면 거의 안 납작
            _maxLandSpeed,   // 이 속도 이상이면 최대
            fallSpeed
        );

        float y = Mathf.Lerp(1f, _landSquashY, t); //최대값 기준 세로 납작
        float x = Mathf.Lerp(1f, _landStretchX, t); //최대값 기준 가로 늘림

        _body.localScale = new Vector3(
            _originScale.x * x,
            _originScale.y * y,
            _originScale.z
        );

        float deltaY = _originScale.y - (_originScale.y * y);
        _visualRoot.localPosition = new Vector3(0f, -deltaY * 0.5f, 0f);



        _currentRecoverSpeed = Mathf.Lerp(_landRecoverSpeed, _landRecoverSpeed * 1.3f, t); // 빠르게 떨어질수록 복원도 빠름

        _isSquashed = true;  // 복원 시작
    }
    // =================점프 차지 연출=======================
    public void SetChargeVisual(float chargeRate)
    {
        _isCharging = true;

        // chargeRate: 0~1
        float y = Mathf.Lerp(1f, _jumpSquashY, chargeRate); // 점점 납작
        float x = Mathf.Lerp(1f, _jumpStretchX, chargeRate);// 가로 늘어남

        _body.localScale = new Vector3(
            _originScale.x * x,
            _originScale.y * y,
            _originScale.z
        );

        // ===== 바닥 기준 보정 =====
        float deltaY = _originScale.y - (_originScale.y * y);
        _visualRoot.localPosition = new Vector3(0f, -deltaY * 0.5f, 0f);

    }
    // ==================점프================
    public void Jump(float power, Vector2 dir)
    {
        if (!IsGrounded) return;//공중 점프 방지
        if (dir == Vector2.zero) return;            // 방향 없으면 실행 안함

        _currentRecoverSpeed = _jumpRecoverSpeed;

        _isCharging = false;
        _isSquashed = true;
        IsGrounded = false;
        
        _rigid.velocity = Vector2.zero;  // 기존 속도 제거
        _rigid.AddForce(    // 힘 적용
            dir.normalized * power,  // 방향 정규화 후 파워 곱함
            ForceMode2D.Impulse   // 즉시 힘 적용
        );
    }
    // ================= 충돌 처리 =================
    private void OnTriggerEnter2D(Collider2D col) //트리거 
    {
        if (col.CompareTag("Item"))
        {
            if (col.TryGetComponent<ItemForMiniGame>(out var item))
            {
                _jumpGame.OnItemCollected(item.Reward, item.Amount); // 로직에 전달
            }

            col.gameObject.SetActive(false);         // 아이템 비활성화
        }
        else if (col.CompareTag("DeadZone"))
        {
            _jumpGame.OnPlayerDead();                 // 게임 종료 전달
            gameObject.SetActive(false); // 플레이어 끄기
        }
    }
    public void SetFinalJumpLayer(bool isFinalJump) //플레이어 레이어 변경 함수
    {
        if (isFinalJump)
        {
            gameObject.layer = LayerMask.NameToLayer("FinalJump");
            Debug.Log("레이어 설정");
        }
        else
        {
            gameObject.layer = _defaultLayer;
            Debug.Log("디폴트 레이어 ");
        }
    }


    // 현재 높이 제공
    public float GetHeight()
    {
        return transform.position.y;
    }
}
