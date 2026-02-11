using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine;

public class UpdateChecker : MonoBehaviour
{
    AppUpdateManager _updateManager;
    public bool IsReady { get; private set; } = false;
    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(CheckForUpdate());
#else
        IsReady = true; //안드로이드 아니면 바로 준비 완료
#endif
    }
    private IEnumerator CheckForUpdate()
    {
        _updateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            _updateManager.GetAppUpdateInfo(); //업데이트 정보 요청

        yield return appUpdateInfoOperation;

        if (!appUpdateInfoOperation.IsSuccessful) //결과 못받으면
        {
            Debug.LogWarning($"[UpdateChecker] 업데이트 정보 못받음: {appUpdateInfoOperation.Error}");
            IsReady = true;
            yield break;
        }

        var appUpdateInfoResult = appUpdateInfoOperation.GetResult(); //결과 얻기

        // 업데이트 불가능이면 리턴
        if (appUpdateInfoResult.UpdateAvailability != UpdateAvailability.UpdateAvailable)
        {
            IsReady = true;
            yield break;
        }

        // Immediate 옵션 생성
        var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(); // 즉시 업데이트 옵션

        // 즉시 업데이트 요청
        var startUpdateRequest = _updateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions); // 업데이트 플로우 시작

        yield return startUpdateRequest; // 즉시 업데이트는 yield return으로 대기 

        if (startUpdateRequest.Status == AppUpdateStatus.Failed)
        {
            Debug.LogWarning($"[UpdateChecker] 업데이트 요청 실패: {startUpdateRequest.Error}"); // 실패 처리
            IsReady = true;
            yield break;
        }

        IsReady = true;
    }
}
