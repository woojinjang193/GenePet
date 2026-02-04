using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStep : MonoBehaviour// 액션(Action)들을 실행하고 조건(Condition)을 대기하다가 완료되면 콜백 호출

{
    [Header("즉시 실행")]
    [SerializeField] private List<TutorialActionBase> _actions = new(); // 실행할 액션

    [Header("완료 조건")]
    [SerializeField] private List<TutorialConditionBase> _conditions = new(); // 완료를 판단할 조건 조립

    [Header("자식도 검색할지 여부")]
    [SerializeField] private bool _includeChildren = false;

    [Header("다중 컨디션 여부")]
    [SerializeField] private bool _requireAllConditions = false; // false=OR(하나라도), true=AND(전부)

    private Action _onCompleted;      // 완료 시 호출할 콜백(Chunk가 전달)
    private bool _isRunning = false;
    private int _completedConditionCount = 0; // AND 모드에서 완료된 조건 카운트

    private void Awake()
    {
        CollectComponents(); // 런타임 자동 수집
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        CollectComponents(); // 에디터에서 값 변경 시 자동 수집
    }
#endif
    // Action/Condition을 자동으로 찾아 리스트에 채움
    private void CollectComponents()
    {
        // Actions 수집
        _actions.Clear(); // 기존 리스트 초기화
        if (_includeChildren)
            _actions.AddRange(GetComponentsInChildren<TutorialActionBase>(true)); //자식 포함(비활성 포함)
        else
            _actions.AddRange(GetComponents<TutorialActionBase>()); // 본인 오브젝트만

        // Conditions 수집
        _conditions.Clear(); //기존 리스트 초기화
        if (_includeChildren)
            _conditions.AddRange(GetComponentsInChildren<TutorialConditionBase>(true)); //자식 포함(비활성 포함)
        else
            _conditions.AddRange(GetComponents<TutorialConditionBase>()); // 본인 오브젝트만

        //_actions.RemoveAll(a => a == null);  // null 제거
        //_conditions.RemoveAll(c => c == null);    // null 제거
    }
    public void Enter(Action onCompleted)  //스텝 시작
    {
        if (_isRunning) return;

        _isRunning = true;
        _onCompleted = onCompleted;//완료 콜백 저장
        _completedConditionCount = 0;

        gameObject.SetActive(true);

        // 액션 먼저 실행
        for (int i = 0; i < _actions.Count; i++)
        {
            var act = _actions[i];
            if (act == null) continue;  // null 액션 스킵
            act.Execute();  // 즉시 실행
        }

        //// 조건이 없다면 즉시 완료
        //if (_conditions.Count == 0)
        //{
        //    CompleteStep(); // [추가] 조건 없으면 바로 다음으로
        //    return;
        //}

        // 조건 구독 시작
        for (int i = 0; i < _conditions.Count; i++)
        {
            var cond = _conditions[i];
            if (cond == null) continue; // null 조건 스킵

            cond.Begin(HandleConditionMet); //조건이 만족되면 콜백 받음
        }
    }

    public void Exit()  // Chunk가 호출: 스텝 종료(정리)
    {
        if (!_isRunning)
        {
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < _conditions.Count; i++)
        {
            var cond = _conditions[i];
            if (cond == null) continue;
            cond.End();
        }

        _isRunning = false;
        _onCompleted = null;
        _completedConditionCount = 0;

        gameObject.SetActive(false); //스텝 오브젝트 비활성화
    }
    private void HandleConditionMet(TutorialConditionBase who)  // Condition이 만족되면 여기로 들어옴
    {
        if (!_isRunning) return;

        if (_requireAllConditions)
        {
            // AND: 모든 조건 완료시 완료
            _completedConditionCount++;

            if (_completedConditionCount >= CountValidConditions())
            {
                CompleteStep(); // 전부 만족되면 완료
            }
        }
        else
        {
            //OR: 하나라도 만족되면 즉시 완료
            CompleteStep();
        }
    }

    // 스텝 완료 처리
    private void CompleteStep()
    {
        if (!_isRunning) return;

        // 조건 정리
        for (int i = 0; i < _conditions.Count; i++)
        {
            var cond = _conditions[i];
            if (cond == null) continue;
            cond.End();
        }

        _isRunning = false;

        // 완료 콜백 호출 (Chunk.NextStep) ????????????????
        var cb = _onCompleted; // 지역 변수로 보관(재진입 방지)
        _onCompleted = null;   // [추가] 참조 해제
        cb?.Invoke();          // [추가] 다음 스텝으로
    }

    // AND 모드에서 유효한 조건 개수 계산
    private int CountValidConditions()
    {
        int count = 0;
        for (int i = 0; i < _conditions.Count; i++)
        {
            if (_conditions[i] != null) count++; //null은 포함 안함
        }
        return count;
    }
}
