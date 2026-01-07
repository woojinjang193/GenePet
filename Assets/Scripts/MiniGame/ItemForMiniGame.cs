using UnityEngine;

public class ItemForMiniGame : MonoBehaviour
{
    [Header("아이템 타입")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("아이템 타입")]
    [SerializeField] private RewardType _reward;
    [SerializeField] private int _amount;

    public RewardType Reward => _reward;
    public int Amount => _amount;
    
    public void Init(Sprite icon, RewardType reward, int amount)
    {
        _spriteRenderer.sprite = icon;
        _reward = reward;
        _amount = amount;
        gameObject.SetActive(true);
    }
}
