using UnityEngine;

public abstract class AdRequestBase : MonoBehaviour, IAdRequester
{
    private bool _isRequesting; // 중복 요청 방지
    private bool _rewardGiven;  // 보상 중복 지급 방지

    protected bool CanRequest => !_isRequesting; //외부에서 요청 가능 여부 확인용

    protected void Request() //자식이 호출하는 공통 요청 함수
    {
        if (_isRequesting) return; //연타/중복 요청 차단

        _isRequesting = true;  // 요청 상태 ON
        _rewardGiven = false;  // 보상 플래그 초기화

        Manager.AD.ShowRewardedAd(this); // 자기 자신을 requester로 전달
    }

    public void AdWatched()
    {
        if (!_isRequesting) return;
        if (_rewardGiven) return;

        _rewardGiven = true;
        OnReward();  // 자식마다 다른 보상 로직 실행
    }

    public void AdClosed()
    {
        if (!_isRequesting) return; //유효하지 않은 콜백 방어

        _isRequesting = false; // 요청 상태 OFF
        OnClosed();   //자식에서 닫힘 후 추가 행동 필요시 훅
    }

    // ===== 보상을 override 해서 구현 =====
    protected abstract void OnReward();

    // ===== 광고 닫으면 행동 =====
    protected abstract void OnClosed();
}
