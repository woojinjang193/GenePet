using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandDeleteButton : MonoBehaviour, IConfirmRequester
{
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(OnDeleteIslandClick);
    }

    private void OnDeleteIslandClick()
    {
        if (Manager.Game != null)
        {
            Manager.Game.ShowConfirmMessage("Warning_DeleteIsland",0, this);
        }
    }

    public void Confirmed(int requestNum)
    {
        if(requestNum == 0) //아일랜드 삭제 경고
        {
            Manager.Save.RemoveIsland();
            SceneManager.LoadScene("InGameScene");
        }
    }
    public void Canceled(int requestNum)
    {
        
    }
}
