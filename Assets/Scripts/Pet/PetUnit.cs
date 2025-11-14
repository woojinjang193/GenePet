using System;
using UnityEngine;

public class PetUnit : MonoBehaviour
{
    [Header("코어")]
    private PetStatusCore _status = new PetStatusCore();
    private PetConfigSO _currentConfig;
    public PetConfigSO CurConfig => _currentConfig;
    public PetStatusCore Status => _status;

    private string _petId;


    //private PetManager _petManager;

    //private void Awake()
    //{
    //    _petManager = FindObjectOfType<PetManager>();
    //    if (_petManager == null)
    //    {
    //        Debug.LogError("PetManager 없음");
    //        enabled = false;
    //        return;
    //    }
    //}
    public void Init(PetSaveData save)
    {
        _petId = save.ID;

        _status.SetValues( save.ID, save.Hunger, save.Health, save.Cleanliness, save.Happiness );

        _status.SetFlag(PetFlag.IsLeft, save.IsLeft);
        _status.SetFlag(PetFlag.IsSick, save.IsSick);

        _status.Growth = save.GrowthStage;

        Debug.Log($"데이터 로드완료 ID: {_petId}");
    }

    public void SetConfig(PetConfigSO cfg)
    {
        _currentConfig = cfg;
        _status.SetConfig(cfg);
    }

    public bool TryGrow()
    {
        if (_currentConfig == null)
            return false;

        if (_status.Growth == GrowthStatus.Adult)
            return false;

        if (_status.GrowthTimer < _currentConfig.TimeToGrow)
            return false;

        if (_status.GrowthExp < _currentConfig.ExpToGrow)
            return false;

        GrowthStatus next = GetNextGrowth(_status.Growth);
        _status.Growth = next;

        _status.ResetGrowthProgress();

        return true;
    }

    private GrowthStatus GetNextGrowth(GrowthStatus cur)
    {
        switch (cur)
        {
            case GrowthStatus.Egg: 
                return GrowthStatus.Baby;

            case GrowthStatus.Baby: 
                return GrowthStatus.Teen;

            case GrowthStatus.Teen: 
                return GrowthStatus.Adult;

            default: return cur;
        }
    }
}
