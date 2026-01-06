using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGameUIManager : MonoBehaviour
{
    [Header("점프게임 매니저")]
    [SerializeField] private JumpMiniGame _jumpGame;

    [Header("게임오버 패널")]
    [SerializeField] private JumpGameOverPanel _gameoverPanel;

    private void Awake()
    {
        _jumpGame.OnGameOver += OnGameOver;
    }
    private void OnDestroy()
    {
        _jumpGame.OnGameOver -= OnGameOver;
    }
    private void OnGameOver()
    {
        //최고점수 받아오기 추가
        int curScore = _jumpGame.Score;
        _gameoverPanel.Open(curScore, 10);
    }
}
