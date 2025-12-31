using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    private FirebaseAuth _auth;
    public bool IsReady { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase(); // Firebase 초기화 시작
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError("Firebase 의존성 문제");
                return;
            }

            _auth = FirebaseAuth.DefaultInstance;
            SignInAnonymously();
        });
    }
    private void SignInAnonymously()
    {
        if (_auth.CurrentUser != null)
        {
            Debug.Log($"이미 로그인 됨 UID : {_auth.CurrentUser.UserId}");
            IsReady = true;
            return;
        }

        _auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("익명로그인 실패");
                IsReady = true;
                return;
            }

            AuthResult result = task.Result;
            FirebaseUser user = result.User;

            Debug.Log($"익명 로그인 성공 UID: {user.UserId}");
            Manager.Save.CurrentData.UserData.FirebaseUID = user.UserId;
            Manager.Save.SaveGame();

            IsReady = true;
        });
    }
}
