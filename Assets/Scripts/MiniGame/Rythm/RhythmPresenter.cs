using UnityEngine;
using System.Collections.Generic;
public class RhythmPresenter : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource; // 소리 재생용
    [SerializeField] private AudioClip _tickClip;      // 박자 소리
    [SerializeField] private RythmPatternSO _pattern;  // 리듬 패턴
    [SerializeField] private float _bpm = 120f;        // 현재 BPM
    [SerializeField] private int _measureCount = 1;    // 사용할 마디 수

    private List<double> _noteTimes; // 노트 타이밍(초, double)

    public void Play()
    {
        Debug.Log("재생");

        if (_audioSource == null) //추가됨!!!
        {
            Debug.LogError("_audioSource가 null"); //추가됨!!!
            return; //추가됨!!!
        }

        if (_tickClip == null) //추가됨!!!
        {
            Debug.LogError("_tickClip이 null"); //추가됨!!!
            return; //추가됨!!!
        }

        _audioSource.clip = _tickClip;

        // 패턴을 시간 배열로 변환
        _noteTimes = new List<double>(RythmTimingCalculator.ConvertToTime(_pattern, _bpm, _measureCount)); //리스트 복사

        double dspStart = AudioSettings.dspTime + 0.15; // 시작 딜레이

        for (int i = 0; i < _noteTimes.Count; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>(); //추가됨!!! // 노트용 오디오소스 생성
            src.clip = _tickClip;                                     //추가됨!!! // 재생할 클립 지정
            src.playOnAwake = false;                                  //추가됨!!!
            src.PlayScheduled(dspStart + _noteTimes[i]);              //수정됨!!! // 각자 다른 소스로 예약
        }

    }
}
