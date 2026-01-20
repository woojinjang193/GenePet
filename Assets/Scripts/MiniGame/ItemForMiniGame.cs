using UnityEngine;

public class ItemForMiniGame : MonoBehaviour
{
    [Header("아이템 아이콘")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("아이템 타입")]
    [SerializeField] private RewardType _reward;
    [SerializeField] private int _amount;

    public RewardType Reward => _reward;
    public int Amount => _amount;
    
    public void Init(RewardType reward, int amount)
    {
        _spriteRenderer.sprite = GetIcon(reward);
        _reward = reward;
        _amount = amount;
        gameObject.SetActive(true);
    }

    private Sprite GetIcon(RewardType reward)
    {
        if(Manager.Item != null)
        {
            return Manager.Item.ItemImages.GetItemSprite(reward);
        }
        else
        {
            return null;
        }
    }
}
