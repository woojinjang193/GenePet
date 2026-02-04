using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GPGSManager : MonoBehaviour
{
    private bool _isSigningIn;

    private void Awake()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        PlayGamesPlatform.DebugLogEnabled = true; //개발 빌드에서만 로그
#endif
        PlayGamesPlatform.Activate();             // GPGS 활성화
    }

    private void Start()
    {
        AutoLogin(); //시작 시 자동 로그인 시도
    }

    public void AutoLogin() // 자동 로그인(실패해도 게임 진행)
    {
        if (_isSigningIn) return;
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        if (Social.localUser != null && Social.localUser.authenticated) return;

        _isSigningIn = true;

        PlayGamesPlatform.Instance.Authenticate((SignInStatus status) =>
        {
            _isSigningIn = false;
            Debug.Log($"GPGS Auto SignInStatus: {status}");
        });
    }

    public void LoginButton() // UI 버튼에서 호출(팝업 로그인)
    {
        if (_isSigningIn) return;
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        if (Social.localUser != null && Social.localUser.authenticated) return;

        _isSigningIn = true;

        PlayGamesPlatform.Instance.ManuallyAuthenticate((SignInStatus status) => //팝업 로그인
        {
            _isSigningIn = false;
            Debug.Log($"GPGS Manual SignInStatus: {status}");
        });
    }
}
