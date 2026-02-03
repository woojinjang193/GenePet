using UnityEngine;

[CreateAssetMenu(fileName = "New GameConfigSO", menuName = "SO/GameConfigSO")]
public class GameConfig : ScriptableObject
{
    [Header("<color=yellow>게임 설정</color>")]
    [Header("백그라운드 게임저장 간격")]
    public int UploadIntervalSec;
    [Header("MissingPoster 광고 사용 리밋")]
    public int LimitForMissingPosterAD;
    [Header("일일 광고 보상 리밋")]
    public int LimitForDailyRewardAD;

    [Header("<color=yellow>유저 설정</color>")]
    [Header("최대 소유 펫 수")]
    public int MaxPetAmount;
    [Header("최대 소유 알 수")]
    public int MaxEggAmount;
    [Header("맥스 에너지")]
    public int MaxEnergy;
    [Header("에너지 1오르는데 걸리는 시간")]
    public float EnergyRecoveringTime;

    [Header("<color=yellow>펫 설정</color>")]
    [Header("쓰다듬기 행복도 양")]
    public float PettingHappinessAdd;
    [Header("쓰다듬기 행복도 쿨타임(시)")]
    public float PettingCooldownHour;

    [Header("<color=yellow>미니게임 설정</color>")]
    [Header("미니게임 경험치")]
    public float MiniGameEXP;
    [Header("미니게임 행복도")]
    public float MiniGameHappiness;

    [Header("<color=yellow>섬 설정</color>")]
    [Header("섬 방문 호감도 쿨타임")]
    public float VisitingAffinityCooldown;
    [Header("섬 방문 호감도 양")]
    public float VisitingAffinityGain;
    [Header("선물주기 가능 쿨타임")]
    public float GiftCooldown;
    [Header("선물주기 호감도 증가량")]
    public float GiftingPoint;
    [Header("선물 안주면 줄어드는 호감도 양")]
    public float DisappointingPoint;

    [Header("<color=yellow>먹이 설정</color>")]

    [Header("밥먹일 수 있는 기준 포만도")]
    public float CanFeedPetBelow;
    [Header("밥 먹으면 오르는 포만도 양")]
    public float MealFullnessGain;
    [Header("밥 먹으면 내려가는 청결도 양")]
    public float MealCleanlinessDecrease;

    [Header("간식 먹으면 오르는 포만도 양")]
    public float SnackFullnessGain;
    [Header("간식 먹으면 오르는 경험치 양")]
    public float SnackExpGain;
    [Header("간식 먹으면 내려가는 청결도 양")]
    public float SnackCleanlinessDecrease;

    [Header("<color=yellow>소환 설정</color>")]

    [Header("랜덤 스폰 가격")]
    public int RandomSpawnPrice;

    [Header("<color=yellow>가출후 데려온 후 스텟</color>")]
    [Header("포만감")]
    public float ComeBackHunger;
    [Header("청결도")]
    public float ComeBackCleanliness;
    [Header("행복도(감소량)")]
    public float ComeBackHappiness;
    [Header("체력")]
    public float ComeBackHealth;
}
