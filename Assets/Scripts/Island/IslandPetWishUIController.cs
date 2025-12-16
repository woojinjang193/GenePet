using UnityEngine;

public class IslandPetVisualController : MonoBehaviour
{
    [SerializeField] private WishBubble _wishBubble; //위시 버블
    [SerializeField] private IslandPetMouthController _mouth; //입 애니메이션

    public void ShowWish(Sprite sprite)
    {
        _wishBubble.Init(sprite); //위시 아이콘 표시
    }

    public void PlaySuccess()
    {
        _wishBubble.GiftGiven(); //위시 제거
        _mouth.StartAnimation(true); //성공 애니메이션
    }

    public void PlayFail()
    {
        _mouth.StartAnimation(false); //실패 애니메이션
    }

    public IslandPetMouthController Mouth => _mouth; //외부 이벤트 연결용
}
