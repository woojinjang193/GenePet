using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

// ===================== 미니게임 결과 저장용 데이터 =====================
[Serializable]
public class MiniGameData
{
    public int PlayCount; // 플레이 횟수
    public int BestScore; // 최고 점수
    public MiniGameData()
    {
        PlayCount = 0;
        BestScore = 0;
    }
}

[Serializable]
public class UserData
{
    public int TotalRaisedPets;
    public long LastSavedUnixTime; //마지막 세이브 타임
    public long LastPetSavedUnixTime; //마지막 펫 상태 저장 타임
    public List<RewardClaimRecord> RewardClaims;

    public string LocalUID; // UID
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

    public MiniGameData[] MiniGameResults; // 미니게임 결과(플레이횟수/최고점수)

    public UserData()
    {
        TotalRaisedPets = 0;
        LastSavedUnixTime = 0;
        LastPetSavedUnixTime = 0;
        RewardClaims = new();

        LocalUID = "";
        CurLanguage = Language.None;
        UserDisplayName = "";
        PetSlot = 1; //플레이어 맥스 펫 수
        Energy = 10;
        EggList = new List<EggData>();
        HavePetList = new List<PetSaveData>();
        HadPetList = new List<PetSaveData>();
        Island = new IslandData();
        IslandPetList = new List<PetSaveData>();
        Items = new UserItemData();

        MiniGameResults = new MiniGameData[(int)MiniGame.Null]; // Null 제외(0~2) 3칸 생성
        for (int i = 0; i < MiniGameResults.Length; i++) // 각 칸 초기화
        {
            MiniGameResults[i] = new MiniGameData(); // 기본값 세팅
        }
    }
}
[Serializable]
public class RewardClaimRecord
{
    public string ID;
    public string LastDate;

    public RewardClaimRecord()
    {
        ID = "";
        LastDate = "";
    }
}

[Serializable]
public class GenePair
{
    public string DominantId;
    public string RecessiveId;
    public bool IsDominantCut;
    public bool IsRecessiveCut;

    public bool IsDoGuaranteed;
    public bool IsReGuaranteed;
    public GenePair()
    {
        DominantId = "";
        IsDominantCut = false;
        IsDoGuaranteed = false;

        RecessiveId = "";
        IsRecessiveCut = false;
        IsReGuaranteed = false;
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
    //public Sprite EggSprite;
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

    public long LastPettingHappinessUnixTime; // 마지막 쓰다듬 행복도 지급 시간


    public PetSaveData()
    {
        Rarity = RarityType.Common;
        //EggSprite = null;
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
        Happiness = 0f;
        Cleanliness = 100f;
        Health = 100f;

        AgeSeconds = 0f; //필요없나?
        GrowthExp = 0f;

        LastPettingHappinessUnixTime = 0;
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
    public List<string> PurchasedGoldNCs;

    public int Money; //소지금
    public bool IsAdRemoved;
    public int IslandTicket;
    public int MissingPoster;
    public int GeneticScissors;
    public int geneticTester;
    public int Snack;
    public int GrowthBooster;
    public int GeneticGlue;
    public int Gem;
    public int GuaranteeSticker;

    //차후 업데이트할때 추가될걸 대비한 아이템 미리 생성
    public int TimeSpeedUp; //추후 내펫끼리 교배에 쓰일수 있는 아이템
    public int AdditionalItem1; //추후 추가될수도 있는 아이템
    public int AdditionalItem2; //추후 추가될수도 있는 아이템
    public int AdditionalItem3; //추후 추가될수도 있는 아이템
    public int AdditionalItem4; //추후 추가될수도 있는 아이템
    public int AdditionalItem5; //추후 추가될수도 있는 아이템

    public List<Room> Rooms;

    //선물
    public int MasterGift;
    public int Gift1;
    public int Gift2;
    public int Gift3;

    public UserItemData()
    {
        PurchasedGoldNCs = new List<string>();

        Money = 2000;
        IsAdRemoved = false;
        IslandTicket = 1;
        MissingPoster = 1;
        GeneticScissors = 1;
        geneticTester = 1;
        Snack = 1;
        GrowthBooster = 1;
        GeneticGlue = 1;
        Gem = 20;
        GuaranteeSticker = 1;

        Rooms = new List<Room>()  //유저가 가진 방 목록
        { Room.Default};

        MasterGift = 5;
        Gift1 = 1;
        Gift2 = 1;
        Gift3 = 1;
    }
}


