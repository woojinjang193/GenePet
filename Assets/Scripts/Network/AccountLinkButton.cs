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

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);

        if(_gpgs == null)
        {
            _gpgs = FindObjectOfType<GPGSAuthManager>();
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
        FirebaseAuth auth = FirebaseAuth.DefaultInstance; // Firebase Auth
        FirebaseUser user = auth.CurrentUser; // 현재 유저

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
                            auth.CurrentUser.UserId; // Firebase UID 저장
                        Manager.Save.SaveGame(); // 세이브

                        Success("Linked!"); // 완료 표시
                    });
            });
    }

    private void Fail(string msg) // 실패 처리
    {
        _buttonText.text = msg; // 메시지 표시
        _isProcessing = false; // 다시 가능
    }

    private void Success(string msg) // 성공 처리
    {
        _buttonText.text = msg; // 메시지 표시
        _isProcessing = false; // 완료
    }
}
