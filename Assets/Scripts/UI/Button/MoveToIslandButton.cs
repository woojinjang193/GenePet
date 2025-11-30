using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveToIslandButton : MonoBehaviour
{
    private Button _islandButton;
    [SerializeField] private PetManager _petManager;

    private void Awake()
    {
        if(_petManager == null)
        {
            _petManager = FindObjectOfType<PetManager>();
        }    
        _islandButton = GetComponent<Button>();
        _islandButton.onClick.AddListener(MoveToIsland);
    }

    private void MoveToIsland()
    {
        UserData userData = Manager.Save.CurrentData.UserData;

        var petList = _petManager.ActivePets;

        bool hasAdult = false;

        for(int i = 0;  i < petList.Count; i++)
        {
            if(petList[i].Status.Growth == GrowthStatus.Adult)
            {
                Debug.Log("어른 확인");
                hasAdult = true;
                break;
            }
        }

        if (!hasAdult)
        {
            Debug.Log("섬은 어른만 갈 수 있음");
            return;
        }
        bool isIslandOpen = userData.Island.IsOpen;

        if (isIslandOpen) //이미 열려있을 때
        {
            Manager.Save.SaveGame();
            SceneManager.LoadScene("IslandScene");
            return;
        }

        if (userData.Items.IslandTicket > 0) //새로 열어야 할 때
        {
            userData.Items.IslandTicket--;

            Debug.Log($"섬으로 이동. 티켓 {userData.Items.IslandTicket}남음");
            userData.Island.IsOpen = true;
            SceneManager.LoadScene("IslandScene");
            return;
        }

        if (userData.Items.IslandTicket <= 0)
        {
            Debug.Log("티켓이 부족합니다");
        }
    }
}
