
using System;
using UnityEngine;

public class Action_SetTutorialFlag : TutorialActionBase
{
    [Header("튜토리얼 키")]
    [SerializeField] private TutorialTriggerKey _key;
    [SerializeField] private bool _completed;
    public override void Execute()
    {
        if (_key == TutorialTriggerKey.None) return;

        var tutorialFlags = Manager.Save.CurrentData.UserData.tutorialFlags;

        switch(_key)
        {
            case TutorialTriggerKey.FirstVisit: tutorialFlags.FirstVisit = _completed; break;
            case TutorialTriggerKey.FirstGeneEdit: tutorialFlags.FirstGeneEdit = _completed; break;
            case TutorialTriggerKey.FirstIsland: tutorialFlags.FirstIsland = _completed; break;
            case TutorialTriggerKey.FirstMissing: tutorialFlags.FirstMissing = _completed; break;
        }
    }

}
