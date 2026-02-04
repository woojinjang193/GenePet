using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<TutorialBase> _tutorial1; //Main:첫 방문 ~ 부화
    [SerializeField] private List<TutorialBase> _tutorial2; //Main:첫 어른 이벤트
    [SerializeField] private List<TutorialBase> _tutorial3; //

    private int _curIndex = -1;
    private List<TutorialBase> _curTutorial = new();
    private void Awake()
    {
        _curTutorial = _tutorial1; //테스트용 나중에 이벤트 받아서 튜토리얼 세팅하도록 바꿔야함
        NextTutorial();
    }

    public void NextTutorial()
    {
        // 이전 튜토리얼 정리
        if (_curIndex >= 0)
        {
            var prev = _curTutorial[_curIndex];
            prev.OnCompleted -= OnCompleted; //이벤트 해제
            prev.Exit();
        }

        if (_curIndex >= _curTutorial.Count - 1)
        {
            _curTutorial.Clear();
            Debug.Log("튜토리얼 끝");
            gameObject.SetActive(false);
            return;
        }

        _curIndex++;
        var cur = _curTutorial[_curIndex];
        cur.OnCompleted += OnCompleted; // 완료 이벤트 구독
        cur.Enter();
    }

    // 완료되면 다음으로
    private void OnCompleted()
    {
        NextTutorial();
    }
}
