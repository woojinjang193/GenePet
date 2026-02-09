using TMPro;
using UnityEngine;

public class ItemForMiniGame : MonoBehaviour 
{
    [Header("아이템 아이콘")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("아이템 타입")]
    [SerializeField] private RewardType _reward;
    [SerializeField] private int _amount;

    [Header("TMP")]
    [SerializeField] private TMP_Text _amountText;

    public RewardType Reward => _reward;
    public int Amount => _amount;
    
    public void Init(RewardType reward, int amount)
    {
        _spriteRenderer.sprite = GetIcon(reward);
        _reward = reward;
        _amount = amount;
        gameObject.SetActive(true);

        if( _amountText != null )
        {
            _amountText.text = ""; // CHECK: 플렛폼 아이템 개수 초기화 안되는 문제 체크 (점프씬용)
        }

        if (_amountText != null && amount > 1)
        {
            _amountText.text = $"x{_amount.ToString()}";
            _amountText.gameObject.SetActive(true);
        }
    }

    public void ResetItem()
    {
        _spriteRenderer.sprite = null;
        _reward = RewardType.None;
        _amount = 0;
        gameObject.SetActive(true);

        if (_amountText != null)
        {
            _amountText.gameObject.SetActive(false);
        }
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
