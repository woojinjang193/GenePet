using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialChunk : MonoBehaviour //하나의 튜토리얼 스텝 리스트를 순서대로 실행하고 완료알림
{
    public event Action<TutorialChunk> OnChunkCompleted;// 청크 완료 통지(컨트롤러가 구독)

    [Header("Steps")]
    [SerializeField] private List<TutorialStep> _steps = new(); //인스펙터에서 스텝 조립

    [Header("스킵/완료시 정리할 액션들)")]
    [SerializeField] private List<TutorialActionBase> _cleanupActions = new(); // 스킵/완료 시 원복 액션들


    private int _index = -1; //현재 스텝 인덱스
    private TutorialStep _runningStep; //현재 실행 중 스텝 참조
    private bool _isRunning = false;
    public bool IsRunning => _isRunning;
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
    // TutorialStep을 자동으로 찾아 리스트에 채움
    private void CollectComponents()
    {
        // Actions 수집
        _steps.Clear(); // 기존 리스트 초기화
        _steps.AddRange(GetComponentsInChildren<TutorialStep>(true)); //자식 검색(비활성 포함)

        _steps.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex())); // 순서 고정
    }
    public void StartChunk() // 컨트롤러가 호출하는 시작 함수
    {
        if (_isRunning) return;
        _isRunning = true;

        gameObject.SetActive(true); // 튜토리얼 활성화

        _index = -1;
        NextStep();
    }

    public void StopChunk() // 강제 종료 호출
    {
        if (!_isRunning) return;

        CleanupRunningStep();
        ExecuteCleanupActions(); //상태 원복

        _isRunning = false;
        _index = -1;

        //gameObject.SetActive(false);
    }

    // 다음 스텝으로 진행
    private void NextStep()
    {
        CleanupRunningStep(); // 이전 스텝 Exit/이벤트 해제

        _index++;

        if (_index >= _steps.Count)
        {
            FinishChunk(); // 완료 이벤트 발생
            return;
        }

        _runningStep = _steps[_index];
        if (_runningStep == null) // 스텝이 비어있으면 다음으로 스킵
        {
            NextStep();
            return;
        }

        // 스텝 시작: 스텝이 끝나면 NextStep을 호출하도록 콜백 전달
        _runningStep.Enter(NextStep);
    }

    // 덩어리 완료 처리
    private void FinishChunk()
    {
        CleanupRunningStep();
        ExecuteCleanupActions();

        _isRunning = false;
        _index = -1;

        //gameObject.SetActive(false);

        OnChunkCompleted?.Invoke(this); //컨트롤러에 완료 알림
    }

    // 현재 실행 중 스텝 정리(Exit 호출 포함)
    private void CleanupRunningStep()
    {
        if (_runningStep == null) return;
        _runningStep.Exit(); // 스텝 내부 코루틴/리스너 정리
        _runningStep = null;
    }
    // Stop/Finish에서 항상 실행되는 원복 액션 실행
    private void ExecuteCleanupActions()
    {
        for (int i = 0; i < _cleanupActions.Count; i++)
        {
            var act = _cleanupActions[i];
            if (act == null) continue;
            act.Execute(); // 원복 실행(즉시)
        }
        gameObject.SetActive(false);
    }
}
