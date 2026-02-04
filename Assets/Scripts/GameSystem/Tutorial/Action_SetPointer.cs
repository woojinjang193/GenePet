using UnityEngine;

// 포인터를 특정 부모로 붙이고 방향/애니매니션를 설정하는 액션
public class Action_SetPointer : TutorialActionBase 
{
    [Header("포인터")]
    [SerializeField] private TutorialPointer _pointer;

    [Header("부모")]
    [SerializeField] private Transform _parent; // 포인터가 붙을 부모

    [Header("연출")]
    [SerializeField] private PointerDir _dir = PointerDir.Up; // 화살표 방향
    [SerializeField] private bool _animated = true;           // 애니 사용 여부

    public override void Execute()
    {
        if (_pointer == null) return;
        if (_parent == null) return;

        _pointer.SetPointer(_dir, _parent, _animated);
    }
}
