using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstIslandObserver : MonoBehaviour
{
    [SerializeField] private TutorialController _controller;

    private void Start()
    {
        if (!Manager.Save.CurrentData.UserData.tutorialFlags.FirstIsland)
            _controller.TryStartTutorial(TutorialTriggerKey.FirstIsland);
    }
}
