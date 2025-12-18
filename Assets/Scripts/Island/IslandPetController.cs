using UnityEngine;
using System.Collections.Generic;
using System;

public class IslandPetController : MonoBehaviour
{
    [SerializeField] private IslandPetVisualController _visual; //비주얼 컨트롤러

    private IslandManager _islandManager; //섬 매니저
    private GiftCooldownService _cooldownService; //쿨타임 서비스
    private GiftWishController _wishController; //위시 로직

    private IslandData _islandData;

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
    }
    private void OnDestroy()
    {
        _visual.Mouth.OnGiveTaken -= OnGiveTaken; //이벤트 해제
        _islandManager.OnIslandMyPetChange -= OnIslandMyPetChange;
    }
    private void TryShowWish()
    {
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

            return;
        }
        else
        {
            // 새 위시 생성
            _islandData.CurWish = _wishController.CreateWish();
            _islandData.GiftCooldownStartTime = _cooldownService.RecordGiftTime(); //쿨타임 초기화

            Sprite wishSprite = Manager.Item.ItemImages.GetGiftSprite(_islandData.CurWish);
            _visual.ShowWish(wishSprite);
        }
    }
                            
    //============이벤트 발생시==============
    private void OnGiveTaken(Gift gift) //선물 먹음 이벤트
    {
        if (_islandData.CurWish != gift) //선물 불일치
        {
            _visual.PlayFail(); //실패 연출
            Debug.Log("선물 실패");
            return;
        }

        _visual.PlaySuccess(); //성공 연출
        Debug.Log("선물 성공");

        _islandManager.ChangeAffinity(Manager.Game.Config.GiftingPoint); //호감도 적용

        ResetGiftState();
    }
    private void OnIslandMyPetChange() //펫 변경 이벤트
    {
        ResetGiftState();     // 펫 변경 시 초기화
        //TryShowWish();   // 쿨타임 끝났으면 위시 표시
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
            if (values[i] == Gift.None)
            {
                continue;
            }
            list.Add(values[i]);
        }

        return list;
    }

}
