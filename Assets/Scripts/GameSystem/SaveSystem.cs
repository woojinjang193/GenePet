using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GenePair
{
    public string DominantId;
    public string RecessiveId;

    public GenePair()
    { 
        DominantId = "";
        RecessiveId = "";
    }
}

[Serializable]
public class GenesContainer
{
    public GenePair Body;       
    public GenePair Arm;       
    public GenePair Feet;       
    public GenePair Pattern;    
    public GenePair Eye;        
    public GenePair Mouth;      
    public GenePair Ear;         
    public GenePair Acc;
    public GenePair Blush;
    public GenePair Wing;       

    public GenePair Color;      
    public GenePair Personality;
    public PartColorGenes PartColors;

    public GenesContainer()           
    {                                 
        Body = new GenePair();     
        Arm = new GenePair();
        Feet = new GenePair();
        Pattern = new GenePair();     
        Eye = new GenePair();         
        Mouth = new GenePair();       
        Ear = new GenePair();         
        Acc = new GenePair();        
        Wing = new GenePair();       
        Blush = new GenePair();

        Color = new GenePair();       
        Personality = new GenePair();
        PartColors = new PartColorGenes();
    }                                 
}

[Serializable]
public class PartColorGenes
{
    public string BodyColorId;
    public string ArmColorId;
    public string FeetColorId;
    public string PatternColorId;
    public string EarColorId;
    public string BlushColorId;

    public PartColorGenes()
    {
        BodyColorId = "";
        ArmColorId = "";
        FeetColorId = "";
        PatternColorId = "";
        EarColorId = "";
        BlushColorId = "";
    }
}

[Serializable]                  
public class PetSaveData
{
    public bool IsInfoUnlocked;
    public string ID;
    public int Seed;
    public string DisplayName;
    public string FatherId;
    public string MotherId;
    public GenesContainer Genes;
    public string GrowthStage;
    public float AgeSeconds;
    public float Hunger;
    public float Happiness;
    public float Energy;
    public float Cleanliness;
    public float Health;
    //public float Stress;

    public PetSaveData()
    {
        IsInfoUnlocked = false;
        ID = "";
        Seed = 0;
        DisplayName = "";
        FatherId = "";
        MotherId = "";
        Genes = new GenesContainer();
        GrowthStage = "Egg";
        AgeSeconds = 0f;
        Hunger = 100f;
        Happiness = 100f;
        Energy = 100f;
        Cleanliness = 100f;
        Health = 100f;
        //stress = 0f;
    }
}

[Serializable]
public class IslandData
{
    public bool IsOpen;
    public bool IsLeft;
    public bool IsMarried;
    public PetSaveData IslandPetSaveData;
    public float Affinity;
    public int VisitCount;

    public IslandData()
    {
        IsOpen = false;
        IsLeft = false;
        IsMarried = false;
        IslandPetSaveData = new PetSaveData();
        Affinity = 50f;
        VisitCount = 0;
    }
}

[Serializable]
public class UserItemData
{
    public int IslandTicket;
    public int MissingPoster;
    public int Item3Amount;
    public int Item4Amount;
    public int Item5Amount;

    public UserItemData()
    {
        IslandTicket = 1;
        MissingPoster = 1;
        Item3Amount = 0;
        Item4Amount = 0;
        Item5Amount = 0;
    }

}

[Serializable]
public class UserData
{
    public string UID;
    public Language CurLanguage;
    public string UserDisplayName;
    public PetSaveData HavePet;
    public List<PetRecordData> HadPetList;
    public IslandData Island;
    public UserItemData Items;

    public UserData()
    {
        UID = "";
        CurLanguage = Language.ENG;
        UserDisplayName = "";
        HavePet = new PetSaveData();
        HadPetList = new List<PetRecordData>();
        Island = new IslandData();
        Items = new UserItemData();
    }
}

[Serializable]
public class GameSaveSnapshot
{
    public int SchemaVersion;                                   // 스키마 버전(데이터 구조 변경 추적)
    public int SnapshotVersion;                                 // 스냅샷 버전(증분 저장 버전)
    public UserData UserData;

    public GameSaveSnapshot()                                   // 기본 생성자
    {                                                           // 생성자 시작
        SchemaVersion = 1;                                      // 초기 스키마 버전 1
        SnapshotVersion = 0;                                    // 초기 스냅샷 버전 0
        UserData = new UserData();
    }
}

