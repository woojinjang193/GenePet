using UnityEngine;
using System.Collections.Generic;

public class GiftWishController
{
    private List<Gift> _possibleGifts; //가능한 선물 목록

    public GiftWishController(List<Gift> gifts)
    {
        _possibleGifts = gifts; //선물 목록 저장
    }

    public Gift CreateWish()
    {
        int rand = Random.Range(0, _possibleGifts.Count); //랜덤 인덱스
        Gift newWish = _possibleGifts[rand]; //현재 위시 설정
        return newWish; //위시 반환
    }
}
