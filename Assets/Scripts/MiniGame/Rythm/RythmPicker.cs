using UnityEngine;

public class RythmPicker : MonoBehaviour // 패턴 선택 담당
{
    public RythmPatternSO Pick(RythmLevelPresetSO preset) // 프리셋에서 랜덤 패턴 뽑기
    {
        if (preset == null) return null; // 안전장치
        if (preset.RythmPatternList == null) return null; // 안전장치

        int count = preset.RythmPatternList.Length; // 패턴 개수
        if (count <= 0) return null; // 패턴이 없으면 null
        int rand = Random.Range(0, count);
        return preset.RythmPatternList[rand];
    }
}