[Serializable]                                                  // Json 직렬화 특성
public class PendingEvent                                       // 서버 미전송 이벤트 1건을 표현
{
    public string Type;                                         // 이벤트 타입(예: "breed","edit_gene","use_item")
    public int BaseVersion;                                     // 이벤트가 생성될 당시의 기반 스냅샷 버전
    public string ClientTimeIsoUtc;                             // 클라이언트 시각(표시용)
    public string PayloadJson;                                  // 이벤트 페이로드를 Json 문자열로 저장

    public PendingEvent()                                       // 기본 생성자
    {                                                           // 생성자 시작
        Type = "";                                              // 기본값 빈 문자열
        BaseVersion = 0;                                        // 기본값 0
        ClientTimeIsoUtc = "";                                  // 기본값 빈 문자열
        PayloadJson = "";                                       // 기본값 빈 문자열
    }                                                           // 생성자 끝
}

public static class SaveSystem                                  // 정적 세이브 시스템 클래스
{
    private static readonly string FolderName = "SaveData";     // 저장 폴더 이름 정의
    private static readonly string SnapshotFile = "snapshot.json"; // 스냅샷 파일명 정의
    private static readonly string EventsFile = "event_queue.jsonl"; // 이벤트 큐 파일명 정의
    private static readonly string BackupFile = "snapshot_backup.json"; // 스냅샷 백업 파일명 정의
    private static string _rootPath;                            // 저장 루트 경로(런타임에 계산)

    public static void Initialize()                              // 저장 시스템 초기화 함수
    {                                                           // 함수 시작
        _rootPath = Path.Combine(Application.persistentDataPath, FolderName); // 퍼시스턴트 경로와 폴더명 결합
        if (Directory.Exists(_rootPath) == false)               // 저장 폴더가 존재하지 않는지 확인
        {                                                       // 조건문 시작
            Directory.CreateDirectory(_rootPath);               // 폴더 생성
        }                                                       // 조건문 끝
    }                                                           // 함수 끝

    public static string GetSnapshotPath()                       // 스냅샷 파일의 전체 경로를 반환
    {                                                           // 함수 시작
        return Path.Combine(_rootPath, SnapshotFile);           // 루트 경로와 파일명을 결합해 반환
    }                                                           // 함수 끝

    public static string GetEventsPath()                         // 이벤트 큐 파일의 전체 경로를 반환
    {                                                           // 함수 시작
        return Path.Combine(_rootPath, EventsFile);             // 루트 경로와 파일명을 결합해 반환
    }                                                           // 함수 끝

    public static string GetBackupPath()                         // 백업 파일의 전체 경로를 반환
    {                                                           // 함수 시작
        return Path.Combine(_rootPath, BackupFile);             // 루트 경로와 파일명을 결합해 반환
    }                                                           // 함수 끝

    public static bool SaveSnapshot(GameSaveSnapshot snapshot)   // 스냅샷을 디스크에 저장하는 함수(성공 여부 반환)
    {                                                           // 함수 시작
        try                                                     // 예외 처리 시작
        {                                                       // 블록 시작
            string json = JsonUtility.ToJson(snapshot, true);   // 스냅샷 객체를 보기 좋은 포맷으로 Json 직렬화
            string tmpPath = GetSnapshotPath() + ".tmp";        // 원자적 쓰기를 위한 임시 파일 경로 생성
            byte[] bytes = Encoding.UTF8.GetBytes(json);        // UTF8 바이트 배열로 변환
            File.WriteAllBytes(tmpPath, bytes);                 // 임시 파일에 바이트 쓰기
            CreateBackupIfExists();                             // 기존 본파일이 있으면 백업 생성
            ReplaceFile(tmpPath, GetSnapshotPath());            // 임시 파일을 본파일로 원자적 교체
            return true;                                        // 성공 시 true 반환
        }                                                       // 블록 끝
        catch (Exception e)                                      // 예외 발생 시 처리
        {                                                       // 블록 시작
            Debug.LogError("SaveSnapshot 실패: " + e.Message);  // 에러 로그 출력
            return false;                                       // 실패 시 false 반환
        }                                                       // 블록 끝
    }                                                           // 함수 끝

