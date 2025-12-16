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

        long lastGiftTime = Manager.Save.CurrentData.UserData.Island.LastGiftGivenTime; //마지막 선물 시간

        if (!_cooldownService.CanGiveGift(lastGiftTime)) return; //쿨타임 미완료면 종료

        _currentWish = _wishController.CreateWish(); //위시 생성

        Sprite wishSprite = Manager.Item.ItemImages.GetGiftSprite(_currentWish); //아이콘 가져오기
        _visual.ShowWish(wishSprite); //위시 표시

        _visual.Mouth.OnGiveTaken += OnGiveTaken; //선물 전달 이벤트 구독
    }

    private void OnDestroy()
    {
        _visual.Mouth.OnGiveTaken -= OnGiveTaken; //이벤트 해제
    }

    private void OnGiveTaken(Gift gift)
    {
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

        Manager.Save.CurrentData.UserData.Island.LastGiftGivenTime = _cooldownService.RecordGiftTime(); //선물 시간 기록
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
