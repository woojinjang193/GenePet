using System;
using TMPro;
using UnityEngine;

public enum BrickColor { None, one, two, three }

[Serializable]
public struct BrickData
{
    public BrickColor ColorName;
    public Color Color;
    public LevelReward Reward;
    public int Score;
}

public class PinballBrick : MonoBehaviour
{
    [Header("비주얼")]
    [SerializeField] private SpriteRenderer _brickRenderer;
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TMP_Text _amountText;

    [Header("HP")]
    [SerializeField] private int _hp = 1;

    private BrickData _data;
    private int _curHp;

    // ======= 이벤트 ========
    public event Action<int> OnAddScore;
    public event Action<BrickColor, LevelReward> OnGiveItem;
    public event Action<PinballBrick> OnBroken; //파괴시 연출 이벤트

    public void Init(BrickData data)
    {
        _data = data;

        _curHp = Mathf.Max(1, _hp);

        _brickRenderer.color = _data.Color; //컬러 세팅

        if(Manager.Item != null && _data.Reward.RewardType != RewardType.None)
        {
            _iconRenderer.sprite = Manager.Item.ItemImages.GetItemSprite(_data.Reward.RewardType);
        }

        if(_data.Reward.RewardType == RewardType.None)
        {
            _iconRenderer.gameObject.SetActive(false);
        }

        if (_data.Reward.Amount != 0) // 개수 표시
        {
            _amountText.text = _data.Reward.Amount.ToString();
        }
        
    }

    public void Hit()
    {
        _curHp--;
        if (_curHp > 0)
        {
            // TODO: 히트 연출(깜빡/사운드)
            return;
        }

        Break();
    }

    private void Break()
    {
        OnAddScore?.Invoke(_data.Score); // 점수 이벤트 발생
        OnGiveItem?.Invoke(_data.ColorName, _data.Reward); //아이템 이벤트 발생

        OnBroken?.Invoke(this); //파괴시 연출 이벤트 발생
        gameObject.SetActive(false); // 비활성화
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Ball"))
        {
            Hit();
        }
    }
}
