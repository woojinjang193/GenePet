using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{

    public long LastPlayedUnixTime; //마지막 접속 시간
    public string LocalUID; // UID
    public string FirebaseUID; // UID
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
        LocalUID = "";
        FirebaseUID = "";
        CurLanguage = Language.EN;
        UserDisplayName = "";
        PetSlot = 1; //플레이어 맥스 펫 수
        Energy = 50;
        EggList = new List<EggData>();
        HavePetList = new List<PetSaveData>();
        HadPetList = new List<PetSaveData>();
        Island = new IslandData();
        IslandPetList = new List<PetSaveData>();
        Items = new UserItemData();
    }
}

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
    public Room RoomType;

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
        RoomType = Room.Default;

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
    public List<Room> Rooms;

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
        Rooms = new List<Room>() { Room.Default, Room.Room1, Room.Room2, Room.Room3, Room.Room4, Room.Room5 };

        MasterGift = 5;
        Gift1 = 1;
        Gift2 = 1;
        Gift3 = 1;
        Gift4 = 1;
    }

    [Serializable]
    public class MiniGameData
    {
        public int PlayCount;
        public int BestScore;
        public MiniGameData() 
        {
            PlayCount = 0;
            BestScore = 0;
        }
    }
}


