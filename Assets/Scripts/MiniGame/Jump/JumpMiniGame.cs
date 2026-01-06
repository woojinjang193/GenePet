using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JumpMiniGame : MiniGameBase
{
    [Header("플레이어")]
    [SerializeField] private JumpPlayerController _player;

    [Header("차지")]
    [SerializeField] private float _maxChargeTime = 2f;
    [SerializeField] private AnimationCurve _chargeCurve;// 차지 곡선(0~1)

    [Header("점프")]
    [SerializeField] private float _basePower = 15f;
    [SerializeField] private float _jumpAngle = 60f;

    [Header("점수")]
    [SerializeField] private float _scorePerHeight = 1f; // 높이 1당 점수
    [SerializeField] private TMP_Text _bestScoreText;
    [SerializeField] private TMP_Text _curScoreText;

    // ===== 내부 상태 =====
    private bool _isHolding; // 버튼 누르고 있는지
    private int _direction;  // -1 왼쪽 / 1 오른쪽
    private float _chargeTime; // 누적 차지 시간
    private float _maxReachedHeight;  // 최고 도달 높이

    // ===== 미니게임별 능력 계수 =====
    private float _jumpPowerMul = 1f; // 점프 파워 배율
    private float _scoreMul = 1f;     // 점수 배율

    protected override void Start()
    {
        base.Start();

        _maxReachedHeight = transform.position.y;

        //ApplyPetAbility(); // 성격/행복도 적용 //TODO:테스트 끝나면 활성화 해야함
    }

    private void Update()
    {
        UpdateCharge();        // 차지 누적
        UpdateHeightScore();   // 높이 점수
    }

    // ================= 입력 =================
    public void OnPressButton(int dir)    // 버튼 누름 (JumpButton에서 호출)
    {
        if (!_player.IsGrounded) return;

        _direction = dir;
        _isHolding = true;
    }
    public void OnReleaseButton()    // 버튼 해제 (JumpButton에서 호출)
    {
        if (!_isHolding) return;

        _isHolding = false;
        DoJump();   // 점프 실행
        _chargeTime = 0f; // 차지 초기화
        _direction = 0;  // 방향 초기화
    }

    // ================= 차지 =================
    private void UpdateCharge()
    {
        if (!_isHolding) return;

        _chargeTime += Time.deltaTime; //시간 누적

        if (_chargeTime > _maxChargeTime) //최대 시간보다 많으면 최대 시간으로 설정
        {
            _chargeTime = _maxChargeTime;
        }

        float rate = _chargeTime / _maxChargeTime; // 0~1 차지 비율
        _player.SetChargeVisual(rate); //납작 연출 전달
    }

    // ================= 점프 =================
    private void DoJump()
    {
        float t = Mathf.Clamp01(_chargeTime / _maxChargeTime); // 차지 비율(0~1) 
        float chargeRate = _chargeCurve != null ? _chargeCurve.Evaluate(t) : t; // 커브 있으면 커브 적용 없으면 선형
                                                                                // 
        float power = _basePower * chargeRate * _jumpPowerMul; // 최종 점프 파워 계산

        float rad = _jumpAngle * Mathf.Deg2Rad;                // 각도 → 라디안 변환

        Vector2 jumpDir = new Vector2(                          // 점프 방향 벡터 생성
            Mathf.Cos(rad) * _direction,                        // 좌/우 반영
            Mathf.Sin(rad)                                      // 위쪽 성분
        );

        _player.Jump(power, jumpDir);                           // 플레이어에게 점프 요청
    }

    // ================= 점수 =================
    private void UpdateHeightScore()
    {
        float curY = transform.position.y;
        if (curY <= _maxReachedHeight) return; //내려가는 경우에는 기록안함

        float diff = curY - _maxReachedHeight;
        int gained = Mathf.FloorToInt(diff * _scorePerHeight * _scoreMul);

        if (gained > 0)
        {
            AddScore(gained); // Base의 점수 누적
            _maxReachedHeight = curY;
        }
    }

    // ================= 외부 이벤트 =================
    public void OnItemCollected(RewardType type, int amount)
    {
        GainItem(type, amount); // 아이템 누적
    }

    public void OnPlayerDead()
    {
        FinishGame();  // 미니게임 종료
    }

    // ================= 능력 적용 =================
    private void ApplyPetAbility()
    {
        // 기본값
        _jumpPowerMul = 1f;
        _scoreMul = 1f;

        // 성격 보정 (예시)
        switch (int.Parse(_pet.Genes.Personality.DominantId))
        {
            case (int)PersonalityType.Brave:
                //능력
                break;
        }

        // 행복도 보정 (0~100 가정)
        float happyRate = Mathf.Clamp01(_pet.Happiness / 100f);
        _jumpPowerMul += happyRate * 0.4f; // 최대 +40%
        _scoreMul += happyRate * 0.3f;     // 최대 +30%
    }
}
