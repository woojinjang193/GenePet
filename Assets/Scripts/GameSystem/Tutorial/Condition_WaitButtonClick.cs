using UnityEngine;
using UnityEngine.UI;

// 특정 버튼 클릭을 기다렸다가 스텝을 완료시키는 조건
public class Condition_WaitButtonClick : TutorialConditionBase
{
    [Header("버튼")]
    [SerializeField] private Button _button; // 기다릴 버튼

    protected override void OnBegin() //시작
    {
        if (_button == null) return;
        _button.onClick.AddListener(OnClicked);
    }

    protected override void OnEnd() //종료
    {
        if (_button == null) return;
        _button.onClick.RemoveListener(OnClicked);
    }

    private void OnClicked()
    {
        Met(); // 조건 만족 처리
    }
}
