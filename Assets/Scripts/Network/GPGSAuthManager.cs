using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

public class GPGSAuthManager : MonoBehaviour
{
    private void Awake()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    public void Login(Action<bool> callback) // 로그인 요청 + 결과 콜백
    {
        // 오프라인이면 즉시 실패
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback?.Invoke(false); // 실패 반환
            return;
        }

        // 이미 로그인 돼 있으면 성공 처리
        if (Social.localUser.authenticated)
        {
            callback?.Invoke(true);
            return;
        }

        // 로그인 시도
        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            Debug.Log($"GPGS SignInStatus: {status}");     // 원인 status 출력
            callback?.Invoke(status == SignInStatus.Success); //성공 판정
        });
    }
}
