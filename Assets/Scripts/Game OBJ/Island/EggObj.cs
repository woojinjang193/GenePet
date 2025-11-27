using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggObj : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    
    private EggData _data;
    public void Init(EggData egg)
    {
        _sprite.sprite = egg.Image;
        _data = egg;
    }

    public void OnMouseDown()
    {
        var eggHaveList = Manager.Save.CurrentData.UserData.EggList;
        int maxHave = Manager.Game.Config.MaxEggAmount;
        
        if (maxHave <= eggHaveList.Count)
        {
            Debug.Log($"알을 더이상 가질 수 없음. 현재: {eggHaveList.Count}");
        }
        else
        {
            eggHaveList.Add(_data);

            Manager.Save.RemoveIsland();

            var background = FindObjectOfType<RewardBackground>(true);
            background.gameObject.SetActive(true);
            background.SetImage(_sprite.sprite);
            
            _data = null;
            _sprite.sprite = null;
            gameObject.SetActive(false);
            Debug.Log($"알 추가. 현재: {eggHaveList.Count}");
        }
    }
}
