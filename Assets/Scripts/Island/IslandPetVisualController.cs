using System.Collections;
using UnityEngine;

public class IslandPetVisualController : MonoBehaviour
{

    [Header("호감도 up/down 파티클")]
    [SerializeField] private ParticleSystem _particleAffinityUp;
    [SerializeField] private ParticleSystem _particleAffinityDown;

    [Header("펫 떠남/알남 파티클")] //TODO: 안쓰면 지우기
    [SerializeField] private ParticleSystem _particleLeave;
    [SerializeField] private ParticleSystem _particleLayEgg;

    [Header("위시 버블")]
    [SerializeField] private WishBubble _wishBubble;
    [Header("입 애니메이션")]
    [SerializeField] private IslandPetMouthController _mouth;

    public IslandPetMouthController Mouth => _mouth; //외부 이벤트 연결용

    public void ShowWish(Sprite sprite)
    {
        _wishBubble.Init(sprite); //위시 아이콘 표시
    }

    public void GiftSuccess()
    {
        _wishBubble.GiftGiven(); //위시 제거
        _mouth.StartAnimation(true); //성공 애니메이션
    }

    public void GiftFail()
    {
        _mouth.StartAnimation(false); //실패 애니메이션
    }
    
    public void CloseWishBubble()
    {
        _wishBubble.gameObject.SetActive(false);
    }

    //==============호감도 변경 이벤트================
    public void ShowAffinityChangeEffect(bool isUp) // true = 호감도 오름, false = 내려감
    {
        ParticleSystem particle;
        particle = isUp ? _particleAffinityUp : _particleAffinityDown;

        particle.Emit(30);
    }
    public void ShowLayOrLeaveEffect(bool isMarried) //true = 알 , false = 떠남
    {
        ParticleSystem particle;
        particle = isMarried ? _particleLayEgg : _particleLeave;

        particle.Emit(30);
    }
}
