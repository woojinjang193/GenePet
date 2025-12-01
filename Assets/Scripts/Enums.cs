using UnityEngine;

public enum GrowthStatus { Egg, Baby, Teen, Adult }
public enum PetStat { Hunger, Happiness, Energy, Cleanliness, Health, GrowthTimer, GrowthExp }
public enum PetFlag { IsSick, IsLeft }
public enum RarityType { Common, Rare, Epic, Legendary } //GeneManager 에서 확률 설정

public enum Confirm { RemovePet, GiveUpPet}
public enum PersonalityType 
{
    Brave,          // 용감한 - 체력·행동력 높음, 싸움 잘함
    Shy,            // 소심한 - 낯가림, 스트레스 잘 받음
    Friendly,       // 친화적인 - 행복도 회복 빠름
    Lazy,           // 게으른 - 에너지 소모 적지만 성장 느림
    Curious,        // 호기심 많음 - 새로운 행동 자주 시도
    Greedy,         // 욕심 많음 - 음식 자주 원함
    Calm,           // 차분한 - 스트레스 잘 안 받음
    Aggressive,     // 공격적 - 다른 펫과 잘 싸움
    Playful,        // 장난꾸러기 - 행복도 쉽게 오르지만 피곤해짐
    Lonely,         // 외로움 잘 느낌 - 상호작용 없으면 스트레스 증가
    Intelligent,    // 똑똑함 - 성장 효율 높음  
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
    Unhappy,
    Dirty,
    LayEgg,
}

public enum Language
{
    KOR = 0,
    ENG = 1,
    JP = 2,
    DE = 3,
    CH = 4,
}