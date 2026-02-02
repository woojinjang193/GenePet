using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    [Header("튜토리얼 패널")]
    [SerializeField] private GameObject _tutorialPanel;

    [Header("버튼")]
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    private void Awake()
    {
        _openButton.onClick.AddListener(Open);
        _closeButton.onClick.AddListener(Close);
    }
    private void Open()
    {
        _tutorialPanel.SetActive(true);
    }
    private void Close()
    {
        _tutorialPanel.SetActive(false);
    }
}
