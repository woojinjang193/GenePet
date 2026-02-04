using System;
using UnityEngine;

public abstract class TutorialConditionBase : MonoBehaviour //스텝 완료를 결정하는 조건의 공통 베이스(구독/해제 패턴)
{
    protected Action<TutorialConditionBase> _onMet; //조건 만족 시 TutorialStep에 알릴 콜백

    private bool _isActive = false;
    private bool _isMet = false;

    // 스텝이 시작될 때 호출: 조건 감시 시작
    public void Begin(Action<TutorialConditionBase> onMet)
    {
        if (_isActive) return; // 이미 감시 중이면 무시

        _isActive = true;
        _isMet = false;
        _onMet = onMet;// 콜백 저장

        OnBegin(); // 자식 클래스에서 구독/코루틴 시작
    }

    // 스텝이 끝날 때 호출: 조건 감시 종료
    public void End()
    {
        if (!_isActive) return; // 감시 중이 아니면 무시

        _isActive = false;
        OnEnd();

        _onMet = null;
    }

    // 자식 조건에서 조건 만족 시 호출해야 하는 공통 함수
    protected void Met()
    {
        if (!_isActive) return;
        if (_isMet) return;  // 이미 만족 처리했으면 무시

        _isMet = true;
        _onMet?.Invoke(this);
    }

    protected abstract void OnBegin(); // 감시 시작(리스너 구독/코루틴 시작 등)
    protected abstract void OnEnd();   // 감시 종료(리스너 해제/코루틴 종료 등)
}
