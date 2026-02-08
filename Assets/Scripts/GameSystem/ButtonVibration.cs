using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVibration : MonoBehaviour
{
    [Header("진동세기")]
    [SerializeField] [Range(1,255)]private int _amplitude;
    [Header("길이(초)")]
    [SerializeField] private float _second;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }
    private void OnClicked() //초를 ms 로 변환해서 호출
    {
        Vibration.Vibrate(_second, _amplitude);
    }
}
