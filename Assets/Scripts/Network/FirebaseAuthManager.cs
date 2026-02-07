using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using Google;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    [SerializeField] private string _webClientId; // Google Web Client ID

    private GoogleSignInConfiguration _googleConfig; // 구글 로그인 설정
    private bool _isLinking = false;// 연동/로그인 중복 방지 플래그

    private FirebaseAuth _auth;
    public FirebaseAuth Auth => _auth;
    public bool IsReady { get; private set; }  // 익명 로그인까지 완료되면 true

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase(); // Firebase 초기화 시작
    }

    // ===================== 초기화 =====================

    private void InitializeFirebase()
    {
        IsReady = false; // 초기화 시작 시 준비 상태 false로 명확히

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != DependencyStatus.Available) // 의존성 문제면 중단
            {
                Debug.LogError("Firebase 의존성 문제");
                return;
            }

            _auth = FirebaseAuth.DefaultInstance; //Auth 인스턴스 확보

            //  GoogleSignIn 설정 생성(이 설정으로 구글 로그인 UI를 띄움)
            _googleConfig = new GoogleSignInConfiguration
            {
                WebClientId = _webClientId,
                RequestIdToken = true,
                RequestEmail = true
            };

            SignInAnonymously(); //기본은 익명 로그인부터
        });
    }

    // ===================== 익명 로그인 =====================

    private void SignInAnonymously()
    {
        if (_auth == null) //Auth가 없으면 진행 불가
        {
            Debug.LogError("FirebaseAuth 없음");
            IsReady = false;
            return;
        }

        // 이미 유저가 있으면(익명이든 구글이든) 추가 로그인 없이 준비 완료
        if (_auth.CurrentUser != null)
        {
            Debug.Log($"이미 로그인 됨 UID : {_auth.CurrentUser.UserId}");
            IsReady = true;
            return;
        }

        //익명 로그인(비동기)
        _auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("익명로그인 실패");
                IsReady = true;
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log($"익명 로그인 성공 UID: {user.UserId}");

            IsReady = true;
        });
    }

    // ===================== 버튼 진입점 =====================

    public void LinkGoogleButton()
    {
        // 연동 버튼 하나만 눌렀을 때 중복 실행/오프라인 등을 빠르게 막음
        if (!CanStartLinking()) return; // 시작 불가면 종료

        LinkWithGoogle(success => //구글 연동 시도
        {
            Debug.Log($"Google Link Result: {success}");
            if (!success) return;

            // 연동 성공 직후: 서버 세이브가 있으면 선택 UI 띄우기
            if (Manager.Server != null) Manager.Server.PromptConflictIfServerSaveExistsAfterLink();
        });
    }

    private bool CanStartLinking() // 연동 시작 가능 조건을 한 곳으로 모음
    {
        if (!IsReady) return false; // Firebase 초기화/익명로그인 완료 전이면 불가
        if (_isLinking) return false; // 이미 진행 중이면 불가
        if (Application.internetReachability == NetworkReachability.NotReachable) return false; // 오프라인이면 불가
        if (string.IsNullOrEmpty(_webClientId))
        {
            Debug.LogError("WebClientId 비어있음");
            return false;
        }
        if (_auth == null || _auth.CurrentUser == null)
        {
            Debug.LogError("Firebase 유저 없음(익명로그인 먼저 필요)");
            return false;
        }
        return true;
    }

    // ===================== 구글 연동/로그인 메인 =====================

    private void LinkWithGoogle(System.Action<bool> callback)
    {
        if (!CanStartLinking()) { callback?.Invoke(false); return; } // 가드 로직 통일

        _isLinking = true; // 진행 시작 플래그

        GoogleSignIn.Configuration = _googleConfig; // 구글 로그인 설정 적용

        // 구글 로그인 UI(비동기)
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                FinishLinking(false, callback, "GoogleSignIn 실패"); // 종료 처리 통일
                return;
            }

            string idToken = task.Result.IdToken; // 구글 IdToken
            if (string.IsNullOrEmpty(idToken))
            {
                FinishLinking(false, callback, "idToken 없음"); // 종료 처리 통일
                return;
            }

            // Firebase용 Credential 생성
            Credential cred = GoogleAuthProvider.GetCredential(idToken, null);
            FirebaseUser cur = _auth.CurrentUser;

            if (cur.IsAnonymous)
            {
                // 익명 > 구글 연결
                LinkAnonymousToGoogle(cur, cred, callback);
                return;
            }

            // 이미 익명이 아니면 구글 Credential로 로그인
            SignInWithGoogleCredential(cred, callback);
        });
    }

    private void FinishLinking(bool success, System.Action<bool> callback, string log)
    {
        _isLinking = false; // 진행 플래그 해제
        if (!string.IsNullOrEmpty(log))
        {
            if (success) Debug.Log(log);   //성공 로그
            else Debug.LogError(log); //실패 로그
        }
        callback?.Invoke(success); 
    }

    // ================== 익명 계정에 구글 연결 ==================

    private void LinkAnonymousToGoogle(FirebaseUser cur, Credential cred, System.Action<bool> callback)
    {
        cur.LinkWithCredentialAsync(cred).ContinueWithOnMainThread(t =>
        {
            if (t.IsCanceled || t.IsFaulted)
            {
                // 이미 다른 계정이 이 구글을 쓰고 있으면: 연결 대신 그 계정으로 로그인 전환
                var fex = t.Exception?.GetBaseException() as FirebaseException;
                if (fex != null && (AuthError)fex.ErrorCode == AuthError.CredentialAlreadyInUse)
                {
                    SignInWithGoogleCredential(cred, callback);
                    return;
                }

                FinishLinking(false, callback, "Google Link 실패");
                return;
            }

            FinishLinking(true, callback, $"Google Link 성공 UID: {_auth.CurrentUser.UserId}"); //종료 처리 통일
        });
    }

    // ================== 구글 Credential로 로그인 ==================

    private void SignInWithGoogleCredential(Credential cred, System.Action<bool> callback)
    {
        _auth.SignInWithCredentialAsync(cred).ContinueWithOnMainThread(t =>
        {
            if (t.IsCanceled || t.IsFaulted)
            {
                FinishLinking(false, callback, "Google SignIn 실패"); // 종료 처리 통일
                return;
            }

            FinishLinking(true, callback, $"Google SignIn 성공 UID: {_auth.CurrentUser.UserId}"); // 종료 처리 통일
        });
    }

    // ================== 로그아웃 > 익명 복귀 ==================

    public void SignOutToAnonymous()
    {
        // 구글 세션 로그아웃(다음 로그인 때 계정 선택 화면 나오게 하려는 목적)
        GoogleSignIn.DefaultInstance.SignOut();

        // 파이어베이스 로그아웃(현재 UID를 비움)
        if (_auth != null) _auth.SignOut();

        IsReady = false; // 준비 상태 리셋(다시 익명 로그인 필요)

        // linking 중이었다면 강제로 플래그 해제(꼬임 방지)
        _isLinking = false; // 중복/꼬임 방지

        SignInAnonymously(); //다시 익명 로그인
    }

}
