using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoHomeInTheMiddleOfPlaying : MonoBehaviour, IConfirmRequester
{
    [SerializeField] private Button _goHomeButton;


    private void Awake()
    {
        _goHomeButton.onClick.AddListener(TryGoHome);
    }
    private void TryGoHome()
    {
        Manager.Game.ShowConfirmMessage("Warning_GoBackHome",0, this);
    }
    public void Confirmed(int requestNum)
    {
        if(requestNum == 0)
        {
            Manager.Mini.EndMiniGame();
        }
    }
    public void Canceled(int requestNum)
    {
    }
}
