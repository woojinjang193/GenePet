using UnityEngine;
// GameObject 활성/비활성을 즉시 설정하는 액션
public class Action_SetActive : TutorialActionBase
{
    [Header("GO")]
    [SerializeField] private GameObject _target; // 대상 오브젝트

    [Header("활성화")]
    [SerializeField] private bool _active = true; // 활성화 여부

    public override void Execute()
    {
        if (_target == null) return;
        _target.SetActive(_active);  //활성/비활성 적용
    }
}
