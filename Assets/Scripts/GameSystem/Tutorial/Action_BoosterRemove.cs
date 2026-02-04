using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BoosterRemove : TutorialActionBase
{
    public override void Execute()
    {
        Manager.Save.CurrentData.UserData.Items.GrowthBooster = 0;
    }
}
