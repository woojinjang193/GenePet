using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveToIslandButton : MonoBehaviour
{
    [SerializeField] private Button _islandButton;
    [SerializeField] private PetUnit _petUnit;

    private void Awake()
    {
        _islandButton.onClick.AddListener(MoveToIsland);
    }

    private void MoveToIsland()
    {
        UserData uerData = Manager.Save.CurrentData.UserData;

        if(!_petUnit.IsExciting || _petUnit.Status.GetFlag(PetFlag.IsLeft))
        {
            Debug.Log("펫이 없습니다.");
            return;
        }

        if (_petUnit.Status.Growth != GrowthStatus.Adult)
        {
            Debug.Log("아직 어른이 아닙니다");
            return;
        }
        bool isIslandOpen = uerData.Island.IsOpen;

        if (isIslandOpen)
        {
            Manager.Save.SaveGame();
            SceneManager.LoadScene("IslandScene");
            return;
        }
        
        

        if (uerData.Items.IslandTicket > 0)
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
