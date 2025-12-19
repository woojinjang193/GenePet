using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[Serializable]
public class GenePair
{
    public string DominantId;
    public string RecessiveId;
    public bool IsDominantCut;
    public bool IsRecessiveCut;
    public GenePair()
    {
        DominantId = "";
        IsDominantCut = false;

        RecessiveId = "";
        IsRecessiveCut = false;
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
    public GenePair Tail;
    public GenePair Whiskers;

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
        Tail = new GenePair();
        Whiskers = new GenePair();

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
    public string WingColorId;
    public string TailColorId;
    //public string WhiskersColorId;
    //public string BlushColorId;

    public PartColorGenes()
    {
        BodyColorId = "";
        ArmColorId = "";
        FeetColorId = "";
        PatternColorId = "";
        EarColorId = "";
        WingColorId = "";
        TailColorId = "";
        //WhiskersColorId = "";
    }
}
[Serializable]
public class EggData
{
    //public Sprite Image;
    //획득시간 추가할까??
    public PetSaveData PetSaveData;

    public EggData()
    {
        //Image = null;
        PetSaveData = new PetSaveData();
    }
}

[Serializable]                  
public class PetSaveData
{
    public RarityType Rarity;
    public Sprite EggSprite;

    public bool IsLeft;
    public bool IsSick;
    
    public string ID;
    public string DisplayName;
    public string FatherId;
    public string MotherId;

    public bool IsInfoUnlocked;
    public GenesContainer Genes;

    public GrowthStatus GrowthStage;
    public float Hunger;
    public float Happiness;
    public float Cleanliness;
    public float Health;

    public float AgeSeconds;
    public float GrowthExp;

    public PetSaveData()
    {
        Rarity = RarityType.Common;
        EggSprite = null;

        IsLeft = false;
        IsSick = false;
        
        ID = "";
        DisplayName = "";
        FatherId = "";
        MotherId = "";

        IsInfoUnlocked = false;
        Genes = new GenesContainer();

        GrowthStage = GrowthStatus.Egg;
        Hunger = 100f;
        Happiness = 100f;
        Cleanliness = 100f;
        Health = 100f;

        AgeSeconds = 0f; //필요없나?
        GrowthExp = 0f;
    }
}

[Serializable]
public class IslandData
{
    public bool IsOpen;
    public bool IsLeft;
    public bool IsMarried;
    public string IslandMyPetID;
    public PetSaveData IslandPetSaveData;
    public float Affinity;
    public int VisitCount;
    public long LastVisitTime;
    public long GiftCooldownStartTime;
    public Gift CurWish;

    public IslandData()
    {
        IsOpen = false;
        IsLeft = false;
        IsMarried = false;
        IslandMyPetID = "";
        IslandPetSaveData = new PetSaveData();
        Affinity = 0f;
        VisitCount = 0;
        LastVisitTime = 0;
        GiftCooldownStartTime = 0;
        CurWish = Gift.None;
    }
}

[Serializable]
public class UserItemData
{
    public int Money; //소지금
    public bool IsAdRemoved;
    public int IslandTicket;
    public int MissingPoster;
    public int GeneticScissors;
    public int geneticTester;
    public int Snack;

    //선물
    public int MasterGift;
    public int Gift1;
    public int Gift2;
    public int Gift3;
    public int Gift4;

    public UserItemData()
    {
        Money = 0;
        IsAdRemoved = false;
        IslandTicket = 1;
        MissingPoster = 1;
        GeneticScissors = 1;
        geneticTester = 1;
        Snack = 1;

        MasterGift = 5;
        Gift1 = 1;
        Gift2 = 1;
        Gift3 = 1;
        Gift4 = 1;
    }
}

[Serializable]
public class UserData
{
    
