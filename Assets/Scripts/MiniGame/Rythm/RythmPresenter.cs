using System;
using System.Collections.Generic;
using UnityEngine;

public class RythmPresenter : MonoBehaviour
{
    // 샘플 비트가 "언제 울릴지(DSP)"를 외부에 알려줌
    public event Action<double, float> OnSampleBeatScheduled; // (beatDsp, beatDurationSec)

    // 샘플 비트를 "기준 DSP(start)" + "비트 오프셋(Time)"로 예약 재생한다.
    // AudioSettings.dspTime를 여기서 새로 잡으면 BGM과 기준이 달라져 박자 틀어짐.
    public void PlaySample(double dspStart, RythmPatternSO pattern, RythmLevelPresetSO preset)
    {
        List<RythmBeatTime> beats = RythmTimingCalculator.ConvertToTime(pattern, preset.BPM, preset.MeasureCount);
        float beatDuration = 60f / preset.BPM;

        foreach (var beat in beats)
        {
            double beatDsp = dspStart + beat.Time;

            AudioSource src = gameObject.AddComponent<AudioSource>(); // TODO: 풀링 추천
            src.playOnAwake = false;
            src.clip = preset.NormalBeatClip;

            // "레벨/턴 기준 start"에 맞춰 예약
            src.PlayScheduled(beatDsp);

            // 재생이 끝나면 제거 (풀링이면 제거 안 함)
            Destroy(src, (float)(beat.Time + src.clip.length + 0.1));

            OnSampleBeatScheduled?.Invoke(beatDsp, beatDuration);
        }
    }
}
