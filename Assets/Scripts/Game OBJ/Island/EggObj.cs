using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EggObj : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    
    private EggData _data;
    public void Init(EggData egg)
    {
        _sprite.sprite = egg.PetSaveData.EggSprite;
        _data = egg;
    }

    public void OnMouseDown() //알 획득시
    {
        var eggHaveList = Manager.Save.CurrentData.UserData.EggList;
        int maxHave = Manager.Game.Config.MaxEggAmount;
        
        if (maxHave <= eggHaveList.Count) //알을 더이상 가질 수 없을때
        {
            Manager.Game.ShowPopup("You have too many eggs");
            Debug.Log($"알을 더이상 가질 수 없음. 현재: {eggHaveList.Count}");
            return;
        }

        //알 획득 
        eggHaveList.Add(_data); //리스트에 추가
        Manager.Item.EnqueueEgg(_data); //알을 보상 큐로 전달

        Manager.Save.RemoveIsland();// 섬 삭제

        _data = null;
        _sprite.sprite = null;
        Debug.Log($"알 추가. 현재: {eggHaveList.Count}");

        //알 획득 연출 넣을거면 여기서

        SceneManager.LoadScene("InGameScene");
    }
}
