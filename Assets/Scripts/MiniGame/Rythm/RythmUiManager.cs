using System.Collections;
using TMPro;
using UnityEngine;

public class RythmUiManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmScoring _scoring;

    [Header("플레이어 목숨")]
    [SerializeField] private GameObject[] _playerHearts;

    [Header("게임오버 판넬")]
    [SerializeField] private GameObject _gameOverPanel;

    private void Awake()
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
    
    
    public void GameOverPanelOn()
    {
        _gameOverPanel.SetActive(true);
    }
}
