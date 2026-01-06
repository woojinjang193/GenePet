using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBase : MonoBehaviour
{
    protected PetSaveData _pet; // 플레이 중인 펫 데이터
    protected int _score;  // 점수
    protected float _playSecond;

    protected Dictionary<RewardType, int> _gainedItems = new(); // 획득 아이템 누적
    protected bool _isPlaying;

    protected virtual void Start()
    {
        //_pet = Manager.Mini.CurPet;  //TODO:테스트 끝나면 활성화 해야함
        _isPlaying = true; //TODO: 테스트용 나중에 지워야함

        _score = 0;              // 점수 초기화
        _playSecond = 0;
        _gainedItems.Clear();    // 보상 기록 초기화
    }
    protected void GameStart()
    {
        _isPlaying = true;
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
    protected void GainItem(RewardType type, int amount)
    {
        if (!_isPlaying) return;

        if (_gainedItems.ContainsKey(type))
            _gainedItems[type] += amount;
        else
            _gainedItems[type] = amount;
    }

    // ===== 종료 =====
    protected void FinishGame()
    {
        _isPlaying = false;

        List<RewardData> rewards = new();
        Debug.Log($"보상 목록:");
        foreach (var pair in _gainedItems)
        {
            rewards.Add(RewardData.CreateItem(pair.Key, pair.Value));
            Debug.Log($"아이템: {pair.Key}, 개수: {pair.Value}");
        }

        //TODO:테스트 끝나면 활성화 해야함
        //Manager.Mini.EndMiniGame(rewards, _score);
    }
}