    public long LastPlayedUnixTime; //마지막 접속 시간
    public string UID; // UID
    public Language CurLanguage; //현재 언어
    public string UserDisplayName; // 유저네임
    public int PetSlot; // 최대 보유 가능 펫 수
    public int Energy; //에너지
    public List<EggData> EggList; // 보유 알 리스트
    public List<PetSaveData> HavePetList; // 보유 펫 리스트
    public List<PetSaveData> HadPetList; // 보유했던 펫 리스트
    public IslandData Island; // 섬 정보
    public List<PetSaveData> IslandPetList; //만난 섬 펫 리스트
    public UserItemData Items; // 아이템

    public UserData()
    {
        LastPlayedUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        UID = "";
        CurLanguage = Language.ENG;
        UserDisplayName = "";
        PetSlot = 1; //플레이어 맥스 펫 수
        Energy = 10;
        EggList = new List<EggData>();
        HavePetList = new List<PetSaveData>();
        HadPetList = new List<PetSaveData>();
        Island = new IslandData();
        IslandPetList = new List<PetSaveData>();
        Items = new UserItemData();
    }
}

[Serializable]
public class GameSaveSnapshot
{
    public int SchemaVersion;                               
    public int SnapshotVersion;                             
    public UserData UserData;

    public GameSaveSnapshot()                               
    {                                                       
        SchemaVersion = 1;                                  
        SnapshotVersion = 0;                                
        UserData = new UserData();
    }
}

[Serializable]                                              
public class PendingEvent                                
{
    public string Type;                                  
    public int BaseVersion;                              
    public string ClientTimeIsoUtc;                      
    public string PayloadJson;                           

    public PendingEvent()                                   
    {                                                       
        Type = "";                                          
        BaseVersion = 0;                                    
        ClientTimeIsoUtc = "";                              
        PayloadJson = "";                                   
    }                                                       
}

public static class SaveSystem                              
{
    private static readonly string FolderName = "SaveData";     
    private static readonly string SnapshotFile = "snapshot.json"; 
    private static readonly string EventsFile = "event_queue.jsonl"; 
    private static readonly string BackupFile = "snapshot_backup.json"; 
    private static string _rootPath;                           

    public static void Initialize()                              
    {                                                       
        _rootPath = Path.Combine(Application.persistentDataPath, FolderName);
        if (Directory.Exists(_rootPath) == false)               
        {                                                       
            Directory.CreateDirectory(_rootPath);               
        }                                                       
    }                                                           

    public static string GetSnapshotPath()                      
    {                                                           
        return Path.Combine(_rootPath, SnapshotFile);           
    }                                                           

    public static string GetEventsPath()                        
    {                                                           
        return Path.Combine(_rootPath, EventsFile);             
    }                                                           

    public static string GetBackupPath()                        
    {                                                           
        return Path.Combine(_rootPath, BackupFile);             
    }                                                           

    public static bool SaveSnapshot(GameSaveSnapshot snapshot)  
    {                                                           
        try                                                     
        {                                                       
            string json = JsonUtility.ToJson(snapshot, true);   
            string tmpPath = GetSnapshotPath() + ".tmp";        
            byte[] bytes = Encoding.UTF8.GetBytes(json);        
            File.WriteAllBytes(tmpPath, bytes);                 
            CreateBackupIfExists();                             
            ReplaceFile(tmpPath, GetSnapshotPath());            
            return true;                                        
        }                                                       
        catch (Exception e)                                     
        {                                                       
            Debug.LogError("SaveSnapshot 실패: " + e.Message);  
            return false;                                       
        }                                                       
    }                                                           

    public static bool LoadSnapshot(out GameSaveSnapshot snapshot) 
    {                                                           
        snapshot = null;                                        
        try                                                     
        {                                                       
            string path = GetSnapshotPath();                    
            if (File.Exists(path) == false)                     
            {                                                   
                return false;                                   
            }                                                   
            string json = File.ReadAllText(path, Encoding.UTF8);
            GameSaveSnapshot data = JsonUtility.FromJson<GameSaveSnapshot>(json); 
            if (data == null)                                   
            {                                                   
                return TryLoadBackup(out snapshot);             
            }                                                   
            snapshot = data;                                    
            return true;                                        
        }                                                       
        catch (Exception e)                                     
        {                                                       
            Debug.LogWarning("LoadSnapshot 실패, 백업 시도: " + e.Message); 
            return TryLoadBackup(out snapshot);                 
        }                                                       
    }                                                           

