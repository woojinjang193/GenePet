using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLinkButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private GPGSAuthManager _gpgs;
    private Button _button;

    private bool _isProcessing = false;
    private FirebaseAuth _auth;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);

        if(_gpgs == null)
        {
            _gpgs = FindObjectOfType<GPGSAuthManager>();
        }

        _auth = FirebaseAuth.DefaultInstance;
    }
    private void Start()
    {
        var user = _auth.CurrentUser;
        if (user != null && !user.IsAnonymous)
        {
            Success();
        }
    }
    private void OnClick()
    {
        if (_isProcessing) return;

        _isProcessing = true; // 처리 시작
        _buttonText.text = "Login..."; // 상태 표시

        _gpgs.Login(OnGPGSLoginResult);
    }
    private void OnGPGSLoginResult(bool success) // 로그인 결과 콜백
    {
        if (!success)
        {
            Fail("Login Failed");
            return;
        }

        // 로그인 성공시
        _buttonText.text = "Link Account..."; 
        LinkFirebase(); // Firebase 연동 시작
    }

    private void LinkFirebase()
    {
        var user = _auth.CurrentUser;

        if (user == null)
        {
            Fail("No Firebase User"); // Firebase 없음
            return;
        }

        // GPGS 서버 인증 코드 요청
        PlayGamesPlatform.Instance.RequestServerSideAccess(
            false, // refresh 토큰 불필요
            authCode =>
            {
                Credential credential =
                    GoogleAuthProvider.GetCredential(authCode, null); // 자격증명 생성

                // Firebase 계정 연동
                user.LinkWithCredentialAsync(credential)
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Fail("Link Failed"); // 연동 실패
                            return;
                        }

                        // 연동 성공
                        Manager.Save.CurrentData.UserData.FirebaseUID =
                            _auth.CurrentUser.UserId; // Firebase UID 저장

                        Manager.Save.SaveGame(); // 세이브
                        Manager.Server.UploadSave(); //서버에 업로드

                        var linkedUser = _auth.CurrentUser;
                        if (linkedUser != null && !linkedUser.IsAnonymous)
                        {
                            Success();
                        }
                    });
            });
    }

    private void Fail(string msg) // 실패 처리
    {
        _buttonText.text = msg; // 메시지 표시
        _isProcessing = false; // 다시 가능
    }

    private void Success() // 성공 처리
    {
        _buttonText.text = "Linked!";
        _isProcessing = false;
        _button.interactable = false;

    }
}
