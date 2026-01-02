using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSaveManager : Singleton<ServerSaveManager>
{
    public bool IsReady { get; private set; }
    private bool _isUploading = false;
    private FirebaseFirestore _db;

    protected override void Awake()
    {
        base.Awake();
        _db = FirebaseFirestore.DefaultInstance;
        IsReady = false;
    }

    // ===== 업로드 =====
    public void UploadSave()
    {
        if (_isUploading)
        {
            Debug.LogWarning("세이브 서버에 업로드 중");
            return;
        }
        
        _isUploading = true;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("오프라인 상태. 서버 업로드 스킵");
            _isUploading = false;
            return;
        }

        var save = Manager.Save.CurrentData;
        var uid = save.UserData.FirebaseUID;

        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("Firebase UID 없음. 업로드 스킵");
            _isUploading = false;
            return;
        }

        var doc = _db.Collection("users").Document(uid); // 유저 문서 참조

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "version", save.SnapshotVersion },          // 세이브 버전
            { "updatedAt", FieldValue.ServerTimestamp },  // 서버 시간
            { "save", JsonUtility.ToJson(save) }          // 전체 세이브 JSON
        };

        doc.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            _isUploading = false;

            if (task.IsFaulted || task.IsCanceled) //실패 체크
            {
                Debug.LogError("서버 업로드 실패");
                return;
            }
            Debug.Log("<color=green>서버에 세이브 업로드 성공</color>");
        });
    }

    // ===== 다운로드 + 버전 비교 =====
    public void DownloadIfNewer()
    {
        IsReady = false;

        // 오프라인이면 서버 로드 스킵
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("오프라인 상태. 서버 로드 스킵"); 
            IsReady = true;
            return;
        }

        var localSave = Manager.Save.CurrentData;
        var uid = localSave.UserData.FirebaseUID;

        if (string.IsNullOrEmpty(uid))
        {
            IsReady = true;
            Debug.LogWarning("Firebase UID 없음. 서버 로드 스킵");
            return;
        }

        var doc = _db.Collection("users").Document(uid);

        doc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogWarning("서버 세이브 요청 실패. 로컬 유지");
                IsReady = true;
                return;
            }

            // 서버 문서 없음
            if (!task.Result.Exists)
            {
                Debug.LogWarning("서버 세이브 없음");
                IsReady = true;
                return;
            }

            long serverVersion = task.Result.GetValue<long>("version");
            long localVersion = localSave.SnapshotVersion;

            if (serverVersion > localVersion)
            {
                string json = task.Result.GetValue<string>("save");
                GameSaveSnapshot serverSave =
                    JsonUtility.FromJson<GameSaveSnapshot>(json);

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
}
