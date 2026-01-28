using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PetPetting : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Camera _cam;  // 스크린 >월드 변환용 카메라
    [SerializeField] private Collider2D _petCollider;   // 펫 판정 콜라이더
    [SerializeField] private ParticleSystem _pettingParticle; //쓰다듬 파티클

    [Header("레이어 마스크")]
    [SerializeField] private LayerMask _petLayer;   //펫 레이어

    [Header("움직임")]
    [SerializeField] private float _moveThresholdPx = 15f;   // 이 픽셀 이상 움직이면 "움직임"으로 인정
    [SerializeField] private float _stopDelay = 0.25f;     // 멈춘 상태가 이 시간 이상이면 파티클 정지

    private bool _isTouching;    //터치 유지 상태
    private bool _isOnPet;    //현재 펫 위인지

    private Vector2 _lastScreenPos;   // 이전 프레임 스크린 좌표
    private float _notMovingTimer;  // 멈춤 누적 시간
    private bool _isPlaying;    // 파티클 재생중 여부 캐시

    public event Action<bool> OnPettingChanged; // 쓰다듬기 이벤트

    private void Awake()
    {
        if (_cam == null) _cam = Camera.main;
        SetParticle(false); 
    }
    private void OnDisable()
    {
        SetParticle(false);
        _isTouching = false;  // 상태 초기화
        _isOnPet = false;   //상태 초기화
        _notMovingTimer = 0f;  //타이머 초기화
        _isPlaying = false;   //캐시 초기화
    }
    private void OnEnable()
    {
        _isTouching = false;
        _isOnPet = false;
        _notMovingTimer = 0f;
        _isPlaying = false;
        _pettingParticle.Stop();
    }

    private void Update()
    {
        if (SelectObject.IsHoldingItem) // 아이템 들고있으면 리턴
        {
            if (_isPlaying) SetParticle(false); // [수정] 켜져있을 때만 1번 끄기
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);   // 첫 터치만 사용(간단 버전)
            Vector2 screenPos = t.position;  // 스크린 좌표

            if (t.phase == TouchPhase.Began)
            {
                _isTouching = true;    // 터치 시작
                _lastScreenPos = screenPos;   // 기준 좌표 저장
                _notMovingTimer = 0f;   // 타이머 리셋
                _isOnPet = IsPointerOnPet(screenPos); //시작 위치가 펫 위인지
                SetParticle(false);  //시작만으로는 재생 안 함(움직여야 재생)
            }
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                if (!_isTouching)  // 예외 보호
                {
                    _isTouching = true;    // 상태 보정
                    _lastScreenPos = screenPos;  // 기준 좌표 보정
                }

                _isOnPet = IsPointerOnPet(screenPos); //현재 펫 위인지

                float movePx = (screenPos - _lastScreenPos).magnitude; // 스크린 이동량(픽셀)
                bool moving = movePx >= _moveThresholdPx;      //  움직임 판정

                if (_isOnPet)
                {
                    if (moving)
                    {
                        _notMovingTimer = 0f;   // 멈춤 타이머 리셋
                        SetParticle(true);   // 펫 위 + 움직임 => ON
                    }
                    else
                    {
                        _notMovingTimer += Time.deltaTime; // 멈춤 시간 누적
                        if (_notMovingTimer >= _stopDelay)  // 일정 시간 멈추면 OFF
                            SetParticle(false);  // OFF
                    }
                }
                else
                {
                    _notMovingTimer = 0f; //  펫 밖이면 타이머 리셋
                    SetParticle(false);    //  펫 밖이면 OFF
                }

                _lastScreenPos = screenPos;    // 다음 프레임 비교용 저장
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                _isTouching = false; // 터치 종료
                _isOnPet = false;   // 펫 위 아님
                _notMovingTimer = 0f;  //타이머 리셋
                SetParticle(false);   // 파티클 OFF
            }

            return; // 모바일이면 아래 마우스 로직 스킵
        }

        #if UNITY_EDITOR
        //  에디터 테스트용(마우스) - 동일 로직
        Vector2 mousePos = Input.mousePosition; //  마우스 스크린 좌표

        if (Input.GetMouseButtonDown(0))
        {
            _isTouching = true;     //  클릭 시작
            _lastScreenPos = mousePos;   //기준 좌표 저장
            _notMovingTimer = 0f;    //타이머 리셋
            _isOnPet = IsPointerOnPet(mousePos);   // 시작 위치가 펫 위인지
            SetParticle(false); // 시작만으로는 재생 안 함
        }
        else if (Input.GetMouseButton(0))
        {
            if (!_isTouching) // 예외 보호
            {
                _isTouching = true;   // 상태 보정
                _lastScreenPos = mousePos;   // 기준 좌표 보정
            }

            _isOnPet = IsPointerOnPet(mousePos);  // 현재 펫 위인지

            float movePx = (mousePos - _lastScreenPos).magnitude; // 스크린 이동량(픽셀)
            bool moving = movePx >= _moveThresholdPx;  // 움직임 판정

            if (_isOnPet)
            {
                if (moving)
                {
                    _notMovingTimer = 0f;  //타이머 리셋
                    SetParticle(true);    // ON
                }
                else
                {
                    _notMovingTimer += Time.deltaTime;  // 누적
                    if (_notMovingTimer >= _stopDelay)   //  일정 시간 멈추면 OFF
                        SetParticle(false);   // OFF
                }
            }
            else
            {
                _notMovingTimer = 0f;    // 리셋
                SetParticle(false);     // OFF
            }

            _lastScreenPos = mousePos;   // 저장
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isTouching = false;    // 클릭 종료
            _isOnPet = false;   // 펫 위 아님
            _notMovingTimer = 0f;   // 타이머 리셋
            SetParticle(false);     //  파티클 OFF
        }
        #endif
    }

    private bool IsPointerOnPet(Vector2 screenPos)
    {
        Vector3 world = _cam.ScreenToWorldPoint(screenPos);   // 스크린->월드
        Vector2 w2 = new Vector2(world.x, world.y);      // 2D 좌표로 변환

        Collider2D hit = Physics2D.OverlapPoint(w2, _petLayer);//  펫 레이어만 체크
        if (hit == null) return false;     // 없으면 false

        if (_petCollider != null) return hit == _petCollider; //내 펫만 true
        return true;    // 레이어만 맞으면 true
    }
    private void SetParticle(bool on)
    {
        if (_pettingParticle == null) return; //  null 방지

        if (on)
        {
            if (!_isPlaying)  // 중복 Play 방지
            {
                _isPlaying = true;  // 상태 갱신
                _pettingParticle.Play();  // ON
                OnPettingChanged?.Invoke(true); //쓰다듬기 이벤트 발생
            }
        }
        else
        {
            if (_isPlaying)  // 중복 Stop 방지
            {
                _isPlaying = false;   //상태 갱신
                _pettingParticle.Stop();
                OnPettingChanged?.Invoke(false); //쓰다듬기 이벤트 발생
            }
        }
    }
}
