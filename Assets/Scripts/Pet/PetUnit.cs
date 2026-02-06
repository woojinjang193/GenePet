using System;
using UnityEngine;

public class PetUnit : MonoBehaviour
{
    [Header("쓰다듬기")]
    [SerializeField] private PetPetting _petting;
    
    private PetStatusCore _status = new PetStatusCore();
    private PetConfigSO _currentConfig;
    public PetConfigSO CurConfig => _currentConfig;
    public PetStatusCore Status => _status;
    public PetManager Petmanager { get; private set; }

    private string _petId;
    public string PetId => _petId;

    private PetVisualController _visul;
    public bool LeftHandled { get; set; }

    //--------------쓰다듬기 행복도---------------------
    private long _pettingCooldownSec; // 120분 쿨다운(초)
    private float _pettingHappinessAdd;     //한 번에 오르는 행복도

    private PetSaveData _saveRef;

    public PetSaveData SaveData => _saveRef;
    private void OnDestroy()
    {
        _status.OnCleanlinessChanged -= _visul.OnCleanlinessChanged;
        _status.OnSick -= _visul.OnSick;
        _status.OnHealthReducing -= _visul.OnHealthReducing;
        _status.OnGrown -= _visul.OnGrown;
        _petting.OnPettingChanged -= OnPettingChanged;
    }
    public void Init(PetSaveData save, PetManager petManager)
    {
        Petmanager = petManager;
        _petId = save.ID;
        _saveRef = save;

        _status.SetValues(PetStat.Hunger, save.Hunger);
        _status.SetValues(PetStat.Health, save.Health);
        _status.SetValues(PetStat.Cleanliness, save.Cleanliness);
        _status.SetValues(PetStat.Happiness, save.Happiness);
 
        _status.SetFlag(PetFlag.IsLeft, save.IsLeft);
        _status.SetFlag(PetFlag.IsSick, save.IsSick);

        _status.SetGrowthExp(save.GrowthExp);

        _status.Growth = save.GrowthStage;
        _visul = GetComponent<PetVisualController>();

        _status.OnCleanlinessChanged += _visul.OnCleanlinessChanged;
        _status.OnSick += _visul.OnSick;
        _status.OnHealthReducing += _visul.OnHealthReducing;
        _status.OnGrown += _visul.OnGrown; //성장이벤트 구독

        _petting.OnPettingChanged += OnPettingChanged;
        //Debug.Log($"데이터 로드완료 ID: {_petId}");

        var config = Manager.Game.Config;

        _pettingHappinessAdd = config.PettingHappinessAdd;
        _pettingCooldownSec = (long)(config.PettingCooldownHour * 60f * 60f);
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

        _status.ResetGrowthProgress();

        GrowthStatus next = GetNextGrowth(_status.Growth);
        _status.Growth = next;

        if (_saveRef != null) // 성장 결과를 세이브데이터에도 즉시 반영
        {
            _saveRef.GrowthStage = next;  //즉시 반영
            //_saveRef.GrowthExp = _status.GrowthExp; // 남은 성장 경험치도 동기화
        }

        _visul.SetSprite(_status.Growth);
        return true;
    }
    public bool ForceGrowOneStage() // 아이템으로 강제 1단계 성장
    {
        if (_status.Growth == GrowthStatus.Adult) return false; // 성체면 실패

        GrowthStatus next = GetNextGrowth(_status.Growth); // 다음 단계 계산
        _status.Growth = next; // 성장단계 변경(이벤트 포함)

        _status.ResetGrowthToZero(); // 성장 진행도 0으로 리셋

        if (_saveRef != null) _saveRef.GrowthStage = next; // 저장데이터 즉시 반영

        if (_visul != null) _visul.SetSprite(next); // 스프라이트 갱신

        return true; //성공
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
    public void ZoomThisPet(bool on)
    {
        //Debug.Log($"줌됨 {on}");
        _visul.AllowToClickLetter(on);
    }
    // ==============펫 쓰다듬기 이벤트====================
    private void OnPettingChanged(bool on)
    {
        if (!on) return;   // 시작(true)일 때만 체크
        if (Status.Growth == GrowthStatus.Egg) return; //알일땐 행복도 안올림
        if (Status.IsLeft) return; //떠난 상태면 행복도 안올림

        //Debug.Log("쓰다듬 쿨타임 체크");
        TryGivePettingHappiness();  // 쿨다운 체크 후 지급
    }
    private void TryGivePettingHappiness() // 쓰다듬 행복도 지급
    {
        if (_saveRef == null) return; // 세이브 참조 없으면 불가

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //현재 UTC 시간
        long last = _saveRef.LastPettingHappinessUnixTime;  //  마지막 지급 시간

        if (now - last < _pettingCooldownSec) return;   //  쿨다운이면 지급 안 함

        _saveRef.LastPettingHappinessUnixTime = now;    // 마지막 지급 시간 갱신

        //행복도 증가(세이브 + 스테이터스 둘 다 반영)
        _saveRef.Happiness += _pettingHappinessAdd;           //  저장데이터 반영
        _status.SetValues(PetStat.Happiness, _saveRef.Happiness); // 런타임 스테이터스 반영
        Petmanager.UpdateStatus(); //UI 업데이트

        Manager.Save.SaveGame(); //즉시 저장
    }
}
