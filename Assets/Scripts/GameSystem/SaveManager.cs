using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public GameSaveSnapshot CurrentData;

    private bool _isSaving = false;
    private bool _isReady = false;
    public bool IsReady { get { return _isReady; } }

    public event Action OnAppPaused;//펫 데이터 저장용 PetManager 에서 구독
    protected override void Awake()
    {
        base.Awake();
        SaveSystem.Initialize();
        LoadOrCreateSave();
    }
    private void LoadOrCreateSave()
    {
        GameSaveSnapshot data;
        bool loaded = SaveSystem.LoadSnapshot(out data);

        if (loaded)
        {
            Debug.Log("기존 세이브파일 로드 완료");
            CurrentData = data;
            _isReady = true;
        }
        else
        {
            Debug.Log("세이브파일 없음. 새로 생성");
            CurrentData = CreateNewSave();
            _isReady = true;
        }
    }

    private GameSaveSnapshot CreateNewSave()
    {
        GameSaveSnapshot snapshot = new GameSaveSnapshot();
        snapshot.UserData.LocalUID = Guid.NewGuid().ToString(); 
        snapshot.SchemaVersion = 1;
        snapshot.SnapshotVersion = 1;
        CurrentData = snapshot;

        return snapshot;
    }

    public void SaveGame()
    {
        if (CurrentData == null)
        {
            Debug.LogWarning("저장할 데이터가 없습니다.");
            return;
        }
        //게임 매니저에서 펫 목록 받아와서 수치 저장 

        CurrentData.SnapshotVersion = CurrentData.SnapshotVersion + 1; // 저장 횟수 증가
        bool result = SaveSystem.SaveSnapshot(CurrentData); // 세이브 파일로 저장 시도
        if (result == true)
        {
            Debug.Log("세이브 완료");
        }
        else
        {
            Debug.LogError("세이브 실패");
        }
    }

    public void RegisterNewPet(PetSaveData pet, bool isMine)
    {
        if (CurrentData == null)
        {
            Debug.LogError("세이브 데이터가 없습니다.");
            return;
        }

        if (isMine) // 플레이어 소유 펫일 때
        {
            if (CurrentData.UserData.HavePetList == null)
            {
                CurrentData.UserData.HavePetList = new List<PetSaveData>();
            }
            if (CurrentData.UserData.HadPetList == null)
            {
                CurrentData.UserData.HadPetList = new List<PetSaveData>();
            }

            CurrentData.UserData.HavePetList.Add(pet);
            CurrentData.UserData.HadPetList.Add(pet);

            CurrentData.UserData.TotalRaisedPets++;
            Debug.Log($"펫 ID: {pet.ID} 등록");
        }
        else //섬 펫일때
        {
            CurrentData.UserData.Island.IslandPetSaveData = pet;
        }

        SaveGame();
    }
    public void RecordIslandPet(PetSaveData islandPet)
    {
        CurrentData.UserData.IslandPetList.Add(islandPet);
    }

    public void RemovePetData(string id)
    {
        for (int i = 0; i < CurrentData.UserData.HavePetList.Count; i++)
        {
            var pet = CurrentData.UserData.HavePetList[i];
            if(pet.ID == id)
            {
                CurrentData.UserData.HavePetList.RemoveAt(i);
                Debug.Log($"펫 {id} 삭제");

                if(id == CurrentData.UserData.Island.IslandMyPetID) //삭제한 펫이 섬에 등록된 펫이라면 등록된 펫아이디 지움
                {
                    CurrentData.UserData.Island.IslandMyPetID = "";
                    Debug.Log($"섬에 등록된 펫이라서 섬에서 삭제함");
                }
                break;
            }
        }
        SaveGame();
    }
    //public void RemoveIslandPet() //RemoveIsland()에서 IslandPetSaveData 삭제되서 주석처리
    //{
    //    if (CurrentData.UserData.Island.IslandPetSaveData == null)
    //    {
    //        Debug.Log("삭제할 펫 정보 없음");
    //        return;
    //    }
    //    CurrentData.UserData.Island.IslandPetSaveData = new PetSaveData();
    //    Debug.Log($"섬펫 삭제 완료");
    //    SaveGame();
    //}

    public void RemoveIsland()
    {
        if (CurrentData.UserData.Island == null)
        {
            Debug.Log("삭제할 섬 정보 없음");
            return;
        }
        CurrentData.UserData.Island = new IslandData();
        Debug.Log($"섬 삭제 완료");
        SaveGame();
    }

    //앱 종료, 씬전환, 앱 퍼즈시
    private void OnApplicationPause(bool pause)
    {
        if (_isSaving) return; //세이브 중이면 리턴

        if (pause)
        {
            _isSaving = true;
            
            OnAppPaused?.Invoke(); //펫 데이터 저장용
            SaveLastSaveTime();
            SaveGame();

            Debug.Log("SaveManager Quit 시간 + 서버 저장");
            Manager.Server.TryUploadOnBackground();
        } 
        else
        {
            _isSaving = false;
        }
    }
    private void OnApplicationQuit()
    {
        if (_isSaving) { Debug.LogWarning("OnApplicationQuit 호출, _isSaving = true 라서 리턴"); return; } //세이브 중이면 리턴

        _isSaving = true;
        
        OnAppPaused?.Invoke();
        SaveLastSaveTime();
        SaveGame();

        Debug.Log("OnApplicationQuit 호출됨");
        Manager.Server.TryUploadOnBackground();
    }
    public void SaveLastSaveTime()
    {
        CurrentData.UserData.LastSavedUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Debug.Log($"마지막 저장시간 : {CurrentData.UserData.LastSavedUnixTime}");
    }
    public void SaveMainSceneLeaveTime()
    {
        CurrentData.UserData.LastPetSavedUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Debug.Log($"메인씬 떠난 시간 : {CurrentData.UserData.LastPetSavedUnixTime}");
    }
}
