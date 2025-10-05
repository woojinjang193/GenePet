using UnityEngine;

public sealed class PetController : MonoBehaviour
{
    [SerializeField] private PetUnit _pet;

    public PetStatusCore Status
    {
        get { return _pet != null ? _pet.Status : null; }
    }
    public PetConfigSO Config
    {
        get { return _pet != null ? _pet.CurrentConfig : null; }
    }

    public void Feed()
    {
        if (_pet == null || Status == null || Config == null) return;

        if (Status.GetStat(PetStat.Hunger) > 95f)
        {
            Debug.Log("이미 배부름");
            return;
        }
        Debug.Log($"밥먹음. 허기짐 : {Status.GetStat(PetStat.Hunger)}, 청결도 : {Status.GetStat(PetStat.Cleanliness)}");
        Status.AddStat(PetStat.Hunger, Config.FeedHungerGain);

        Status.AddStat(PetStat.Cleanliness, -5f);
    }

    public void Play(float resultScale = 1f)
    {
        if (_pet == null || Status == null || Config == null) return;

        if (Status.GetStat(PetStat.Energy) < Config.PlayEnergyCost)
        {
            Debug.Log($"에너지가 부족합니다 :{Status.GetStat(PetStat.Energy)}");
            return;
        }

        float happyGain = Config.PlayHappinessGain * Mathf.Clamp01(resultScale);

        Status.AddStat(PetStat.Happiness, happyGain);
        Status.AddStat(PetStat.Energy, -Config.PlayEnergyCost);
        Status.AddStat(PetStat.Cleanliness, -8f);
        Debug.Log($"놀아줌. 행복도 : {Status.GetStat(PetStat.Happiness)}, 에너지 : {Status.GetStat(PetStat.Energy)}");
    }

    public void Sleep(bool on)
    {
        if (_pet == null || Status == null) return;
        Status.SetFlag(PetFlag.IsSleeping, on);
    }

    public void Clean()
    {
        if (_pet == null || Status == null || Config == null) return;
        Status.AddStat(PetStat.Cleanliness, Config.CleanGain);
        Debug.Log($"목욕. 청결도 : {Status.GetStat(PetStat.Cleanliness)}");
    }

    public void Heal()
    {
        if (_pet == null || Status == null || Config == null) return;

        bool isSick = Status.GetFlag(PetFlag.IsSick);
        if (!isSick)
        {
            Debug.Log("안아픔");
            return;
        }
        

        // 간단히 아픔 해제 + 체력 회복
        Status.SetFlag(PetFlag.IsSick, false);
        Status.AddStat(PetStat.Health, Config.HealAmount);
        Debug.Log($"아픔 : {Status.GetFlag(PetFlag.IsSick)}");
    }

    public void SetGrowth(GrowthStatus next)
    {
        if (_pet == null || Status == null) return;
        Status.Growth = next;
    }

    public void ShowStatus()
    {
        if (Status == null)
        {
            Debug.LogError("스테이터스 없음");
            return;
        }

        string msg =
            "건강: " + Status.GetStat(PetStat.Health).ToString("F1") +
            ", 포만: " + Status.GetStat(PetStat.Hunger).ToString("F1") +
            ", 행복: " + Status.GetStat(PetStat.Happiness).ToString("F1") +
            ", 에너지: " + Status.GetStat(PetStat.Energy).ToString("F1") +
            ", 청결: " + Status.GetStat(PetStat.Cleanliness).ToString("F1") +
            ", 수면: " + (Status.GetFlag(PetFlag.IsSleeping) ? "T" : "F") +
            ", 아픔: " + (Status.GetFlag(PetFlag.IsSick) ? "T" : "F") +
            ", 불행포인트 :" + _pet._unHappyScore;
        Debug.Log("[PetController] " + msg);
    }
}
