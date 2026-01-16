using UnityEngine;

public class RythmInput : MonoBehaviour // 입력 사운드 재생 담당
{
    [SerializeField] private AudioSource _src; // 입력 사운드용 오디오소스

    private AudioClip _inputNormalClip; // 일반 입력 클립
    private AudioClip _inputLastBeatClip; // 마지막 마디 마지막 박자 입력 클립

    private void Awake() // 초기화
    {
        if (_src == null) _src = GetComponent<AudioSource>(); // 있으면 가져오고
        if (_src == null) _src = gameObject.AddComponent<AudioSource>(); // 없으면 하나 추가
        _src.playOnAwake = false; // 자동재생 끔
        _src.loop = false; // 루프 끔
    }

    public void Init(RythmLevelPresetSO preset) // 레벨 프리셋으로 클립 세팅
    {
        if (preset == null) return; // 안전장치
        _inputNormalClip = preset.NormalBeatClip; // 일반 입력 클립 저장
        _inputLastBeatClip = preset.InputLastBeatClip; // 마지막박 입력 클립 저장
    }

    public void PlayInputSound(bool isLastMeasureLastBeat) // 입력했을 때
    {
        if (_src == null) return; // 안전장치
        AudioClip clip = isLastMeasureLastBeat ? _inputLastBeatClip : _inputNormalClip; // 상황에 따라 선택
        if (clip == null) return; // 클립 없으면 종료
        _src.PlayOneShot(clip); // 원샷 재생(중첩 가능)
    }
}