    private static bool TryLoadBackup(out GameSaveSnapshot snapshot) 
    {                                                           
        snapshot = null;                                        
        try                                                     
        {                                                       
            string backPath = GetBackupPath();                  
            if (File.Exists(backPath) == false)                 
            {                                                   
                return false;                                   
            }                                                   
            string json = File.ReadAllText(backPath, Encoding.UTF8);
            GameSaveSnapshot data = JsonUtility.FromJson<GameSaveSnapshot>(json);
            if (data == null)                                   
            {                                                   
                return false;                                   
            }                                                   
            snapshot = data;                                    
            return true;                                        
        }                                                       
        catch (Exception e)                                     
        {                                                       
            Debug.LogError("TryLoadBackup 실패: " + e.Message); 
            return false;                                       
        }                                                       
    }                                                           

    private static void CreateBackupIfExists()                  
    {                                                           
        string path = GetSnapshotPath();                        
        if (File.Exists(path))                                  
        {                                                       
            try                                                 
            {                                                   
                string json = File.ReadAllText(path, Encoding.UTF8);
                File.WriteAllText(GetBackupPath(), json, Encoding.UTF8);
            }                                                  
            catch (Exception e)                                
            {                                                  
                Debug.LogWarning("백업 생성 실패: " + e.Message); 
            }                                                  
        }                                                      
    }                                                          

    private static void ReplaceFile(string tmpPath, string finalPath) 
    {                                                           
        if (File.Exists(finalPath))                             
        {                                                       
            File.Delete(finalPath);                             
        }                                                       
        File.Move(tmpPath, finalPath);                          
    }                                                           

    public static bool AppendEvent(PendingEvent ev)             
    {                                                           
        try                                                     
        {                                                       
            string line = JsonUtility.ToJson(ev, false);        
            string path = GetEventsPath();                      
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read)) 
            {                                                  
                byte[] bytes = Encoding.UTF8.GetBytes(line + "\n"); 
                fs.Write(bytes, 0, bytes.Length);               
                fs.Flush();                                     
            }                                                   
            return true;                                        
        }                                                       
        catch (Exception e)                                     
        {                                                       
            Debug.LogError("AppendEvent 실패: " + e.Message);   
            return false;                                       
        }                                                       
    }                                                           

    public static List<PendingEvent> ReadAllPendingEvents()     
    {                                                           
        List<PendingEvent> list = new List<PendingEvent>();     
        try                                                     
        {                                                       
            string path = GetEventsPath();                      
            if (File.Exists(path) == false)                     
            {                                                   
                return list;                                    
            }                                                   
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            int i = 0;                                          
            while (i < lines.Length)                            
            {                                                   
                string line = lines[i];                         
                if (string.IsNullOrEmpty(line) == false)        
                {                                               
                    PendingEvent ev = JsonUtility.FromJson<PendingEvent>(line); 
                    if (ev != null)                            
                    {                                          
                        list.Add(ev);                          
                    }                                          
                }                                              
                i = i + 1;                                     
            }                                                  
        }                                                      
        catch (Exception e)                                    
        {                                                      
            Debug.LogError("ReadAllPendingEvents 실패: " + e.Message); 
        }                                                      
        return list;                                           
    }                                                          

    public static bool ClearPendingEvents()                     
    {                                                           
        try                                                     
        {                                                       
            string path = GetEventsPath();                      
            if (File.Exists(path))                              
            {                                                   
                File.Delete(path);                              
            }                                                   
            return true;                                        
        }                                                       
        catch (Exception e)                                     
        {                                                       
            Debug.LogError("ClearPendingEvents 실패: " + e.Message); 
            return false;                                    
        }                                                    
    }                                                        
}
