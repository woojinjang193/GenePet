using System.Collections;
using TMPro;
using UnityEngine;

public class RythmUiManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmScoring _scoring;

    [Header("플레이어 목숨")]
    [SerializeField] private GameObject[] _playerHearts;

    [Header("타이틀 판넬")]
    [SerializeField] private RythmGameTitlePanel _titlePanel;

    private void Start()
    {
        
    }
    public void SetHeart(int amount) //하트 켜줌
    {
        if (amount <= 0) return;
        if (_playerHearts.Length < amount) return;

        for (int i = 0; i < _playerHearts.Length; i++) //다 끔
        {
            _playerHearts[i].gameObject.SetActive(false);
        }

        for (int i = 0;  i < amount; i++) //원하는 개수만큼 킴
        {
            _playerHearts[i].gameObject.SetActive(true);
        }
    }
    public void RemoveHeart() //하트 지움
    {
        foreach (var heart in _playerHearts)
        {
            if(heart.gameObject.activeSelf)
            {
                heart.gameObject.SetActive(false);
                break;
            }
        }
    }
    public void GameOverPanelOn(int curScore, int bestScore)
    {
        _titlePanel.UpdateScore(curScore, bestScore);
        _titlePanel.gameObject.SetActive(true);
        Manager.Item.NotifyRewardsReady();
    }
}
