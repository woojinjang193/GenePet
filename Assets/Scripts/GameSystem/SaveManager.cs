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
            Debug.LogWarning("세이브 데이터가 없습니다.");
            return;
        }

        if(isMine) //플레이어 펫일때
        {
            CurrentData.UserData.HavePet = pet;
            PetRecordData record = new PetRecordData(pet);
            record.Remark = "Raising";
            CurrentData.UserData.HadPetList.Add(record);
        }
        else // 섬 펫일때
        {
            CurrentData.UserData.Island.IslandPetSaveData = pet;
            //PetRecordData record = new PetRecordData(pet);
            //record.Remark = "Raising";
            //CurrentData.UserData.HadPetList.Add(record);
        }

        SaveGame();
    }

    public void RemovePet()
    {
        if (CurrentData.UserData.HavePet == null)
        {
            Debug.Log("삭제할 펫 정보 없음");
            return;
        }
        string name = CurrentData.UserData.HavePet.DisplayName;
        CurrentData.UserData.HavePet = new PetSaveData();
        Debug.Log($"{name} 삭제 완료");
        Manager.Game.PetLeft();
        SaveGame();
    }

    private void OnApplicationQuit()
    {
        // 현재 씬에 존재하는 모든 펫 검색
        var pet = FindObjectOfType<PetUnit>();

        // 세이브 데이터가 비었으면 저장 불가
        if (CurrentData == null || string.IsNullOrWhiteSpace(CurrentData.UserData.HavePet.ID))
        {
            Debug.Log("저장할 펫이 없음");
            return;
        }
            

        // 저장
        pet.UpdatePetSaveData(CurrentData.UserData.HavePet);

        // 모든 펫 데이터 반영 후 실제 세이브 파일 저장
        SaveGame();
    }

}