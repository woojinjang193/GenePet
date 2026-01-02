using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandDeleteButton : MonoBehaviour, IConfirmRequester
{
    [SerializeField] private Button _button;
    [SerializeField] private ConfirmMessage _confirmMessage;

    private void Awake()
    {
        _button.onClick.AddListener(OnDeleteIslandClick);
    }

    private void OnDeleteIslandClick()
    {
        if (_confirmMessage == null)
        {
            _confirmMessage = FindObjectOfType<ConfirmMessage>(true);
        }
        _confirmMessage.OpenConfirmUI("Warning_DeleteIsland", this);
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
