using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    public void AddTicket(int amount)
    {
        Manager.Save.CurrentData.UserData.Items.IslandTicket += amount;
        Debug.Log($"섬티켓 추가 {Manager.Save.CurrentData.UserData.Items.IslandTicket}");
    }
    public void AddMissingPoster(int amount)
    {
        Manager.Save.CurrentData.UserData.Items.MissingPoster += amount;
        Debug.Log($"실종포스터 추가 {Manager.Save.CurrentData.UserData.Items.MissingPoster}");
    }

    private void Update() //테스트용. 삭제해야함
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Manager.Save.CurrentData.UserData.Items.IslandTicket += 1;
            Debug.Log($"섬티켓 추가 {Manager.Save.CurrentData.UserData.Items.IslandTicket}");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Manager.Save.CurrentData.UserData.Items.MissingPoster += 1;
            Debug.Log($"실종포스터 추가 {Manager.Save.CurrentData.UserData.Items.MissingPoster}");
        }
    }
}
