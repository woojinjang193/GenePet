using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlayerController : MonoBehaviour
{
    [SerializeField] private JumpMiniGame _jumpGame;
    [SerializeField] private Rigidbody2D _rigid;

    [Header("비주얼 트랜스 폼")]
    [SerializeField] private Transform _visual;
    [Header("최대 납작 비율")]
    [SerializeField] private float _squashY = 0.7f;
    [Header("가로 늘림 비율")]
    [SerializeField] private float _stretchX = 1.2f;  
    [Header("복원 속도")]
    [SerializeField] private float _recoverSpeed = 12f;

    [Header("바닥 체크")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundConfirmTime = 0.05f;
    public bool IsGrounded { get; private set; }
    private float _groundConfirmTimer;
    private int _groundContactCount; // 바닥 접촉 수

    private Vector3 _originScale;  // 원래 스케일
    private bool _isSquashed;

    private void Awake()
    {
        _originScale = _visual.localScale;  // 원본 저장
    }

    private void Update()
    {
        // 점프 후 자연 복원
        if (_isSquashed)
        {
            _visual.localScale = Vector3.Lerp(
                _visual.localScale,
                _originScale,
                Time.deltaTime * _recoverSpeed
            );

            if (Vector3.Distance(_visual.localScale, _originScale) < 0.01f)
            {
                _visual.localScale = _originScale;
                _visual.localPosition = Vector3.zero; // 위치 복원
                _isSquashed = false;
            }
        }

        // ===== 바닥 확정 =====
        if (!IsGrounded && _groundContactCount > 0) //바닥이 아니고 닿아있는 바닥이 하나 이상일때
        {
            _groundConfirmTimer += Time.deltaTime;
            if (_groundConfirmTimer >= _groundConfirmTime) //타이머 시간 후에 바닥 확정
            {
                IsGrounded = true;
                Debug.Log($"바닥: {IsGrounded}");
            }
        }
    }
    // =================점프 연출=======================
    public void SetChargeVisual(float chargeRate)
    {
        // chargeRate: 0~1
        float y = Mathf.Lerp(1f, _squashY, chargeRate); // 점점 납작
        float x = Mathf.Lerp(1f, _stretchX, chargeRate);// 가로 늘어남

        _visual.localScale = new Vector3(
            _originScale.x * x,
            _originScale.y * y,
            _originScale.z
        );

        // ===== 바닥 기준 보정 =====
        float offsetY = (_originScale.y - _originScale.y * y);// * 0.5f;
        _visual.localPosition = new Vector3(
            _visual.localPosition.x,
            -offsetY, // 아래로 내림
            _visual.localPosition.z
        );

        _isSquashed = true;
    }
    // ==================점프================
    public void Jump(float power, Vector2 dir)
    {
        if (!IsGrounded) return;//공중 점프 방지
        if (dir == Vector2.zero) return;            // 방향 없으면 실행 안함

        IsGrounded = false;
        _groundContactCount = 0; //닿아있는 바닥수 초기화
        _groundConfirmTimer = 0f; //타이머 초기화

        _rigid.velocity = Vector2.zero;             // 기존 속도 제거
        _rigid.AddForce(                            // 힘 적용
            dir.normalized * power,                 // 방향 정규화 후 파워 곱함
            ForceMode2D.Impulse                     // 즉시 힘 적용
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
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) //물리 
    {
        if (IsGround(collision.gameObject.layer)) //레이어 검사
        {
            Debug.Log("바닥 터치");
            _groundContactCount++;
            _groundConfirmTimer = 0f; // 접촉 갱신
        }
    }
    // ================= 바닥 판정 처리 =================
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGround(collision.gameObject.layer))
        {
            _groundContactCount--;
            if (_groundContactCount <= 0)
            {
                _groundContactCount = 0;
                IsGrounded = false;
                _groundConfirmTimer = 0f;
            }
        }
    }
    private bool IsGround(int layer)
    {
        return ((1 << layer) & _groundLayer) != 0;
    }

    // 현재 높이 제공
    public float GetHeight()
    {
        return transform.position.y;
    }
}
