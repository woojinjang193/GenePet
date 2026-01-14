using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JumpMiniGame : MiniGameBase
{
    [Header("플레이어")]
    [SerializeField] private JumpPlayerController _player;
    [SerializeField] private MiniGamePetVisualLoader _playerVisual;

    [Header("카메라")]
    [SerializeField] private JumpGameCamera _camera;

    [Header("차지")]
    [SerializeField] private float _maxChargeTime = 2f;
    [SerializeField] private AnimationCurve _chargeCurve;// 차지 곡선(0~1)

    [Header("점프")]
    [SerializeField] private float _basePower = 15f;
    [SerializeField] private float _jumpAngle = 65f;

    [Header("점수")]
    [SerializeField] private float _scorePerHeight = 1f; // 높이 1당 점수
    [SerializeField] private TMP_Text _bestScoreText;
    [SerializeField] private TMP_Text _curScoreText;

    // ===== 내부 상태 =====
    
    private bool _isHolding; // 버튼 누르고 있는지
    private bool _isCameraMoving; // 카메라 이동중인지
    private int _direction;  // -1 왼쪽 / 1 오른쪽
    private float _chargeTime; // 누적 차지 시간
    private float _maxReachHeight; // 최고 도달 높이
    private Vector3 _cameraStartPos;
    private bool _isFinalJumping;

    // ===== 미니게임별 능력 계수 =====
    private float _jumpPowerMul = 1f; // 점프 파워 배율 (현재 안씀)
    private float _scoreMul = 1f;     // 점수 배율 (현재 안씀)
    private float _coinMul = 1f;  //코인 아이템 획득 배율
    private float _finalJumpPower = 0f; //마지막 점프 파워


    protected override void Start()
    {
        base.Start();

        _cameraStartPos = Camera.main.transform.position; //카메라 시작점 저장
        _playerVisual.LoadPetVisual(_pet); //펫 비주얼 로드

        _isCameraMoving = false;
        _maxReachHeight = _player.transform.position.y;

        _player.OnPlayerGrounded += UpdateHeightScore;
        _camera.OnCameraMoving += OnCameraMoving;
    }
    private void OnDestroy()
    {
        _player.OnPlayerGrounded -= UpdateHeightScore;
        _camera.OnCameraMoving -= OnCameraMoving;
    }
    public void OnGameStartClicked() //게임 시작 버튼 눌림
    {
        GameReset();
        GameStart();
        _isGameOver = false;
    }
    protected override void GameReset()
    {
        Camera.main.transform.position = _cameraStartPos; //카메라 포지션 리셋
        _player.gameObject.SetActive(true);
        _player.gameObject.transform.position = Vector3.zero; //플레이어 포지션 리셋

        _player.SetFinalJumpState(false); //플레이어 Ground 충돌 되도록 설정
        _camera.SetFinalJumpState(false);

        _maxReachHeight = _player.transform.position.y;
        
        ApplyAbilities();
        //TODO: 배경 리셋 여기에 추가

        base.GameReset();

        _curScoreText.text = $"Score: {_score}";
    }
    public void GoBackHome()
    {
        FinishGame();
    }
    private void Update()
    {
        UpdateCharge();  // 차지 누적

        if (_isFinalJumping)
        {
            FinalJumpScoreUpdate();
        }
    }

    // ================= 입력 =================
    private void OnCameraMoving(bool isMoving)
    {
        _isCameraMoving = isMoving;
    }
    public void OnPressButton(int dir)    // 버튼 누름 (JumpButton에서 호출)
    {
        if (_isGameOver) return;
        if (_isCameraMoving) return;
        if (_isHolding) return;
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

    // ================= 점수(플레이어한테 이벤트 호출받음) =================
    private void UpdateHeightScore(float curY)
    {
        if (curY <= _maxReachHeight) return; //기록 갱신 안되면 리턴

        float diff = curY - _maxReachHeight;
        int gained = Mathf.FloorToInt(diff * _scorePerHeight * _scoreMul);

        if (gained > 0)
        {
            AddScore(gained); // Base의 점수 누적
            _maxReachHeight = curY;
            _curScoreText.text = $"Score: {_score}";
        }
    }

    // ================= 외부 이벤트 =================
    public void OnItemCollected(RewardType type, int amount)
    {
        //if(!_isPlaying) return;

        if (type == RewardType.Coin)
        {
            amount = Mathf.FloorToInt(amount * _coinMul); //골드 배율만큼 더 획득
        }

        GainItem(type, amount); // 아이템 누적
    }

    public void OnPlayerDead()
    {
        _isHolding = false;
        _isCameraMoving = false;
        _isGameOver = true;

        Debug.Log("게임오버");
        if(_finalJumpPower > 0)
        {
            Debug.Log("코루틴 시작");
            StartCoroutine(FinalJumpRoutine());
        }
        else
        {
            GameOver(); //바로 게임 오버
        }
    }
    //===========특수능력 ====================
    public void ApplyAbilities()
    {
        if (_effectContext == null) { Debug.LogWarning("_effectContext 없음"); return;}
        
        _coinMul = _effectContext.GoldMultiplier; //코인 배율
        _finalJumpPower = _effectContext.Jump_FinalJumpPower; //마지막 점프 파워

        _isFinalJumping = false;
        Debug.Log($"파이널 점프파워 {_finalJumpPower}");
    }
    private IEnumerator FinalJumpRoutine()
    {
        Debug.Log("파이널점프 코루틴 시작");
        _isPlaying = true;
        yield return new WaitForSeconds(2); //2초 대기, 효과음 같은거 여기에 넣기
        FinalJump();
    }
    private void FinalJump()
    {
        _isFinalJumping = true;

        float rad = _jumpAngle * Mathf.Deg2Rad;  // 각도 → 라디안 변환
        Vector2 jumpDir = Vector2.zero;

        if (_player.transform.position.x <= 0) //플레이어 위치가 왼쪽일때
        {
            jumpDir = new Vector2(Mathf.Cos(rad) * 1, Mathf.Sin(rad));
        }
        else //오른쪽일때
        {
            jumpDir = new Vector2(Mathf.Cos(rad) * -1, Mathf.Sin(rad));
        }

        _camera.SetFinalJumpState(true);
        _player.SetFinalJumpState(true); //파이널 점프시 바닥에 충돌 안되도록 설정
        _player.gameObject.SetActive(true);
        _player.Jump(_finalJumpPower, jumpDir);
        _finalJumpPower = 0f;
    }
    private void FinalJumpScoreUpdate()
    {
        float curY = _player.transform.position.y;

        if (curY > _maxReachHeight)
        {
            float diff = curY - _maxReachHeight;
            int gained = Mathf.FloorToInt(diff * _scorePerHeight * _scoreMul);

            if (gained > 0)
            {
                AddScore(gained);
                _maxReachHeight = curY;
                _curScoreText.text = $"Score: {_score}";
            }
        }
    }
}
