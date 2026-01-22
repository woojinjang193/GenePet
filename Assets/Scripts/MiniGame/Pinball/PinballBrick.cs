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
    public int HP;
}

public class PinballBrick : MonoBehaviour
{
    [Header("비주얼")]
    [SerializeField] private SpriteRenderer _brickRenderer;
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TMP_Text _amountText;

    private BrickData _data;
    private int _hp;
    private int _curHp;

    // ======= 이벤트 ========
    public event Action<int> OnAddScore;
    public event Action<BrickColor, LevelReward, Vector3> OnGiveItem;
    public event Action<BrickColor, Vector3> OnBroken; //파괴시 연출 이벤트

    public void Init(BrickData data)
    {
        _data = data;
        _hp = data.HP;
        VisualSetting();
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
        Vector3 worldPos = transform.position;

        OnAddScore?.Invoke(_data.Score); // 점수 이벤트 발생
        OnBroken?.Invoke(_data.ColorName, worldPos); //파괴시 연출 이벤트 발생

        if(_data.Reward.RewardType != RewardType.None) //리워드가 있을때만 이벤트 발생
        {
            OnGiveItem?.Invoke(_data.ColorName, _data.Reward, worldPos); //아이템 이벤트 발생
        }

        gameObject.SetActive(false); // 비활성화
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Player"))
        {
            Hit();
        }
    }
    private void VisualSetting()
    {
        _curHp = Mathf.Max(1, _hp);

        _brickRenderer.color = _data.Color; //컬러 세팅

        if (Manager.Item != null && _data.Reward.RewardType != RewardType.None)
        {
            _iconRenderer.sprite = Manager.Item.ItemImages.GetItemSprite(_data.Reward.RewardType);
        }

        if (_data.Reward.RewardType == RewardType.None)
        {
            _iconRenderer.gameObject.SetActive(false);
        }

        if (_data.Reward.Amount > 1) // 개수 표시 (2개 아래면 끄기)
        {
            _amountText.text = $"x{_data.Reward.Amount.ToString()}";
        }
        else
        {
            _amountText.gameObject.SetActive(false);
        }
    }
}
