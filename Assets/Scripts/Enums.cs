using UnityEngine;

public enum GrowthStatus { Egg, Baby, Teen, Adult }
public enum PetStat { Hunger, Happiness, Energy, Cleanliness, Health, GrowthTimer, GrowthExp }
public enum PetFlag { IsSick, IsLeft }
public enum RarityType { Common, Rare, Epic, Legendary } //GeneManager 에서 확률 설정
//public enum Confirm { RemovePet, GiveUpPet, DeleteIsland, ChangingIslandMyPet }
public enum Gift { None, Gift1, Gift2, Gift3, MasterGift }
public enum Room { Default, Room_Jump, Room_Rythm, Room_Pinball, Room_Poor, Room_Cozy, Room_Something }
public enum GMPurchaseType {None, Coin, Gem }
public enum RewardCategory
{
    Item,   //일반 아이템 보상
    Egg     //알 보상
}
public enum RewardType
{
    
    Energy,
    Coin,
    RemovedAD,
    IslandTicket,
    GeneticScissors,
    GeneticTester,
    MissingPoster,
    Snack,
    Gift1, Gift2, Gift3,
    MasterGift,
    PetSlot,
    Room_Jump,
    Room_Rythm,
    Room_Pinball,
    Room_Poor,
    //Room_Cozy,
    Room_Something,
    Egg,
    GrowthBooster,
    GeneticGlue,
    Gem,
    GuaranteeSticker,
    None,
    
}
public enum PersonalityType 
{
    Brave, // 용감한
    Shy,  // 소심한
    Greedy, // 욕심 많음
    Fortitude, //불굴의 용기
    Persistent, //끈기있는
    Aggressive, // 공격적
    Calm, // 차분한
}

public enum PartType
{                   
    Body,           
    Arm,
    Feet,
    Eye,            
    Ear,            
    Mouth,
    Acc,           
    Wing,                   
    Pattern,        
    Color,          
    Blush,
    Tail,
    Whiskers,
    Personality     
}

public enum LeftReason
{
    Hunger,
    Sick,
    Unhappy,
    Dirty,
    NoReason
}

public enum Language
{
    KR = 0,
    EN = 1,
    DE = 2,
    JP = 3,
    CH = 4,
    SP = 5,
}
public enum MiniGame
{
    Jump,
    Rythm,
    Pinball,
    Null,
}
public enum RythmType
{
    Quarter,
    Eighth,
    Triplet,
}
public enum UIPanel
{
    None,
    Shop
}