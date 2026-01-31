using UnityEngine; 

public enum JudgeResult
{
    Perfect,
    Good,
    Miss
}

public class RythmJudge : MonoBehaviour
{
    [Header("판정 기준 (박 비율)")]
    public float perfectBeatWindow = 0.1f; // +-0.1박
    public float goodBeatWindow = 0.25f;   // +-0.25박

    public JudgeResult Judge(float inputTimeSec, float targetTimeSec, float beatDurationSec)  // 실제 입력 시간, 정답 시간, 현재 BPM의 1박 길이
    {
        float diff = inputTimeSec - targetTimeSec; // 정답 대비 입력 시간차(초)

        float perfectWindow = perfectBeatWindow * beatDurationSec; // Perfect 허용 범위(초)
        float goodWindow = goodBeatWindow * beatDurationSec;       // Good 허용 범위(초)

        float ad = Mathf.Abs(diff); // 절대값

        if (ad <= perfectWindow) return JudgeResult.Perfect;
        if (ad <= goodWindow) return JudgeResult.Good;
        return JudgeResult.Miss;
    }
}
