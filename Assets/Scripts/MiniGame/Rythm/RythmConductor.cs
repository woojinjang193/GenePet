using System;
using UnityEngine;

public class RythmConductor : MonoBehaviour // 마디가 바뀌는 순간을 알려준다
{
    public event Action<int, double> OnMeasureTick;

    [Header("BGM (2개 필요: 끊김없는 스케줄 교체용)")]
    [SerializeField] private AudioSource _bgmA; // 현재/다음용 1
    [SerializeField] private AudioSource _bgmB; // 현재/다음용 2

    private bool _running;
    private double _levelStartDspTime;          // 레벨 기준 0점(=BGM 시작 기준)
    private double _measureDuration;            // 1마디 길이(초)
    private int _lastProcessedMeasure = -1;     // 같은 마디 중복 틱 방지

    private AudioSource _activeBgm;             // 지금 재생 중인 소스
    private AudioSource _inactiveBgm;           // 다음 예약에 쓸 소스

    public double LevelStartDspTime => _levelStartDspTime;
    public double MeasureDuration => _measureDuration;
    public bool IsRunning => _running;

    private void Awake()
    {
        // 두 소스가 없으면 동작이 불완전하니 최소한 A라도 쓰게 처리
        if (_bgmA == null && _bgmB != null) _bgmA = _bgmB;
        if (_bgmB == null && _bgmA != null) _bgmB = _bgmA;

        // 초기 활성/비활성 지정
        _activeBgm = _bgmA;
        _inactiveBgm = _bgmB;

        // 예약 재생 안정화
        if (_bgmA != null) _bgmA.playOnAwake = false;
        if (_bgmB != null) _bgmB.playOnAwake = false;
    }

    public void StartLevel(float bpm, double dspStart)
    {
        _levelStartDspTime = dspStart;
        _measureDuration = 4.0 * (60.0 / bpm); // 4/4 기준
        _lastProcessedMeasure = -1;
        _running = true;
    }

    public void StopLevel()
    {
        _running = false;

        // BGM도 멈추고 싶으면 여기서 정지
        if (_bgmA != null) _bgmA.Stop();
        if (_bgmB != null) _bgmB.Stop();
    }

    public double GetMeasureStartDsp(int measureIndex)
    {
        return _levelStartDspTime + (measureIndex * _measureDuration);
    }

    // "현재 시점(now)" 기준으로, 다음 마디 경계의 DSP 시간을 반환
    public double GetNextMeasureBoundaryDsp(double nowDsp)
    {
        double elapsed = nowDsp - _levelStartDspTime;
        if (elapsed < 0) elapsed = 0;

        int curMeasure = (int)(elapsed / _measureDuration);
        double curStart = _levelStartDspTime + curMeasure * _measureDuration;
        return curStart + _measureDuration;
    }

    /// <summary>
    /// 끊김 없이 BGM 교체:
    /// - 현재(active)는 dspStart 시점에 정확히 종료(SetScheduledEndTime)
    /// - 다음(inactive)은 dspStart 시점에 정확히 시작(PlayScheduled)
    /// </summary>
    public void PlayBgmAt(AudioClip clip, double dspStart)
    {
        if (clip == null) return;

        // 두 소스가 모두 없으면 재생 불가
        if (_bgmA == null && _bgmB == null) return;

        // 소스 하나만 있으면 "끊김 없는 교체"는 불가(그래도 예약 재생은 함)
        if (_bgmA == _bgmB)
        {
            var s = _bgmA;
            if (s == null) return;

            s.playOnAwake = false;
            s.Stop();                 // 단일 소스는 교체 시점 전까지 유지 불가 → 바로 stop
            s.clip = clip;
            s.loop = true;
            s.PlayScheduled(dspStart);
            _activeBgm = s;
            _inactiveBgm = s;
            return;
        }

        // 초기 상태: active가 아직 재생 중이 아니면 active로 바로 예약
        if (_activeBgm == null) _activeBgm = _bgmA != null ? _bgmA : _bgmB;
        if (_inactiveBgm == null) _inactiveBgm = (_activeBgm == _bgmA) ? _bgmB : _bgmA;

        // 다음 곡은 "비활성" 소스에 세팅 후 정확히 dspStart에 시작 예약
        _inactiveBgm.Stop();          // 비활성은 예약 충돌 방지용으로 정지
        _inactiveBgm.playOnAwake = false;
        _inactiveBgm.clip = clip;
        _inactiveBgm.loop = true;
        _inactiveBgm.PlayScheduled(dspStart);

        // 현재 곡이 재생 중이면 dspStart에 정확히 끊어서 공백 없게 이어줌
        if (_activeBgm.isPlaying)
        {
            // dspStart에 정확히 종료(중간 공백/조기 stop 방지)
            _activeBgm.SetScheduledEndTime(dspStart);
        }

        // 교체: 다음 예약 소스를 active로 스왑
        var tmp = _activeBgm;
        _activeBgm = _inactiveBgm;
        _inactiveBgm = tmp;
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
