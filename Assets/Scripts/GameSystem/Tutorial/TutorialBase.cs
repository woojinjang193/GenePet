using System;
using UnityEngine;

public abstract class TutorialBase : MonoBehaviour
{
    public Action OnCompleted;
    public abstract void Enter(); //해당 튜토리얼 과정을 시작할 때 1회 호출
    public abstract void Exit(); //튜토리얼 과정 종료할때 1회 호출

    protected void Complete()         // [추가] 튜토리얼 종료 시점에 호출
    {
        OnCompleted?.Invoke();        // [추가] 매니저에 "끝남" 알림
    }
}
[Serializable]
public class LineInfo
{
    public string NameID;
    public string TextID;
    public Sprite Sprite;
    public Transform PointerPos;
    public PointerDir PointerDir = PointerDir.None;
    public bool PointerAnim = false;
}