using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardPopUp : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _amount;
    
    private ItemsSO _ItemsSO;

    private Queue<object> _rewardQueue;

    private void Awake()
    {
        if(Manager.Item != null)
        {
            _ItemsSO = Manager.Item.ItemImages;
        }
  
        if (_button == null)
        {
            _button = GetComponentInChildren<Button>();
        }
        _rewardQueue = Manager.Item.RewardQueue;
        _button.onClick.AddListener(ShowNext);

        Debug.Log($"보상 개수: {_rewardQueue.Count}");
    }
    public void ShowNext() // 다음 보상 표시
    {
        Debug.Log("큐 스타트");

        if (_rewardQueue.Count == 0) // 남은 보상 없으면
        {
            Debug.Log("큐 종료");
            gameObject.SetActive(false); // 팝업 숨김
            Manager.Item.ClearRewardQueue(); //큐 클리어
            return;
        }

        object reward = _rewardQueue.Dequeue(); //다음 보상 꺼냄

        if (reward is EggData egg) //알 보상 처리
        {
            _icon.sprite = egg.PetSaveData.EggSprite;
            _amount.text = "";
        }
        else if (reward is System.ValueTuple<RewardType, int> item) //일반 보상 처리
        {
            _icon.sprite = GetSprite(item.Item1);
            _amount.text = $"X {item.Item2}";
        }
    }
    public Sprite GetSprite(RewardType type)
    {
        switch (type)
        {
            case RewardType.Coin: return _ItemsSO.CoinSprite;
            case RewardType.Energy: return _ItemsSO.Energy;
            case RewardType.RemovedAD: return _ItemsSO.RemoveAdSprite;
            case RewardType.IslandTicket: return _ItemsSO.IslandTicketSprite;
            case RewardType.MissingPoster: return _ItemsSO.MissingPosterSprite;
            case RewardType.GeneticScissors: return _ItemsSO.GeneticScissorsSprite;
            case RewardType.GeneticTester: return _ItemsSO.geneticTesterSprite;
            case RewardType.Snack: return _ItemsSO.SnackSprite;
        }
        return null;
    }
}
