using System;
using UnityEngine;

public class RythmConductor : MonoBehaviour
{
    // 마디가 바뀌는 순간을 알려준다.
    // measureIndex: 0,1,2...
    // measureStartDsp: 해당 마디의 시작 DSP 시간(정확한 기준)
    public event Action<int, double> OnMeasureTick;

    [Header("BGM")]
    [SerializeField] private AudioSource _bgmSource;

    private bool _running;
    private double _levelStartDspTime;   // 레벨 기준 0점(=BGM 시작 기준)
    private double _measureDuration;     // 1마디 길이(초)
    private int _lastProcessedMeasure = -1;

    public double LevelStartDspTime => _levelStartDspTime;
    public double MeasureDuration => _measureDuration;
    public bool IsRunning => _running;

    public void StartLevel(float bpm, double dspStart)
    {
        _levelStartDspTime = dspStart;
        _measureDuration = 4.0 * (60.0 / bpm);   // 4/4 기준
        _lastProcessedMeasure = -1;
        _running = true;
    }

    public void StopLevel()
    {
        _running = false;
    }

    public double GetMeasureStartDsp(int measureIndex)
    {
        return _levelStartDspTime + (measureIndex * _measureDuration);
    }

    // "현재 시점(now)" 기준으로, 다음 마디 경계의 DSP 시간을 반환한다.
    public double GetNextMeasureBoundaryDsp(double nowDsp)
    {
        double elapsed = nowDsp - _levelStartDspTime;
        if (elapsed < 0) elapsed = 0;

        int curMeasure = (int)(elapsed / _measureDuration);
        double curStart = _levelStartDspTime + curMeasure * _measureDuration;
        return curStart + _measureDuration;
    }

    public void PlayBgmAt(AudioClip clip, double dspStart)
    {
        if (_bgmSource == null) return;
        if (clip == null) return;

        _bgmSource.Stop();
        _bgmSource.playOnAwake = false;
        _bgmSource.clip = clip;
        _bgmSource.loop = true;

        // BGM도 "기준 DSP"에 예약
        _bgmSource.PlayScheduled(dspStart);
    }

    private void Update()
    {
        if (!_running) return;

        double now = AudioSettings.dspTime;
        double levelTime = now - _levelStartDspTime;

        // 아직 예약 시작 전이면 무시
        if (levelTime < 0) return;

        int measureIndex = (int)(levelTime / _measureDuration);

        // 같은 마디면 중복 틱 방지
        if (measureIndex == _lastProcessedMeasure) return;

        _lastProcessedMeasure = measureIndex;

        double measureStartDsp = GetMeasureStartDsp(measureIndex);
        OnMeasureTick?.Invoke(measureIndex, measureStartDsp);
    }
}
