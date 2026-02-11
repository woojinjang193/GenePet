using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SaveSyncContext // 충돌 발생 상황 구분
{
    Startup, // 게임 실행 시 동기화
    Link   // 구글 연동 시
}

public class SaveSyncConflictUI : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject _panel;

    [Header("텍스트")]
    [SerializeField] private TMP_Text _localText;  // 로컬 요약 표시 텍스트
    [SerializeField] private TMP_Text _serverText; // 서버 요약 표시 텍스트

    [Header("버튼")]
    [SerializeField] private Button _useServerButton;  // 서버 사용 버튼
    [SerializeField] private Button _useLocalAndUploadButton;   // 로컬 사용 + 즉시 업로드 버튼
    [SerializeField] private Button _logoutAndRelinkButton;  // 연동 충돌에서만 쓰는 로그아웃 버튼(연동했는데 세이브파일이 존제 할 경우)

    private Coroutine _waitRoutine;
    private ServerSaveManager Server => Manager.Server;
    private FirebaseAuthManager Fire => Manager.Fire;

    private void Awake()
    {
        SetPanelVisible(false);

        if (_useServerButton != null) _useServerButton.onClick.AddListener(OnClickUseServer);
        if (_useLocalAndUploadButton != null) _useLocalAndUploadButton.onClick.AddListener(OnClickUseLocalAndUpload);
        if (_logoutAndRelinkButton != null) _logoutAndRelinkButton.onClick.AddListener(OnClickLogoutAndRelink);
    }

    private void OnEnable()
    {
        if (Server == null)
        {
            _waitRoutine = StartCoroutine(WaitSaveManagerRoutine());
        }
        else
        {
            Server.OnSyncConflict += Open;
        }
    }
    private IEnumerator WaitSaveManagerRoutine()
    {
        while (Server == null)
        {
            yield return null;
        }

        Server.OnSyncConflict += Open;
        _waitRoutine = null;
    }
    private void OnDisable()
    {
        if (_waitRoutine != null) // 대기 코루틴이 돌고 있으면 중지
        {
            StopCoroutine(_waitRoutine);
            _waitRoutine = null;
        }

        if (Server != null) Server.OnSyncConflict -= Open;
    }

    private void Open(SaveSyncConflict conflict)
    {
        if (conflict == null) return;

        SetPanelVisible(true);

        ConfigureButtons(conflict.Context); //컨텍스트별 버튼 노출

        SetSummaryTexts(conflict); //텍스트 구성
    }

    private void ConfigureButtons(SaveSyncContext context)
    {
        bool isStartup = (context == SaveSyncContext.Startup); // 실행 충돌인지 여부

        // 실행일 때: 로컬+업로드 버튼 보이기
        if (_useLocalAndUploadButton != null)
            _useLocalAndUploadButton.gameObject.SetActive(isStartup);

        // 연동일 때: 로그아웃 버튼 보이기
        if (_logoutAndRelinkButton != null)
            _logoutAndRelinkButton.gameObject.SetActive(!isStartup);
    }
    private void SetSummaryTexts(SaveSyncConflict c)
    {
        if (_localText != null)
        {
            _localText.text =
                $"[LOCAL]\n" +
                $"TotalPets: {c.LocalTotalPets}\n" +
                $"LastPlayed: {FormatUnixSeconds(c.LocalLastPlayed)}"; // 유닉스초 > 날짜
        }

        if (_serverText != null)
        {
            _serverText.text =
                $"[SERVER]\n" +
                $"TotalPets: {c.ServerTotalPets}\n" +
                $"UpdatedAt: {FormatTimestamp(c.ServerUpdatedAt)}"; // Timestamp > 날짜
        }
    }
    private void SetPanelVisible(bool visible)
    {
        if (_panel != null) _panel.SetActive(visible);
    }

    private void Close()
    {
        SetPanelVisible(false);
    }

    private void OnClickUseServer()
    {
        if (Server != null) Server.ResolveUseServerSave(); // 서버 세이브 적용
        Close();
    }

    private void OnClickUseLocalAndUpload()
    {
        if (Server != null) Server.ResolveUseLocalSave(); // 로컬 유지 + 업로드
        Close();
    }
    private void OnClickLogoutAndRelink()
    {
        if (Fire != null) Fire.SignOutToAnonymous(); // 로그아웃 > 익명 복귀
        if (Server != null) Server.ClearPendingConflict(); //충돌 정보 제거

        Close();
    }

    // ==============유닉스 타임 > 날짜 문자열=============
    private string FormatUnixSeconds(long unixSeconds)
    {
        try
        {
            return DateTimeOffset
                .FromUnixTimeSeconds(unixSeconds) //유닉스 "초" 변환
                .ToLocalTime()                    // 로컬 시간으로 변환
                .ToString("yyyy-MM-dd HH:mm:ss"); // 보기 좋은 포맷
        }
        catch
        {
            return unixSeconds.ToString(); // 실패 시 원값
        }
    }
    // Firestore Timestamp > 로컬 시간 문자열
    private string FormatTimestamp(Firebase.Firestore.Timestamp ts)
    {
        try
        {
            return ts
                .ToDateTime()   //Timestamp > DateTime(UTC 기준)
                .ToLocalTime()  // 로컬 시간으로 변환
                .ToString("yyyy-MM-dd HH:mm:ss");  //보기 좋은 포맷
        }
        catch
        {
            return ts.ToString(); //실패 시 기본 출력
        }
    }
}
