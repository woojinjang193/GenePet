using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBase : MonoBehaviour 
{
    protected RewardReservation _rewardReservation = new(); // 라운드/스폰 제한용 예약 상태
    public RewardReservation RewardReservation => _rewardReservation; // 보상 스포너에서 접근용

    protected PetSaveData _pet; // 플레이 중인 펫 데이터
    protected int _score;  // 점수
    public int Score => _score;

    protected float _playSecond;

    protected Dictionary<RewardType, int> _gainedItems = new(); // 획득 아이템 누적
    protected List<EggData> _gainedEggs = new(); //알 보상 누적(개별 표시용)
    protected bool _isRewardsFinalized; //보상 지급/누적 1회만 하도록 가드

    protected bool _isPlaying;
    protected bool _isGameOver;

    protected MiniGameContext _effectContext; // 미니게임 효과 컨텍스트

    public event Action OnGameOver;
    public event Action OnGameStart;

    protected virtual void Start()
    {
        if(Manager.Audio != null)
        {
            Manager.Audio.StopBGM();
        }

        _gainedItems.Clear();    // 보상 기록 초기화
        _gainedEggs.Clear();     //알 보상 초기화
        _isRewardsFinalized = false; //지급 가드 초기화

        _pet = Manager.Mini.CurPet;

        MiniGamePersonalityEffectSO table = Manager.Mini.GetEffectTable(); // 미니게임별 테이블
        
        //성격 가져오기
        string personalityID = _pet.Genes.Personality.DominantId;
        PersonalitySO personalitySO = Manager.Gene.GetPartSOByID<PersonalitySO>(PartType.Personality, personalityID);
        PersonalityType petsonality = personalitySO.Personality;
        
        float happiness01 = _pet.Happiness / 100;
        _effectContext = MiniGameEffectApplier.Apply(table, petsonality, happiness01); //성격 적용

        var user = Manager.Save.CurrentData.UserData; // 예약 초기화에 유저 필요
        int maxEgg = Manager.Game.Config.MaxEggAmount; // 최대 알 수 필요
        _rewardReservation.ResetFromUser(user, maxEgg); // 라운드 시작 예약 상태 초기화
    }
    protected virtual void GameStart()
    {
        OnGameStart?.Invoke();
        _isPlaying = true;
    }
    protected virtual void GameReset()
    {
        _score = 0;  // 점수 초기화
        _playSecond = 0; //플레이 시간 초기화
        _isGameOver = false;

        _gainedItems.Clear(); //라운드 시작 시 보상 초기화
        _gainedEggs.Clear();  //라운드 시작 시 알 초기화
        _isRewardsFinalized = false; //라운드 시작 시 지급 가드 초기화

        var user = Manager.Save.CurrentData.UserData; // 예약 초기화에 유저 필요
        int maxEgg = Manager.Game.Config.MaxEggAmount; // 최대 알 수 필요
        _rewardReservation.ResetFromUser(user, maxEgg); // 라운드 리셋 시 예약 상태 초기화
    }
    protected virtual void GameOver()
    {
        _isPlaying = false;
        GainMoneyByScore();

        Manager.Mini.UpdateMiniGameResult(Score); //미니게임 결과 저장

        FinalizeAndPayoutRewards(); //한 판 끝날 때 즉시 지급 + 표시 누적

        OnGameOver?.Invoke(); 
    }
    private void Update()
    {
        if (!_isPlaying) return;

        _playSecond += Time.deltaTime; //필요없으면 지우기
    }
    // ===== 점수 =====
    protected void AddScore(int amount)
    {
        if (!_isPlaying) return;

        _score += amount;
    }

    // ===== 아이템 =====
    protected void GainMoneyByScore()
    { 
        GainItem(RewardType.Coin, _score);
    }
    protected void GainItem(RewardType type, int amount)
    {
        if (_gainedItems.ContainsKey(type))
        {
            _gainedItems[type] += amount;
        }
        else
        {
            _gainedItems[type] = amount;
        }

        Debug.Log($"[획득] {type.ToString()} + {amount}");
    }
    protected void GainEgg(EggData egg) //알 보상 누적용(미니게임에서 알 보상 뽑으면 이걸 호출)
    {
        if (egg == null) return; //null 방어
        _gainedEggs.Add(egg);
    }
    private void FinalizeAndPayoutRewards() //지급(세이브 반영) + 표시 누적(미니게임매니저)
    {
        if (_isRewardsFinalized) return; //중복 방지
        _isRewardsFinalized = true;

        List<RewardData> rewards = BuildRewardList();
        if (rewards.Count <= 0) return;

        Manager.Item.GiveMiniGameRewards(rewards); //지급+저장(큐 적재 X)
        Manager.Mini.AccumulateRewards(rewards); //메인씬 팝업용 누적
    }
    private List<RewardData> BuildRewardList() //현재 라운드 보상 -> RewardData 변환
    {
        List<RewardData> rewards = new();

        foreach (var pair in _gainedItems)
        {
            if (pair.Key == RewardType.None) continue;
            if (pair.Value == 0) continue;
            rewards.Add(RewardData.CreateItem(pair.Key, pair.Value));
        }

        for (int i = 0; i < _gainedEggs.Count; i++)
        {
            rewards.Add(RewardData.CreateEgg(_gainedEggs[i]));//알은 개별로
        }

        return rewards;
    }
    // ===== 종료 =====
    protected void FinishGame()
    {
        Manager.Mini.EndMiniGame(); //여기서는 팝업 표시만(보상 리스트 전달 X)
    }
}
