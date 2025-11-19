using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveToIslandButton : MonoBehaviour
{
    private Button _islandButton;

    private void Awake()
    {
        _islandButton = GetComponent<Button>();
        _islandButton.onClick.AddListener(MoveToIsland);
    }

    private void MoveToIsland()
    {
        UserData uerData = Manager.Save.CurrentData.UserData;

        bool isIslandOpen = uerData.Island.IsOpen;

        if (isIslandOpen) //이미 열려있을 때
        {
            Manager.Save.SaveGame();
            SceneManager.LoadScene("IslandScene");
            return;
        }

        if (uerData.Items.IslandTicket > 0) //새로 열어야 할 때
        {
            uerData.Items.IslandTicket--;

            Debug.Log($"섬으로 이동. 티켓 {uerData.Items.IslandTicket}남음");
            uerData.Island.IsOpen = true;
            SceneManager.LoadScene("IslandScene");
            return;
        }

        if (uerData.Items.IslandTicket <= 0)
        {
            Debug.Log("티켓이 부족합니다");
        }
    }
}