    public static bool LoadSnapshot(out GameSaveSnapshot snapshot) // 스냅샷을 디스크에서 불러오는 함수(성공 여부 반환)
    {                                                           // 함수 시작
        snapshot = null;                                        // 출력 파라미터를 우선 null로 초기화
        try                                                     // 예외 처리 시작
        {                                                       // 블록 시작
            string path = GetSnapshotPath();                    // 스냅샷 파일 경로 획득
            if (File.Exists(path) == false)                     // 파일 존재 여부 확인
            {                                                   // 조건문 시작
                return false;                                   // 없으면 false 반환
            }                                                   // 조건문 끝
            string json = File.ReadAllText(path, Encoding.UTF8);// 파일 내용을 UTF8로 읽기
            GameSaveSnapshot data = JsonUtility.FromJson<GameSaveSnapshot>(json); // Json을 객체로 역직렬화
            if (data == null)                                   // 역직렬화 실패 여부 확인
            {                                                   // 조건문 시작
                return TryLoadBackup(out snapshot);             // 백업에서 로드 시도
            }                                                   // 조건문 끝
            snapshot = data;                                    // 성공 시 출력 파라미터에 대입
            return true;                                        // true 반환
        }                                                       // 블록 끝
        catch (Exception e)                                      // 예외 발생 시 처리
        {                                                       // 블록 시작
            Debug.LogWarning("LoadSnapshot 실패, 백업 시도: " + e.Message); // 경고 로그 출력
            return TryLoadBackup(out snapshot);                 // 백업에서 로드 시도
        }                                                       // 블록 끝
    }                                                           // 함수 끝

    private static bool TryLoadBackup(out GameSaveSnapshot snapshot) // 백업 파일에서 로드 시도 함수
    {                                                           // 함수 시작
        snapshot = null;                                        // 출력 파라미터 초기화
        try                                                     // 예외 처리 시작
        {                                                       // 블록 시작
            string backPath = GetBackupPath();                  // 백업 파일 경로 획득
            if (File.Exists(backPath) == false)                 // 백업 파일 존재 여부 확인
            {                                                   // 조건문 시작
                return false;                                   // 없으면 false 반환
            }                                                   // 조건문 끝
            string json = File.ReadAllText(backPath, Encoding.UTF8); // 백업 파일을 UTF8로 읽기
            GameSaveSnapshot data = JsonUtility.FromJson<GameSaveSnapshot>(json); // 역직렬화 시도
            if (data == null)                                   // 역직렬화 실패 시
            {                                                   // 조건문 시작
                return false;                                   // false 반환
            }                                                   // 조건문 끝
            snapshot = data;                                    // 성공 시 출력 파라미터 설정
            return true;                                        // true 반환
        }                                                       // 블록 끝
        catch (Exception e)                                      // 예외 처리
        {                                                       // 블록 시작
            Debug.LogError("TryLoadBackup 실패: " + e.Message); // 에러 로그 출력
            return false;                                       // 실패 시 false 반환
        }                                                       // 블록 끝
    }                                                           // 함수 끝

    private static void CreateBackupIfExists()                   // 기존 스냅샷이 있으면 백업을 만드는 함수
    {                                                           // 함수 시작
        string path = GetSnapshotPath();                        // 스냅샷 경로 획득
        if (File.Exists(path))                                  // 파일 존재 여부 확인
        {                                                       // 조건문 시작
            try                                                 // 예외 처리 시작
            {                                                   // 블록 시작
                string json = File.ReadAllText(path, Encoding.UTF8); // 기존 스냅샷을 읽기
                File.WriteAllText(GetBackupPath(), json, Encoding.UTF8); // 백업 파일로 그대로 저장
            }                                                   // 블록 끝
            catch (Exception e)                                  // 예외 처리
            {                                                   // 블록 시작
                Debug.LogWarning("백업 생성 실패: " + e.Message); // 경고 로그 출력
            }                                                   // 블록 끝
        }                                                       // 조건문 끝
    }                                                           // 함수 끝

    private static void ReplaceFile(string tmpPath, string finalPath) // 임시 파일을 본파일로 교체하는 함수
    {                                                           // 함수 시작
        if (File.Exists(finalPath))                             // 본파일이 이미 존재하는지 확인
        {                                                       // 조건문 시작
            File.Delete(finalPath);                             // 기존 본파일 삭제
        }                                                       // 조건문 끝
        File.Move(tmpPath, finalPath);                          // 임시 파일을 본파일 이름으로 이동(원자적 교체 유사)
    }                                                           // 함수 끝

