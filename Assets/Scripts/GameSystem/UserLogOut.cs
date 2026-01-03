using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserLogOut : MonoBehaviour
{
    private Button _button;
    private bool _isProcessing = false;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (_isProcessing) return;

        _isProcessing = true;
        _button.interactable = false;

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        // 익명 계정이면 Firebase 유저 삭제
        if (user != null && user.IsAnonymous)
        {
            DeleteServerSave(user.UserId, () =>
            {
                user.DeleteAsync().ContinueWithOnMainThread(_ =>
                {
                    Debug.Log("<color=red>FireBase 유저 데이터 삭제</color>");
                    FinishReset();
                });
            });
        }
        else
        {
            // 연동 계정이면 로그아웃만
            auth.SignOut();
            Debug.Log("<color=green>로그아웃 완료</color>");
            FinishReset();
        }
    }

    private void DeleteServerSave(string firebaseUID, System.Action onDone)
    {
        if (string.IsNullOrEmpty(firebaseUID))
        {
            onDone?.Invoke();
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        db.Collection("users").Document(firebaseUID).DeleteAsync()
            .ContinueWithOnMainThread(_ =>
            {
                onDone?.Invoke();
            });
    }

    private void FinishReset()
    {
        // 로컬 세이브 삭제
        SaveSystem.DeleteSnapshot();

        //모든 매니저 제거
        ReleaseAllManagers();

        // 첫 씬으로 이동
        SceneManager.LoadScene("LoginScene");
    }
    private void ReleaseAllManagers()
    {
        ShopManager.ReleaseManager();
        GeneManager.ReleaseManager();
        SaveManager.ReleaseManager();
        GameManager.ReleaseManager();
        LanguageManager.ReleaseManager();
        AudioManager.ReleaseManager();
        ItemManager.ReleaseManager();
        FirebaseAuthManager.ReleaseManager();
        ServerSaveManager.ReleaseManager();
    }
}
