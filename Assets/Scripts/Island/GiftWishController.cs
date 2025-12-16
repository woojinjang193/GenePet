using UnityEngine;
using System.Collections.Generic;

public class GiftWishController
{
    private List<Gift> _possibleGifts; //가능한 선물 목록
    private Gift _currentWish; //현재 원하는 선물
    private int _affinityValue; //현재 선물 호감도

    public GiftWishController(List<Gift> gifts)
    {
        _possibleGifts = gifts; //선물 목록 저장
    }

    public Gift CreateWish()
    {
        int rand = Random.Range(0, _possibleGifts.Count); //랜덤 인덱스
        _currentWish = _possibleGifts[rand]; //현재 위시 설정
        _affinityValue = 10; //임시 호감도 값
        return _currentWish; //위시 반환
    }

    public bool IsCorrect(Gift givenGift)
    {
        return _currentWish == givenGift; //선물 일치 여부 반환
    }

    public int GetAffinity()
    {
        _currentWish = Gift.None;
        return _affinityValue; //호감도 반환
    }
}
