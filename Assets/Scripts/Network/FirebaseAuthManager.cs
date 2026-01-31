using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using Google;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    [SerializeField] private string _webClientId; // Web Client ID 입력용

    private GoogleSignInConfiguration _googleConfig; // 구글 로그인 설정
    private bool _isLinking; //연동 중복 방지

    private FirebaseAuth _auth;
    public bool IsReady { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase(); // Firebase 초기화 시작
    }
    //==================파이어베이스 초기화=============================
    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => // 의존성 체크(비동기)
        {
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError("Firebase 의존성 문제");
                return;
            }

            _auth = FirebaseAuth.DefaultInstance;

            _googleConfig = new GoogleSignInConfiguration // GoogleSignIn 설정 생성
            {
                WebClientId = _webClientId,  // Web Client ID 적용
                RequestIdToken = true,       // Firebase credential용 idToken 필요
                RequestEmail = true          // 선택(디버그/표시용)
            };

            SignInAnonymously(); //익명 로그인
        });
    }
    //==================익명 로그인=============================
    private void SignInAnonymously()
    {
        if (_auth == null)
        {
            Debug.LogError("FirebaseAuth 없음");
            IsReady = false;
            return;
        }

        if (_auth.CurrentUser != null)
        {
            Debug.Log($"이미 로그인 됨 UID : {_auth.CurrentUser.UserId}");
            IsReady = true;
            return;
        }

        _auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task => // 익명 로그인(비동기)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("익명로그인 실패");
                IsReady = true;
                return;
            }

            FirebaseUser user = task.Result.User; // 결과 유저

            Debug.Log($"익명 로그인 성공 UID: {user.UserId}");

            IsReady = true;
        });
    }
    //==================버튼으로 로그인=============================
    public void LinkGoogleButton()
    {
        LinkWithGoogle(success =>
        {
            Debug.Log($"Google Link Result: {success}"); // 결과 로그
            if (success) Manager.Server.DownloadIfNewer(); // 연동 성공 후 서버 세이브 로드
        });
    }
    //==================구글 로그인=============================
    public void LinkWithGoogle(System.Action<bool> callback) // 구글 연동/로그인 메인 함수
    {
        if (!IsReady) { callback?.Invoke(false); return; } // 준비 안 됐으면 실패
        if (_isLinking) { callback?.Invoke(false); return; } // 이미 진행 중이면 실패
        if (Application.internetReachability == NetworkReachability.NotReachable) { callback?.Invoke(false); return; } // 인터넷 없으면 실패
        if (string.IsNullOrEmpty(_webClientId)) { Debug.LogError("WebClientId 비어있음"); callback?.Invoke(false); return; } // ID 없으면 실패
        if (_auth == null || _auth.CurrentUser == null) { Debug.LogError("Firebase 유저 없음(익명로그인 먼저 필요)"); callback?.Invoke(false); return; } // 유저 없으면 실패

        _isLinking = true; // 진행 시작 플래그

        GoogleSignIn.Configuration = _googleConfig; // 구글 로그인 설정 적용

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task => // 구글 로그인 UI(비동기)
        {
            if (task.IsCanceled || task.IsFaulted)  // 구글 로그인 실패면
            {
                _isLinking = false;
                Debug.LogError("GoogleSignIn 실패");
                callback?.Invoke(false);
                return;
            }

            string idToken = task.Result.IdToken; // 구글 IdToken
            if (string.IsNullOrEmpty(idToken)) // 토큰이 없으면
            {
                _isLinking = false;
                Debug.LogError("idToken 없음");
                callback?.Invoke(false);
                return;
            }

            //구글 로그인 성공

            Credential cred = GoogleAuthProvider.GetCredential(idToken, null); // Firebase용 Credential 생성
            FirebaseUser cur = _auth.CurrentUser; // 현재 Firebase 유저

            if (cur.IsAnonymous) // 현재가 익명 유저면
            {
                LinkAnonymousToGoogle(cur, cred, callback); // 익명 > 구글 계정으로 연동
                return; 
            }

            SignInWithGoogleCredential(cred, callback); // 이미 익명이 아니면 그냥 구글 Credential로 로그인
        });
    }
    // ================== 연동 ==================
    private void LinkAnonymousToGoogle(FirebaseUser cur, Credential cred, System.Action<bool> callback) // 익명 계정에 구글 연결
    {
        cur.LinkWithCredentialAsync(cred).ContinueWithOnMainThread(t => // 계정 연결(비동기)
        {
            if (t.IsCanceled || t.IsFaulted) // 연결 실패면
            {
                var fex = t.Exception?.GetBaseException() as FirebaseException; // Firebase 예외 추출
                if (fex != null && (AuthError)fex.ErrorCode == AuthError.CredentialAlreadyInUse) // 이미 다른 계정이 이 구글을 쓰는 경우면
                {
                    SignInWithGoogleCredential(cred, callback); // 연결 대신 그 구글 계정으로 로그인 전환
                    return; // 종료
                }

                _isLinking = false; // 진행 플래그 해제
                Debug.LogError("Google Link 실패"); // 로그
                callback?.Invoke(false); // 실패 콜백
                return; // 종료
            }

            _isLinking = false; // 진행 플래그 해제
            Debug.Log($"Google Link 성공 UID: {_auth.CurrentUser.UserId}"); // 성공 로그
            callback?.Invoke(true); // 성공 콜백
        });
    }
    //==========================구글 Credential로 로그인==========================
    private void SignInWithGoogleCredential(Credential cred, System.Action<bool> callback)
    {
        _auth.SignInWithCredentialAsync(cred).ContinueWithOnMainThread(t => // 로그인(비동기)
        {
            _isLinking = false; // 진행 플래그 해제(여기서 완전히 종료 처리)

            if (t.IsCanceled || t.IsFaulted) // 로그인 실패면
            {
                Debug.LogError("Google SignIn 실패"); // 로그
                callback?.Invoke(false); // 실패 콜백
                return; // 종료
            }

            Debug.Log($"Google SignIn 성공 UID: {_auth.CurrentUser.UserId}"); // 성공 로그
            callback?.Invoke(true); // 성공 콜백
        });
    }
}
