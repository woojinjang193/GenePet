using System.Collections.Generic;

public static class RythmTimingCalculator
{
    // BPM 기준 1박 길이 계산
    public static double GetBeatDuration(float bpm)
    {
        return 60.0 / bpm; // 1박 = 60 / BPM 초
    }

    // 리듬 타입을 박 단위 길이로 변환
    public static double GetBeatLength(RythmType type)
    {
        switch (type)
        {
            case RythmType.Quarter:
                return 1.0; // 4분음표 = 1박
            case RythmType.Eighth:
                return 0.5; // 8분음표 = 0.5박
            case RythmType.Triplet:
                return 1.0 / 3.0; // 셋잇단 = 1/3박
        }

        return 0.0;
    }

    // 패턴을 실제 시간 배열 변환
    public static List<double> ConvertToTime(RythmPatternSO pattern, float bpm, int measureCount)   // 리듬 패턴, 현재 BPM, 허용 마디 수
    {
        List<double> result = new List<double>(); // 결과 리스트
        double beatDuration = GetBeatDuration(bpm); // 1박 시간
        double currentBeat = 0.0; // 누적 박 수
        double maxBeat = measureCount * 4.0; // 마디 초과 컷 기준

        foreach (var info in pattern.RythmList) // 앞에서부터 순서대로
        {
            double beatLength = GetBeatLength(info.Type); // 이 음표의 박 길이

            if (currentBeat >= maxBeat) // 마디 초과면 중단
                break;

            if (!info.isRest) // 쉼표가 아니면
            {
                result.Add(currentBeat * beatDuration); // 입력 정답 시간 저장
            }

            currentBeat += beatLength; // 박 누적
        }

        return result; // 최종 타이밍 반환
    }
}
