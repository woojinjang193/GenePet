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
    public bool IsLeft;
    public bool IsSick;

    public bool IsInfoUnlocked;
    public string ID;
    public string DisplayName;
    public string FatherId;
    public string MotherId;
    public GenesContainer Genes;
    public GrowthStatus GrowthStage;
    public float Hunger;
    public float Happiness;

    public float Cleanliness;
    public float Health;

    public PetSaveData()
    {
        IsLeft = false;
        IsSick = false;

        IsInfoUnlocked = false;
        ID = "";
        DisplayName = "";
        FatherId = "";
        MotherId = "";
        Genes = new GenesContainer();
        GrowthStage = GrowthStatus.Egg;
        Hunger = 100f;
        Happiness = 100f;
        Cleanliness = 100f;
        Health = 100f;
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

    public IslandData()
    {
        IsOpen = false;
        IsLeft = false;
        IsMarried = false;
        IslandMyPetID = "";
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
    public int MaxPetAmount;
    public float Energy;
    public List<PetSaveData> HavePetList;
    public List<PetRecordData> HadPetList;
    public IslandData Island;
    public List<IslandPetRecordData> IslandPetList;
    public UserItemData Items;

    public UserData()
    {
        UID = "";
        CurLanguage = Language.ENG;
        UserDisplayName = "";
        MaxPetAmount = 1;
        Energy = 10;
        HavePetList = new List<PetSaveData>();
        HadPetList = new List<PetRecordData>();
        Island = new IslandData();
        IslandPetList = new List<IslandPetRecordData>();
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
