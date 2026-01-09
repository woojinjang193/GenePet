using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBase : MonoBehaviour
{
    protected PetSaveData _pet; // 플레이 중인 펫 데이터
    protected int _score;  // 점수
    public int Score => _score;

    protected float _playSecond;

    protected Dictionary<RewardType, int> _gainedItems = new(); // 획득 아이템 누적
    protected bool _canHaveEgg;
    protected bool _isPlaying;
    protected bool _isGameOver;

    protected MiniGameContext _effectContext; // 미니게임 효과 컨텍스트

    public event Action OnGameOver;
    public event Action OnGameStart;

    protected virtual void Start()
    {
        _pet = Manager.Mini.CurPet;
        _gainedItems.Clear();    // 보상 기록 초기화

        _canHaveEgg = Manager.Save.CurrentData.UserData.EggList.Count < Manager.Game.Config.MaxEggAmount; //추가 알 획득 가능한 상태인지

        MiniGamePersonalityEffectSO table = Manager.Mini.GetEffectTable(); // 미니게임별 테이블

        //성격 가져오기
        string personalityID = _pet.Genes.Personality.DominantId;
        PersonalitySO personalitySO = Manager.Gene.GetPartSOByID<PersonalitySO>(PartType.Personality, personalityID);
        PersonalityType petsonality = personalitySO.Personality;

        float happiness01 = _pet.Happiness / 100;
        _effectContext = MiniGameEffectApplier.Apply(table, petsonality, happiness01); //성격 적용
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
    }
    protected virtual void GameOver()
    {
        _isPlaying = false;
        GainMoneyByScore();
        OnGameOver?.Invoke();
    }
    private void Update()
    {
        if (!_isPlaying) return;

        _playSecond += Time.deltaTime;
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

    // ===== 종료 =====
    protected void FinishGame()
    {
        if(_gainedItems.Count > 0 ) //보상 있을때 
        {
            List<RewardData> rewards = new();
            Debug.Log($"보상 목록:");
            foreach (var pair in _gainedItems)
            {
                rewards.Add(RewardData.CreateItem(pair.Key, pair.Value));
                Debug.Log($"아이템: {pair.Key}, 개수: {pair.Value}");
            }

            Manager.Mini.EndMiniGame(rewards, _score);
        }
        else //보상 없을때
        {
            Manager.Mini.EndMiniGame(null, _score);
        }
    }
}
