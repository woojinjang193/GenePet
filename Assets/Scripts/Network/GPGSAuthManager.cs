using UnityEngine;
using GooglePlayGames;
using System.Collections;

public class GPGSAuthManager : MonoBehaviour
{
    private int _maxRetry = 3;        //최대 재시도 횟수
    private float _retryDelay = 2f;   //재시도 간격

    private void Awake()
    {
        PlayGamesPlatform.Activate();
    }
    public void Login()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("인터넷 연결 없음.");
            return;
        }

        if (Social.localUser.authenticated)
        {
            Debug.Log("이미 GPGS 로그인됨");
            return;
        }

        StartCoroutine(LoginRetryRoutine());
    }
    private IEnumerator LoginRetryRoutine()
    {
        int tryCount = 0; //시도 횟수

        while (tryCount < _maxRetry)
        {
            tryCount++; //시도 횟수 증가
            Debug.Log($"GPGS 로그인 시도 {tryCount}회"); //로그 출력

            bool isDone = false;   //콜백 완료 여부
            bool isSuccess = false; //성공 여부

            Social.localUser.Authenticate(success =>
            {
                isSuccess = success; //결과 저장
                isDone = true;       //콜백 완료
            });

            //로그인 콜백 올 때까지 대기
            yield return new WaitUntil(() => isDone);

            if (isSuccess)
            {
                Debug.Log("GPGS 로그인 성공"); //성공
                Debug.Log($"PlayerID: {Social.localUser.id}"); //플레이어 ID
                yield break; //코루틴 종료
            }

            Debug.LogWarning("GPGS 로그인 실패"); //실패

            if (tryCount < _maxRetry)
            {
                yield return new WaitForSeconds(_retryDelay); //2초 대기 후 재시도
            }
        }

        Debug.LogError("GPGS 로그인 최종 실패"); //3회 모두 실패
    }
}
