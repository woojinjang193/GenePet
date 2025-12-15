using System;
using UnityEngine;

public class IslandPetController : MonoBehaviour
{
    [Header("선물 스프라이트")]
    [SerializeField] private Sprite _gift1;
    [SerializeField] private Sprite _gift2;
    [SerializeField] private Sprite _gift3;
    [SerializeField] private Sprite _gift4;

    [Header("위시버블 컴포넌트")]
    [SerializeField] private WishBubble _wishBubble;
    [Header("마우스 컨트롤러")]
    [SerializeField] private IslandPetMouthController _mouth;

    private IslandManager _islandManager;

    private Gift _curWish;
    private void Awake()
    {
        if (CanGiveGift())
        {
            _curWish = Manager.Save.CurrentData.UserData.Island.CurWish;
            _wishBubble.Init(GetSprite());
            _mouth.OnGiveTaken += OnGiveTaken;
        }
    }
    private void OnDestroy()
    {
        _mouth.OnGiveTaken -= OnGiveTaken;
    }
    private void Start()
    {
        _islandManager = FindObjectOfType<IslandManager>();
    }

    private bool CanGiveGift() //선물 주기 가능여부 확인
    {
        float giftCooldown = Manager.Game.Config.GiftCooldown;
        long lastTimeGiftGiven = Manager.Save.CurrentData.UserData.Island.LastGiftGivenTime;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if(now - lastTimeGiftGiven > giftCooldown)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private Sprite GetSprite() //선물 스프라이트 받아오기
    {
        switch (_curWish)
        {
            case Gift.None: return GetRandomGifg();
            case Gift.Fish: return _gift1;
            case Gift.Something: return _gift2;
            case Gift.Toy: return _gift3;
            case Gift.Younggi: return _gift4;
            default: return null;
        }
    }
    private Sprite GetRandomGifg() //바라는게 None일경우 랜덤 선물 선택
    {
        int rand = UnityEngine.Random.Range(0, 4);

        switch (rand)
        {
            case 0: _curWish = Gift.Fish; return _gift1;
            case 1: _curWish = Gift.Something; return _gift2;
            case 2: _curWish = Gift.Toy; return _gift3;
            case 3: _curWish = Gift.Younggi; return _gift4;
            default: _curWish = Gift.None; return null;
        }
    }
    private void OnGiveTaken(Gift gift)
    {
        if (_curWish != gift) { _mouth.StartAnimation(false); return; }
           
        //선물별로 호감도 수치 다르게 적용할거면 나중에 스위치문 추가 (위의 GetRandomGifg() 스위치문 복사해서 쓰면 됨)
        _wishBubble.GiftGiven();
        _mouth.StartAnimation(true);
        _islandManager.ChangeAffinity(10); // 필요하면 컨피그에 넣기
    }


    //private void OnTriggerEnter2D(Collider2D collision) //테그 수정해야함
    //{
    //    if (collision.CompareTag("Item1"))
    //    {
    //        _ogMouth = _mouth.sprite;
    //        _ogEye = _eye.sprite;
    //        _mouth.sprite = _openMouthSprite;
    //        _eye.sprite = _closeEyesSprite;
    //    }
    //    else if (collision.CompareTag("Item2"))
    //    {
    //        _ogMouth = _mouth.sprite;
    //        _ogEye = _eye.sprite;
    //        _mouth.sprite = _openMouthForSnackSprite;
    //        _eye.sprite = _closeEyesSprite;
    //
    //    }
    //    else if (collision.CompareTag("Medicine"))
    //    {
    //        _ogMouth = _mouth.sprite;
    //        _ogEye = _eye.sprite;
    //        _mouth.sprite = _openMouthForMedicine;
    //        _eye.sprite = _closeEyesWithTear;
    //    }
    //    else if (collision.CompareTag("CleaningTool"))
    //    {
    //        _ogEye = _eye.sprite;
    //        _eye.sprite = _closeEyesSprite;
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Food"))
    //    {
    //        _mouth.sprite = _ogMouth;
    //        _eye.sprite = _ogEye;
    //    }
    //    else if (collision.CompareTag("Snack"))
    //    {
    //        _mouth.sprite = _ogMouth;
    //        _eye.sprite = _ogEye;
    //
    //    }
    //    else if (collision.CompareTag("Medicine"))
    //    {
    //        _mouth.sprite = _ogMouth;
    //        _eye.sprite = _ogEye;
    //    }
    //    else if (collision.CompareTag("CleaningTool"))
    //    {
    //        _eye.sprite = _ogEye;
    //    }
    //}
}
