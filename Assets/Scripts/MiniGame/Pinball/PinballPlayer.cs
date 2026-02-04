using System;
using UnityEngine;

public class PinballPlayer : MonoBehaviour
{
    [Header("Brick 레이어 인덱스")]
    [SerializeField] private int _brickLayer; // Brick 레이어 번호
    [Header("끼임 판정 시간")]
    [SerializeField] private float _stuckHitDelay = 1f;
    [Header("끼임 판정 속도 임계값")]
    [SerializeField] private float _stuckSpeedThreshold = 0.2f;

    private PinballBrick _currentBrick; // 현재 접촉 중인 브릭 캐시
    private Collider2D _currentBrickCollider; // 현재 접촉 중인 브릭 콜라이더 캐시
    private float _stuckTimer; // 끼임 시간 누적
    private float _stuckCooldown; // 연속 데미지 방지 쿨다운

    private Rigidbody2D _rigid;

    private int _damage = 1;
    private bool _hasGotReward = false;
    public event Action<RewardType, int> OnRewardGet;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }
    public void BallReset(int damage)
    {
        _damage = damage;
        _hasGotReward = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            if (_hasGotReward) return; //이미 보상 받았으면 리턴

            if (collision.TryGetComponent<ItemForMiniGame>(out var item))
            {
                OnRewardGet?.Invoke(item.Reward, item.Amount); // 아이템 획득 이벤트 발생
                _hasGotReward = true;
            }

            //collision.gameObject.SetActive(false);         // 아이템 비활성화
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != _brickLayer) return; //Brick 레이어만 처리

        if (collision.collider.TryGetComponent<PinballBrick>(out var brick)) //조회 후 캐싱
        {
            _currentBrick = brick; //Stay에서 재사용할 브릭 캐시
            _currentBrickCollider = collision.collider; //Stay에서 GetComponent 없이 비교하려고 콜라이더도 캐싱
            _stuckTimer = 0f; // 끼임 누적 초기화
            _stuckCooldown = 0f; //쿨다운 초기화

            _currentBrick.Hit(_damage); //데미지 주기
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_currentBrick == null) return; //캐시된 브릭 없으면 리턴
        if (_currentBrickCollider == null) return; //콜라이더 캐시 없으면 리턴
        if (collision.collider != _currentBrickCollider) return; // 현재 캐시된 브릭과 다른 접촉무시
        if (collision.collider.gameObject.layer != _brickLayer) return; //Brick 레이어만 처리

        if (_stuckCooldown > 0f) //연속 데미지 방지
        {
            _stuckCooldown -= Time.deltaTime; //쿨다운 감소
            return; //쿨다운 중엔 누적 안함
        }

        if (_rigid == null) return; // 안전장치

        float spdSqr = _rigid.velocity.sqrMagnitude; // 속도 제곱
        float thSqr = _stuckSpeedThreshold * _stuckSpeedThreshold; //임계값 제곱

        if (spdSqr > thSqr) // 충분히 움직이면 끼임 아님
        {
            _stuckTimer = 0f; // 누적 리셋
            return;
        }

        _stuckTimer += Time.deltaTime; // 거의 멈춘 상태면 누적

        if (_stuckTimer >= _stuckHitDelay) // 1초 이상 끼면
        {
            _stuckTimer = 0f; // 다음 판정 위해 리셋
            _stuckCooldown = _stuckHitDelay; // 또 1초 지나기 전까진 추가 데미지 금지

            _currentBrick.Hit(_damage); // 끼임 데미지
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != _brickLayer) return; // Brick 레이어만 처리
        if (_currentBrickCollider == null) return; //캐시가 없으면 처리 안함
        if (collision.collider != _currentBrickCollider) return; //캐시된 브릭에서 나가는 경우만 처리

        _currentBrick = null; // 캐시 해제
        _currentBrickCollider = null; // 캐시 해제
        _stuckTimer = 0f; //누적 초기화
        _stuckCooldown = 0f; //쿨다운 초기화
    }
}
