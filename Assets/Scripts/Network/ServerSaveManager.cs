// ServerSaveManager.cs
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSaveManager : Singleton<ServerSaveManager>
{
    public bool IsReady { get; private set; }

    private FirebaseFirestore _db;
    private bool _isUploading = false;

    public event Action<SaveSyncConflict> OnSyncConflict; // 충돌 발생 시 UI에 알림
    private SaveSyncConflict _pendingConflict; // 유저 선택 전까지 충돌 정보 보관

    private bool _isResolvingLinkConflict = false;  // 연동 직후 충돌 선택 진행중 표시(중복 방지)

    private long _lastUploadUnix = 0; // 마지막 서버 업로드 성공 시간(초)

    private int _uploadIntervalSec; // 백그라운드 업로드 최소 간격 캐싱

    protected override void Awake()
    {
        base.Awake();
        _db = FirebaseFirestore.DefaultInstance;
        IsReady = false;

        StartCoroutine(WaitForConfigRoutine());
    }
    private IEnumerator WaitForConfigRoutine()
    {
        while (Manager.Game == null || Manager.Game.Config == null)
        {
            yield return null;
        }

        _uploadIntervalSec = Manager.Game.Config.UploadIntervalSec;
    }

    // ===================== 공통 유틸 =====================

    private bool HasInternet() //오프라인 체크
    {
        return Application.internetReachability != NetworkReachability.NotReachable; //온라인 여부 반환
    }

    private string GetCurrentUidOrNull() //UID 얻기
    {
        return FirebaseAuth.DefaultInstance.CurrentUser?.UserId; // 현재 유저 UID 반환
    }

    private DocumentReference GetUserDoc(string uid) // 유저 서버 저장칸 반환
    {
        return _db.Collection("users").Document(uid); //문서 참조 반환
    }

    private static int GetIntFieldOrDefault(DocumentSnapshot snap, string field, int defaultValue) //필드 존재 체크 + 안전 읽기
    {
        if (snap != null && snap.ContainsField(field)) return snap.GetValue<int>(field); //존재하면 읽기
        return defaultValue;
    }

    private static Timestamp GetTimestampFieldOrDefault(DocumentSnapshot snap, string field, Timestamp defaultValue) // Timestamp 안전 읽기
    {
        if (snap != null && snap.ContainsField(field)) return snap.GetValue<Timestamp>(field); // [추가] 존재하면 읽기
        return defaultValue;
    }

    private bool IsConflictEventAvailable() // UI 구독자 존재 여부 체크를 함수로 분리
    {
        return OnSyncConflict != null; //구독자 있으면 true
    }

    private bool IsConflictPending() // 현재 충돌 대기 상태인지
    {
        return _pendingConflict != null; //대기 중이면 true
    }

    private void RaiseConflict(SaveSyncConflict conflict)
    {
        _pendingConflict = conflict; // 항상 pending에 저장(유저 선택까지 유지)

        if (!IsConflictEventAvailable())
        {
            Debug.LogError("세이브 충돌 발생했는데 SyncConflict UI가 없음. IsReady=false로 대기 상태 유지");
            return;
        }

        OnSyncConflict?.Invoke(_pendingConflict);
    }

    // ===================== 업로드 =====================
    public void UploadSave()
    {
        if (_isUploading) //업로드 중복 방지
        {
            Debug.LogWarning("세이브 서버에 업로드 중");
            return;
        }

        if (!HasInternet()) //오프라인 체크 함수로 통일
        {
            Debug.LogWarning("오프라인 상태. 서버 업로드 스킵");
            return;
        }

        var uid = GetCurrentUidOrNull();
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("Firebase UID 없음. 업로드 스킵");
            return;
        }

        var save = Manager.Save.CurrentData; //로컬 세이브
        var doc = GetUserDoc(uid);

        _isUploading = true; //모든 가드 통과 후에만 업로드 플래그 ON(조기 return 시 false 복구 필요 없음)

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "version", save.SnapshotVersion },  // 세이브 버전
            { "updatedAt", FieldValue.ServerTimestamp }, //서버 시간
            { "save", JsonUtility.ToJson(save) },   // 전체 세이브 JSON
            { "totalRaisedPets", save.UserData.TotalRaisedPets } //비교용 요약 필드
        };

        doc.SetAsync(data).ContinueWithOnMainThread(task => //비동기 업로드
        {
            _isUploading = false; //업로드 종료

            // TEST: 실패 원인 출력
            //if (task.IsFaulted) Debug.LogError(task.Exception);
            //if (task.IsCanceled) Debug.LogWarning("Canceled");

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("서버 업로드 실패");
                return;
            }

            _lastUploadUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //성공 업로드 시간 기록
            Debug.Log("<color=green>서버에 세이브 업로드 성공</color>");
        });
    }
    // ===================== 다운로드 + 비교 =====================
    public void DownloadIfNewer()
    {
        IsReady = false;   //서버 확인 끝날 때까지 로딩 대기
        _pendingConflict = null; //이전 충돌 정보 초기화

        if (!HasInternet()) //오프라인 체크 함수로 통일
        {
            Debug.LogWarning("오프라인 상태. 서버 로드 스킵");
            IsReady = true;
            return;
        }

        var uid = GetCurrentUidOrNull(); // UID 로직 통일
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("Firebase UID 없음. 서버 로드 스킵");
            IsReady = true;
            return;
        }

        var localSave = Manager.Save.CurrentData; //로컬 기준
        var doc = GetUserDoc(uid);

        doc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // TEST: 실패 원인 출력
                if (task.IsFaulted) Debug.LogError(task.Exception); 
                if (task.IsCanceled) Debug.LogWarning("Canceled");

                Debug.LogWarning("서버 세이브 요청 실패. 로컬 유지");
                IsReady = true;
                return;
            }

            DocumentSnapshot snap = task.Result;

            if (!snap.Exists) // 서버 문서 없음
            {
                Debug.LogWarning("서버 세이브 없음");
                IsReady = true;
                return;
            }

            long serverVersion = snap.GetValue<long>("version"); //서버 버전
            long localVersion = localSave.SnapshotVersion; // 로컬 버전

            int serverTotalPets = GetIntFieldOrDefault(snap, "totalRaisedPets", 0);
            int localTotalPets = localSave.UserData.TotalRaisedPets;

            //서버 version이 더 최신인데, 로컬 총펫 수가 더 많음 서버가 최신이라면서 실제 펫 수는 로컬이 더 앞선상황
            bool weirdA = (serverVersion > localVersion) && (localTotalPets > serverTotalPets);

            //로컬 version이 더 최신인데, 서버 총펫 수가 더 많음 로컬이 최신이라면서 실제 진행은 서버가 더 앞선 모순 상황
            bool weirdB = (localVersion > serverVersion) && (serverTotalPets > localTotalPets);

            if (weirdA || weirdB) //이상 케이스면 선택 UI로 넘김
            {
                string serverJson = snap.GetValue<string>("save"); // 적용용 JSON 확보
                Timestamp updatedAt = GetTimestampFieldOrDefault(snap, "updatedAt", default); // 안전 읽기 함수 사용

                var conflict = new SaveSyncConflict() //충돌 정보 생성
                {
                    Context = SaveSyncContext.Startup, //게임 실행 충돌

                    LocalVersion = localVersion,
                    LocalTotalPets = localTotalPets,
                    LocalLastPlayed = localSave.UserData.LastSavedUnixTime,

                    ServerVersion = serverVersion,
                    ServerTotalPets = serverTotalPets,
                    ServerUpdatedAt = updatedAt,
                    ServerSaveJson = serverJson,
                };

                RaiseConflict(conflict);
                return; //Resolve에서 IsReady 풀림
            }

            if (serverVersion > localVersion) // 서버가 최신이면 덮어쓰기
            {
                string json = snap.GetValue<string>("save");
                GameSaveSnapshot serverSave = JsonUtility.FromJson<GameSaveSnapshot>(json);

                Manager.Save.CurrentData = serverSave;
                Manager.Save.SaveGame();

                Debug.Log("서버 세이브로 덮어씀");
            }
            else
            {
                Debug.Log("로컬 세이브가 최신");
            }

            IsReady = true;
        });
    }

    // ===================== 충돌 선택 처리 =====================

    public void ResolveUseServerSave() // UI에서 서버 사용 선택 시
    {
        if (!IsConflictPending())
        {
            Debug.LogWarning("ResolveUseServerSave: 대기중 충돌 없음");
            IsReady = true;
            return;
        }

        GameSaveSnapshot serverSave = JsonUtility.FromJson<GameSaveSnapshot>(_pendingConflict.ServerSaveJson);
        Manager.Save.CurrentData = serverSave;
        Manager.Save.SaveGame();

        _pendingConflict = null; // 충돌 해제
        IsReady = true; 
    }

    public void ResolveUseLocalSave() // UI에서 로컬 사용(+업로드) 선택 시
    {
        _pendingConflict = null;
        IsReady = true;

        UploadSave(); // 로컬을 서버에 올림(온라인일 때만 업로드)
    }

    // ===================== 연동 직후: 서버 세이브 존재 시 선택 요구 =====================

    public void PromptConflictIfServerSaveExistsAfterLink()
    {
        if (_isResolvingLinkConflict) return; //중복 방지
        _isResolvingLinkConflict = true; //진행 시작

        if (!HasInternet()) // 오프라인 체크
        {
            _isResolvingLinkConflict = false;
            Debug.LogWarning("오프라인 상태. 연동 후 서버 세이브 확인 스킵");
            return;
        }

        var uid = GetCurrentUidOrNull();
        if (string.IsNullOrEmpty(uid)) 
        {
            _isResolvingLinkConflict = false;
            Debug.LogWarning("Firebase UID 없음. 연동 후 서버 세이브 확인 스킵");
            return;
        }

        var doc = GetUserDoc(uid); // 문서 참조 생성 로직 통일

        doc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            _isResolvingLinkConflict = false; // 조회 끝

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogWarning("연동 후 서버 세이브 조회 실패. 로컬 유지");
                return;
            }

            var snap = task.Result;

            if (!snap.Exists) //서버 세이브 없으면 로컬을 업로드
            {
                Debug.Log("연동된 계정에 서버 세이브 없음. 로컬 유지");
                UploadSave();
                return;
            }

            // 서버 세이브 존재 > 선택 UI 띄우기
            var localSave = Manager.Save.CurrentData;

            long localVersion = localSave.SnapshotVersion;
            int localTotalPets = localSave.UserData.TotalRaisedPets;
            long localLastPlayed = localSave.UserData.LastSavedUnixTime;

            long serverVersion = snap.GetValue<long>("version");
            int serverTotalPets = GetIntFieldOrDefault(snap, "totalRaisedPets", 0);
            Timestamp updatedAt = GetTimestampFieldOrDefault(snap, "updatedAt", default);
            string serverJson = snap.GetValue<string>("save");

            var conflict = new SaveSyncConflict()
            {
                Context = SaveSyncContext.Link, //연동 충돌

                LocalVersion = localVersion,
                LocalTotalPets = localTotalPets,
                LocalLastPlayed = localLastPlayed,

                ServerVersion = serverVersion,
                ServerTotalPets = serverTotalPets,
                ServerUpdatedAt = updatedAt,
                ServerSaveJson = serverJson,
            };

            // 이미 pending 충돌이 있는데 또 올리면 UI가 연속으로 뜰 수 있어서 가드
            if (IsConflictPending()) return; //이미 대기 중이면 추가 오픈 방지

            RaiseConflict(conflict);
        });
    }

    // ===================== 충돌 대기 해제(로그아웃/재연동용) =====================

    public void CancelPendingConflict()
    {
        _pendingConflict = null; // 충돌 제거
        IsReady = true;    // 로딩에서 막고있으면 풀기
    }

    public void ClearPendingConflict()
    {
        _pendingConflict = null; // 충돌 정보 제거(단순 제거)
    }
    //================== 백그라운드 진입 시 호출용===================
    public void TryUploadOnBackground()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //현재 시간

        if (_isUploading) return; //업로드 중이면 스킵

        //간격 안 됐으면 스킵
        if (_lastUploadUnix > 0 && (now - _lastUploadUnix) < _uploadIntervalSec) return;

        UploadSave(); // 간격 됐으면 업로드
    }

}

// ===================== 충돌 데이터 컨테이너) =====================

public class SaveSyncConflict // 충돌 UI에 넘길 데이터 컨테이너
{
    public SaveSyncContext Context; // 어떤 상황에서 떴는지

    public long LocalVersion;       // 로컬 버전
    public int LocalTotalPets;      // 로컬 총 키운 펫 수
    public long LocalLastPlayed;    // 로컬 마지막 플레이 시간(표시용)

    public long ServerVersion;      // 서버 버전
    public int ServerTotalPets;     // 서버 총 키운 펫 수
    public Timestamp ServerUpdatedAt; // 서버 updatedAt(표시용)
    public string ServerSaveJson;   // 서버 세이브 JSON(선택 후 적용용)
}
