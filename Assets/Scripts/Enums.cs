public enum GrowthStatus { Egg, Baby, Teen, Teen_Rebel, Adult }
public enum PetStat { Hunger, Happiness, Energy, Cleanliness, Health, GrowthTimer, GrowthExp }
public enum PetFlag { IsSick, IsSleeping, IsLeft }
//public enum PartType { Body, Color, Pattern, Ear, Tail, Wing, Eye, Mouth}
public enum RarityType { Common, Rare, Unique, Legendary }
public enum Personality 
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