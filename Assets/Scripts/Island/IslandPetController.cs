using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandPetController : MonoBehaviour
{
    [SerializeField] private IslandPetVisualController _visual; //비주얼 컨트롤러

    private IslandManager _islandManager; //섬 매니저
    private GiftCooldownService _cooldownService; //쿨타임 서비스
    private GiftWishController _wishController; //위시 로직

    private IslandData _islandData;

    private Coroutine _cooldownRoutine; // 스케줄 코루틴

    private void Start()
    {
        _islandManager = FindObjectOfType<IslandManager>(); //섬 매니저 찾기

        _cooldownService = new GiftCooldownService(Manager.Game.Config.GiftCooldown); //쿨타임 초기화
        _wishController = new GiftWishController(GetGiftList()); //가능한 선물 목록
        _islandData = Manager.Save.CurrentData.UserData.Island;

        //이벤트 구독
        _visual.Mouth.OnGiveTaken += OnGiveTaken;
        _islandManager.OnIslandMyPetChange += OnIslandMyPetChange;

        TryShowWish();

        RestartCooldownSchedule(); // 남은시간만큼 기다렸다가 자동 갱신
    }
    private void OnDestroy()
    {
        _visual.Mouth.OnGiveTaken -= OnGiveTaken; //이벤트 해제
        if( _islandManager != null )
        {
            _islandManager.OnIslandMyPetChange -= OnIslandMyPetChange;
        }

        StopCooldownSchedule(); //코루틴 정리
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) return;
        TryShowWish();            //앱 복귀 시 즉시 상태 갱신
        RestartCooldownSchedule(); //남은시간 재계산
    }
    // ================== 쿨타임 스케줄 (단발 대기) ==================
    private void RestartCooldownSchedule()
    {
        StopCooldownSchedule();  // 기존 스케줄 중지
        if(!_islandManager.IsLeft || !_islandManager.IsMarried)
        {
            _cooldownRoutine = StartCoroutine(CooldownScheduleRoutine()); // 새 스케줄 시작
        }
    }

    private void StopCooldownSchedule()
    {
        if (_cooldownRoutine != null)
        {
            StopCoroutine(_cooldownRoutine); // 실행중이면 중지
        }
       
        _cooldownRoutine = null; //초기화
    }
    private IEnumerator CooldownScheduleRoutine()
    {
        while (true)
        {
            // 마이펫 없으면 스케줄 종료
            if (string.IsNullOrWhiteSpace(_islandManager.IslandMyPetID))
            {
                _visual.CloseWishBubble(); //버블 닫기
                yield break;               //루프 종료
            }

            // 남은 시간 계산(0이면 바로 처리)
            float remain = _cooldownService.GetRemainingSeconds(_islandData.GiftCooldownStartTime); //남은시간 계산


            if (remain > 0f)
            {
                yield return new WaitForSecondsRealtime(remain); //타임스케일 영향 없이 정확히 대기
            }

            TryShowWish(); // 쿨타임 완료 순간 즉시 처리(버블 띄우기 등)

            yield return null; // 같은 프레임 중복 처리 방지용 한 프레임 쉬기
        }
    }

    // ===============================================================
    private void TryShowWish()
    {
        if (_islandData.GiftCooldownStartTime == 0) // 첫 방문(초기값) 예외 처리
        {
            _islandData.GiftCooldownStartTime = _cooldownService.RecordGiftTime(); // 지금부터 쿨타임 시작
            _islandData.CurWish = Gift.None; // 위시 없음 유지
            _visual.CloseWishBubble(); // 버블 닫기
            return; // 첫 방문엔 생성하지 않음
        }

        // 마이펫 없으면 아무것도 안 함
        if (string.IsNullOrWhiteSpace(_islandManager.IslandMyPetID))
        {
            _visual.CloseWishBubble();
            return;
        }

        // 쿨타임 돌기 전일때
        if (!_cooldownService.CanGiveGift(_islandData.GiftCooldownStartTime))
        {
            if (_islandData.CurWish != Gift.None) // 이미 위시가 있으면
            {
                Sprite sprite = Manager.Item.ItemImages.GetGiftSprite(_islandData.CurWish);
                _visual.ShowWish(sprite);
            }
            else
            {
                _visual.CloseWishBubble();
            }

            return;
        }

        //쿨타임 다 돌앗을 때
        if (_islandData.CurWish != Gift.None)
        {
            float disappointingPoint = Manager.Game.Config.DisappointingPoint;
            _islandManager.ChangeAffinity(-disappointingPoint);
            ResetGiftState();
            Debug.Log("선물 안줘서 호감도 감소");
            RestartCooldownSchedule(); // ResetGiftState로 시작시간이 바뀌었으니 재스케줄
            return;
        }
        else
        {
            // 새 위시 생성
            _islandData.CurWish = _wishController.CreateWish();
            _islandData.GiftCooldownStartTime = _cooldownService.RecordGiftTime(); //쿨타임 초기화

            Sprite wishSprite = Manager.Item.ItemImages.GetGiftSprite(_islandData.CurWish);
            _visual.ShowWish(wishSprite);

            RestartCooldownSchedule(); // 새 시작시간 기준으로 다음 완료 시점 스케줄
        }
    }
                            
    //============이벤트 발생시==============
    private void OnGiveTaken(Gift gift) //선물 먹음 이벤트
    {
        if (gift == Gift.MasterGift) //마스터 선물일때
        {
            _visual.GiftSuccess(); //성공 연출
            Debug.Log("마스터 선물 성공");
            Manager.Item.UseGift(gift); //선물 개수 감소
            _islandManager.ChangeAffinity(Manager.Game.Config.GiftingPoint); //호감도 적용

            ResetGiftState();
            RestartCooldownSchedule(); // 시작시간 바뀌었으니 스케줄 재계산
            return;
        }

        if (_islandData.CurWish != gift) //선물 불일치
        {
            _visual.GiftFail(); //실패 연출
            Debug.Log("선물 실패");
            return;
        }

        _visual.GiftSuccess(); //성공 연출
        Debug.Log("선물 성공");
        Manager.Item.UseGift(gift); //선물 개수 감소
        _islandManager.ChangeAffinity(Manager.Game.Config.GiftingPoint); //호감도 적용

        ResetGiftState();
        RestartCooldownSchedule(); // 시작시간 바뀌었으니 스케줄 재계산
    }
    private void OnIslandMyPetChange() //펫 변경 이벤트
    {
        ResetGiftState();     // 펫 변경 시 초기화
        //TryShowWish();             // 변경 직후 UI 즉시 반영
        RestartCooldownSchedule(); // 새 펫 기준으로 스케줄 재계산
    }

    //===============================================

    private void ResetGiftState() // 펫 변경 전용 초기화
    {
        _islandData.CurWish = Gift.None; //위시 제거

        _islandData.GiftCooldownStartTime = _cooldownService.RecordGiftTime(); //쿨타임 초기화
        _visual.CloseWishBubble(); //위시 닫기
    }

    // ================= 유틸 =================
    private List<Gift> GetGiftList()
    {
        var list = new List<Gift>();
        var values = (Gift[])Enum.GetValues(typeof(Gift));

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == Gift.None || values[i] == Gift.MasterGift)
            {
                continue;
            }
            list.Add(values[i]);
        }

        return list;
    }

}
