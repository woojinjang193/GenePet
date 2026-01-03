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
            Manager.Game.ShowWarning("Warning_DeleteIsland", this);
        }
    }

    public void Confirmed()
    {
        Manager.Save.RemoveIsland();
        SceneManager.LoadScene("InGameScene");
    }
    public void Canceled()
    {
        
    }
}
