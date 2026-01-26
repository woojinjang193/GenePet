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
        _button.onClick.AddListener(ShowNext);
    }
    public void ShowNext() // 다음 보상 표시
    {
        Debug.Log("큐 스타트");

        if (!Manager.Item.HasReward()) // 남은 보상 없으면
        {
            Debug.Log("큐 종료");
            gameObject.SetActive(false); // 팝업 숨김
            //Manager.Item.ClearRewardQueue(); //큐 클리어(필요한가?)
            return;
        }

        if (!Manager.Item.TryDequeueReward(out RewardData reward)) return; //디큐할게 없으면 리턴

        if (reward.Category == RewardCategory.Egg) //알 보상처리
        {
            RarityType rarity = reward.Egg.PetSaveData.Rarity;
            _icon.sprite = Manager.Item.ItemImages.EggRaritySO.GetEggSprite(rarity); //알 스프라이트
            _amount.text = "";
        }
        if (MiniGameRewardPicker.TryGetRoomFromRewardType(reward.RewardType, out var room)) // 방 타입이면
        {
            _icon.sprite = _ItemsSO.GetItemSprite(reward.RewardType); //아이템 스프라이트
            _amount.text = $"Roooooom";
        }
        else if (reward.Category == RewardCategory.Item) //일반 아이템 보상처리
        {
            _icon.sprite = _ItemsSO.GetItemSprite(reward.RewardType); //아이템 스프라이트
            _amount.text = $"x {reward.Amount}";
        }
    }
}
