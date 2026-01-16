using System.Collections.Generic;

public struct RythmBeatTime // 비트 타이밍 정보
{
    public double Time; // 패턴 시작 기준 시간(초, dsp 오프셋)
    public int MeasureIndex; // 패턴 안에서의 마디 인덱스
    public int BeatIndex; // 패턴 안에서의 박 인덱스(0~3)
}

public static class RythmTimingCalculator // 리듬 정보를 시간으로 변환
{
    public static double GetBeatDuration(float bpm) // 1박 길이(초)
    {
        return 60.0 / bpm; // 60/BPM
    }

    public static double GetBeatLength(RythmType type) // 리듬 타입을 "박 단위" 길이로
    {
        switch (type) // 타입 분기
        {
            case RythmType.Quarter: return 1.0; // 4분음표 = 1박
            case RythmType.Eighth: return 0.5; // 8분음표 = 0.5박
            case RythmType.Triplet: return 1.0 / 3.0; // 셋잇단 = 1/3박
        }
        return 0.0; // 알 수 없으면 0
    }

    public static List<RythmBeatTime> ConvertToTime(RythmPatternSO pattern, float bpm, int measureCount) // 패턴 >시간 배열
    {
        List<RythmBeatTime> result = new(); // 결과 리스트
        double beatDuration = GetBeatDuration(bpm); // 1박 시간(초)
        double currentBeat = 0.0; // 누적 박 수
        double maxBeat = measureCount * 4.0; // 허용 최대 박(4/4 기준)

        foreach (var info in pattern.RythmList) // 앞에서부터
        {
            if (currentBeat >= maxBeat) break; // 마디 초과면 중단

            if (!info.isRest) // 쉼표 아니면
            {
                int measureIndex = (int)(currentBeat / 4.0); // 몇 번째 마디
                int beatIndex = (int)(currentBeat % 4.0); // 마디 안 박자

                result.Add(new RythmBeatTime // 타이밍 추가
                {
                    Time = currentBeat * beatDuration, // 패턴 시작 기준 시간(초)
                    MeasureIndex = measureIndex, // 마디 인덱스
                    BeatIndex = beatIndex // 박 인덱스
                });
            }

            currentBeat += GetBeatLength(info.Type); // 박 누적
        }

        return result; // 반환
    }
}