    public static bool AppendEvent(PendingEvent ev)              // 이벤트 큐 파일에 1건 추가(성공 여부 반환)
    {                                                           // 함수 시작
        try                                                     // 예외 처리 시작
        {                                                       // 블록 시작
            string line = JsonUtility.ToJson(ev, false);        // 이벤트 객체를 한 줄 Json으로 직렬화
            string path = GetEventsPath();                      // 이벤트 파일 경로 획득
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read)) // Append 모드로 파일 열기
            {                                                   // using 블록 시작
                byte[] bytes = Encoding.UTF8.GetBytes(line + "\n"); // 줄바꿈 포함한 바이트 배열 생성
                fs.Write(bytes, 0, bytes.Length);               // 파일에 바이트 쓰기
                fs.Flush();                                     // 버퍼 플러시
            }                                                   // using 블록 끝
            return true;                                        // 성공 시 true 반환
        }                                                       // 블록 끝
        catch (Exception e)                                      // 예외 처리
        {                                                       // 블록 시작
            Debug.LogError("AppendEvent 실패: " + e.Message);   // 에러 로그 출력
            return false;                                       // 실패 시 false 반환
        }                                                       // 블록 끝
    }                                                           // 함수 끝

    public static List<PendingEvent> ReadAllPendingEvents()      // 모든 미전송 이벤트를 읽어 리스트로 반환
    {                                                           // 함수 시작
        List<PendingEvent> list = new List<PendingEvent>();     // 반환할 리스트 생성
        try                                                     // 예외 처리 시작
        {                                                       // 블록 시작
            string path = GetEventsPath();                      // 이벤트 파일 경로 획득
            if (File.Exists(path) == false)                     // 파일 존재 여부 확인
            {                                                   // 조건문 시작
                return list;                                    // 없으면 빈 리스트 반환
            }                                                   // 조건문 끝
            string[] lines = File.ReadAllLines(path, Encoding.UTF8); // 모든 라인을 UTF8로 읽기
            int i = 0;                                          // 반복문 인덱스 초기화
            while (i < lines.Length)                            // 모든 라인을 순회
            {                                                   // 반복문 시작
                string line = lines[i];                         // 현재 라인 가져오기
                if (string.IsNullOrEmpty(line) == false)        // 비어있지 않은 라인인지 확인
                {                                               // 조건문 시작
                    PendingEvent ev = JsonUtility.FromJson<PendingEvent>(line); // 한 줄 Json을 이벤트 객체로 역직렬화
                    if (ev != null)                             // 역직렬화 성공 여부 확인
                    {                                           // 조건문 시작
                        list.Add(ev);                           // 리스트에 추가
                    }                                           // 조건문 끝
                }                                               // 조건문 끝
                i = i + 1;                                      // 인덱스 증가
            }                                                   // 반복문 끝
        }                                                       // 블록 끝
        catch (Exception e)                                      // 예외 처리
        {                                                       // 블록 시작
            Debug.LogError("ReadAllPendingEvents 실패: " + e.Message); // 에러 로그 출력
        }                                                       // 블록 끝
        return list;                                            // 결과 리스트 반환
    }                                                           // 함수 끝

    public static bool ClearPendingEvents()                      // 이벤트 큐 파일을 비우는 함수
    {                                                           // 함수 시작
        try                                                     // 예외 처리 시작
        {                                                       // 블록 시작
            string path = GetEventsPath();                      // 이벤트 파일 경로 획득
            if (File.Exists(path))                              // 파일 존재 여부 확인
            {                                                   // 조건문 시작
                File.Delete(path);                              // 파일 삭제로 초기화
            }                                                   // 조건문 끝
            return true;                                        // 성공 시 true 반환
        }                                                       // 블록 끝
        catch (Exception e)                                      // 예외 처리
        {                                                       // 블록 시작
            Debug.LogError("ClearPendingEvents 실패: " + e.Message); // 에러 로그 출력
            return false;                                       // 실패 시 false 반환
        }                                                       // 블록 끝
    }                                                           // 함수 끝
}
