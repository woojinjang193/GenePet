using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public GameSaveSnapshot CurrentData;
    private bool _isReady = false;
    public bool IsReady { get { return _isReady; } }

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
        snapshot.UserData.UID = Guid.NewGuid().ToString(); 
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

            CurrentData.UserData.HavePetList.Add(pet);

            PetRecordData record = new PetRecordData(pet);
            CurrentData.UserData.HadPetList.Add(record);

            Debug.Log($"펫 ID: {pet.ID} 등록");
        }
        else
        {
            CurrentData.UserData.Island.IslandPetSaveData = pet;
        }

        SaveGame();
    }
    public void RecordIslandPet()
    {
        PetSaveData pet = CurrentData.UserData.Island.IslandPetSaveData;
        IslandPetRecordData record = new IslandPetRecordData(pet);
        CurrentData.UserData.IslandPetList.Add(record);
    }

    public void RemovePet(string id)
    {
        for (int i = 0; i < CurrentData.UserData.HavePetList.Count; i++)
        {
            var pet = CurrentData.UserData.HavePetList[i];
            if(pet.ID == id)
            {
                CurrentData.UserData.HavePetList.RemoveAt(i);
                Debug.Log($"펫 {id} 삭제");
                break;
            }
        }
        SaveGame();
    }
    public void RemoveIslandPet()
    {
        if (CurrentData.UserData.Island.IslandPetSaveData == null)
        {
            Debug.Log("삭제할 펫 정보 없음");
            return;
        }
        CurrentData.UserData.Island.IslandPetSaveData = new PetSaveData();
        Debug.Log($"섬펫 삭제 완료");
        SaveGame();
    }

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

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}