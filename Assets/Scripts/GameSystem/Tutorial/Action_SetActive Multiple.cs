using UnityEngine;

public class Action_SetActiveMultiple : TutorialActionBase
{
    [Header("GO")]
    [SerializeField] private GameObject[] _targets; // 대상 오브젝트들

    [Header("활성화")]
    [SerializeField] private bool _active = true; // 활성화 여부

    public override void Execute()
    {
        if (_targets == null || _targets.Length == 0) return;
        
        for(int i = 0; i < _targets.Length; i++)
        {
            var target = _targets[i];
            if(target == null) continue; // 비었으면 다음
            if(target.gameObject == this) continue; //자신이면 다음

            target.SetActive(_active);
        }
    }
}
