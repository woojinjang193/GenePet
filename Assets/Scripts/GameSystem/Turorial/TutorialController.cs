using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<TutorialBase> _tutorials;

    private int _curIndex = -1;

    private void Awake()
    {
        NextTutorial();
    }

    public void NextTutorial()
    {
        // 이전 튜토리얼 정리
        if (_curIndex >= 0)
        {
            var prev = _tutorials[_curIndex];
            prev.OnCompleted -= OnCompleted; //이벤트 해제
            prev.Exit();
        }

        if (_curIndex >= _tutorials.Count - 1)
        {
            Debug.Log("튜토리얼 끝");
            return;
        }

        _curIndex++;
        var cur = _tutorials[_curIndex];
        cur.OnCompleted += OnCompleted; // 완료 이벤트 구독
        cur.Enter();
    }

    // 완료되면 다음으로
    private void OnCompleted()
    {
        NextTutorial();
    }
}
