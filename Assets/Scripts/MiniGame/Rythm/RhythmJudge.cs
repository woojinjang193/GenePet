using UnityEngine; 

public enum JudgeResult
{
    Perfect,
    Good,
    Miss
}

public class RhythmJudge : MonoBehaviour
{
    [Header("판정 기준 (박 비율)")]
    public float perfectBeatWindow = 0.1f; // +-0.1박
    public float goodBeatWindow = 0.25f;   // +-0.25박

    public JudgeResult Judge(float inputTime, float targetTime, float beatDuration)  // 실제 입력 시간, 정답 시간, 현재 BPM의 1박 길이
    {
        float diffTime = Mathf.Abs(inputTime - targetTime); // 시간 차이 계산
        float diffBeat = diffTime / beatDuration; // 박 단위 오차로 변환

        if (diffBeat <= perfectBeatWindow) // 퍼펙트 판정
            return JudgeResult.Perfect;

        if (diffBeat <= goodBeatWindow) // 굿 판정
            return JudgeResult.Good;

        return JudgeResult.Miss; // 미스
    }
}
