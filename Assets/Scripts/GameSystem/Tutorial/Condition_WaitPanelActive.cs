using UnityEngine;
// 특정 패널(GameObject)의 활성/비활성을 코루틴으로 감시해서 조건을 만족시키는 조건
using System.Collections;
public class Condition_WaitPanelActive : TutorialConditionBase
{
    [Header("타겟")]
    [SerializeField] private GameObject _panel; // 감시할 패널

    [Header("감지 할 동작")]
    [SerializeField] private bool _waitForActive = true; // true=켜짐을 기다림, false=꺼짐을 기다림

    private Coroutine _watchRoutine; // 감시 코루틴

    protected override void OnBegin()
    {
        if (_panel == null) return;
        _watchRoutine = StartCoroutine(Watch());
    }

    protected override void OnEnd()
    {
        if (_watchRoutine != null) // 코루틴이 돌고 있으면 중지
        {
            StopCoroutine(_watchRoutine);
            _watchRoutine = null;
        }
    }

    private IEnumerator Watch()
    {
        while (true)
        {
            bool isActive = _panel.activeInHierarchy; //현재 활성 상태 확인(상위 포함)
            if (isActive == _waitForActive) // 원하는 상태가 되면
            {
                Met(); 
                yield break;
            }

            yield return null;
        }
    }
}
