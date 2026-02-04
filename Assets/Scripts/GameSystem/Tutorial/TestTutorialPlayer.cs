using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTutorialPlayer : MonoBehaviour
{
    [SerializeField] private TutorialTriggerKey _key;
    [SerializeField] private TutorialController _controller;
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Play);
    }
    private void Play()
    {
        Debug.Log("버튼눌림 ");
        _controller.TryStartTutorial(_key);
    }
}
