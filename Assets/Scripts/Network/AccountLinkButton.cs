using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLinkButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _buttonText;
    private Button _button;

    private bool _isProcessing = false;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }
    private void OnClick() // 버튼 클릭
    {
        if (_isProcessing) return; // 이미 처리 중이면 무시

        _isProcessing = true; // 처리 시작
        _buttonText.text = "Login..."; // 상태 표시

        TryLoginAndLink(); // 전체 흐름 시작
    }
    private void TryLoginAndLink()
    {
        // 오프라인 체크
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Fail("No Internet");
            return;
        }

        // GPGS 로그인 안 돼 있으면 로그인부터
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(success =>
            {
                if (!success)
                {
                    Fail("Login Failed");
                    return;
                }

                // 로그인 성공 → 다음 단계
                _buttonText.text = "Link Account...";
                LinkFirebase();
            });

            return;
        }

        // 이미 로그인 돼 있으면 바로 연동
        _buttonText.text = "Link Account...";
        LinkFirebase();
    }

    private void LinkFirebase()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance; // Firebase Auth
        FirebaseUser user = auth.CurrentUser; // 현재 유저

        if (user == null)
        {
            Fail("No Firebase User");
            return;
        }

        // GPGS 서버 인증 코드 요청
        PlayGamesPlatform.Instance.RequestServerSideAccess(
            false, // refresh 토큰 불필요
            authCode =>
            {
                Credential credential =
                    GoogleAuthProvider.GetCredential(authCode, null); // 자격증명 생성

                // Firebase 계정 연결
                user.LinkWithCredentialAsync(credential)
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Fail("Link Failed");
                            return;
                        }

                        // 연동 성공
                        Manager.Save.CurrentData.UserData.FirebaseUID =
                            auth.CurrentUser.UserId; // Firebase UID 저장

                        Manager.Save.SaveGame(); // 세이브

                        Success("Linked!");
                    });
            });
    }

    private void Fail(string msg) // 실패 처리
    {
        _buttonText.text = msg; // 실패 메시지
        _isProcessing = false; // 다시 누를 수 있게
    }

    private void Success(string msg) // 성공 처리
    {
        _buttonText.text = msg; // 성공 메시지
        _isProcessing = false; // 완료
    }
}
