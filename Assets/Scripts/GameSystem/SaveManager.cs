using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public GameSaveSnapshot CurrentData;

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
        }
        else
        {
            Debug.Log("세이브파일 없음. 새로 생성");
            CurrentData = CreateNewSave();
        }
    }

    private GameSaveSnapshot CreateNewSave()
    {
        GameSaveSnapshot snapshot = new GameSaveSnapshot();
        snapshot.UserData.UID = Guid.NewGuid().ToString(); 
        snapshot.SchemaVersion = 1;
        snapshot.SnapshotVersion = 1;
        CurrentData = snapshot;

        if (Manager.Gene.IsReady == true)
        {
            Manager.Game.CreateRandomPet();
        }
        else
        {
            StartCoroutine(WaitGeneAndCreatePetRoutine());
        }

        return snapshot;
    }

    private IEnumerator WaitGeneAndCreatePetRoutine()
    {
        while (Manager.Gene.IsReady == false)
        {
            yield return null;
        }
        Manager.Game.CreateRandomPet();
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

    public void AddNewPet(PetSaveData pet)
    {
        if (CurrentData == null)
        {
            Debug.LogWarning("세이브 데이터가 없습니다.");
            return;
        }

        CurrentData.UserData.HavePetList.Add(pet);
        PetRecordData record = new PetRecordData(pet);
        record.Remark = "Raising";
        CurrentData.UserData.HadPetList.Add(record);

        SaveGame();
    }
}