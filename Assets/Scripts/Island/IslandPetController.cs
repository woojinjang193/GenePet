using UnityEngine;
using System.Collections.Generic;
using System;

public class IslandPetController : MonoBehaviour
{
    [SerializeField] private IslandPetVisualController _visual; //비주얼 컨트롤러

    private IslandManager _islandManager; //섬 매니저
    private GiftCooldownService _cooldownService; //쿨타임 서비스
    private GiftWishController _wishController; //위시 로직

    private Gift _currentWish; //현재 위시

    private void Awake()
    {
        _islandManager = FindObjectOfType<IslandManager>(); //섬 매니저 찾기
        _cooldownService = new GiftCooldownService(Manager.Game.Config.GiftCooldown); //쿨타임 초기화
        _wishController = new GiftWishController(GetGiftList()); //가능한 선물 목록

        var islandData = Manager.Save.CurrentData.UserData.Island;

        long requestStartTime = islandData.GiftRequestStartTime; //퀘스트 요청시간
        bool canGiveGift = _cooldownService.CanGiveGift(requestStartTime); //쿨타임 완료 여부
        bool hasPendingWish = islandData.CurWish != Gift.None; //미지급 위시 존재 여부

        if (canGiveGift) // 쿨타임 완료 상태
        {
            if (hasPendingWish)
            {
                // 선물 안 준 채로 쿨타임 넘김 > 패널티
                Debug.Log("선물 안 준 채로 쿨타임 넘김");
                _islandManager.ChangeAffinity(-Manager.Game.Config.DisappointingPoint);
            }

            // 새 위시 생성
            _currentWish = _wishController.CreateWish();
            islandData.CurWish = _currentWish;

            islandData.GiftRequestStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //시간 저장
            Sprite wishSprite = Manager.Item.ItemImages.GetGiftSprite(_currentWish);
            _visual.ShowWish(wishSprite);
        }
        else
        {
            // 쿨타임 미완료 상태
            if (hasPendingWish)
            {
                // 저장된 위시 그대로 표시
                _currentWish = islandData.CurWish;
                Sprite wishSprite = Manager.Item.ItemImages.GetGiftSprite(_currentWish);
                _visual.ShowWish(wishSprite);
            }
            // else : 이미 선물 준 상태 > 아무것도 안 함
        }

        _visual.Mouth.OnGiveTaken += OnGiveTaken; //이벤트 구독
    }

    private void OnDestroy()
    {
        _visual.Mouth.OnGiveTaken -= OnGiveTaken; //이벤트 해제
    }

    private void OnGiveTaken(Gift gift)
    {
        var islandData = Manager.Save.CurrentData.UserData.Island;

        if (!_wishController.IsCorrect(gift)) //선물 불일치
        {
            _visual.PlayFail(); //실패 연출
            Debug.Log("선물 실패");
            return;
        }

        _visual.PlaySuccess(); //성공 연출
        Debug.Log("선물 성공");

        int affinity = _wishController.GetAffinity(); //호감도 획득
        _islandManager.ChangeAffinity(affinity); //호감도 적용

        islandData.CurWish = Gift.None; //원하는 선물 None으로 변경
        islandData.GiftRequestStartTime = _cooldownService.RecordGiftTime(); //선물 시간 기록
    }

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
