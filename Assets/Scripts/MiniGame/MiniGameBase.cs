using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBase : MonoBehaviour
{
    protected int _score;
    protected int _playSecond;

    protected Dictionary<RewardType, int> _gainedItems = new();
    protected bool _isPlaying;

    protected virtual void Start()
    {
        _score = 0;              // 점수 초기화
        _gainedItems.Clear();    // 보상 기록 초기화
    }

    protected void AddScore(int amount)
    {
        _score += amount; // 점수 증가
    }

    protected void GainItem(RewardType type, int amount)
    {
        if (_gainedItems.ContainsKey(type))
            _gainedItems[type] += amount;
        else
            _gainedItems[type] = amount;
    }

    protected void FinishGame()
    {
        List<RewardData> rewardList = new(); // 보상 리스트 생성

        foreach (var pair in _gainedItems)
        {
            rewardList.Add(RewardData.CreateItem(pair.Key, pair.Value)); // 누적 → RewardData 변환
        }

        Manager.Mini.EndMiniGame(rewardList, _score); // 매니저에 전달
    }
}
