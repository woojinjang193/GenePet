using System.Collections.Generic;
using UnityEngine;

public enum RythmPlayMode
{
    Sample, // 샘플 재생 모드
    Input   // 입력 모드
}

public class RythmPresenter : MonoBehaviour // 샘플 리듬 재생 담당
{
    [Header("Pool")] // 인스펙터 그룹
    [SerializeField] private int _initialPoolSize = 16; // 미리 만들어둘 소스 개수

    private readonly Queue<AudioSource> _pool = new(); // 재사용 가능한 소스 큐
    private readonly List<ActiveVoice> _active = new(); // 현재 예약/재생 중인 소스 목록

    private struct ActiveVoice // 활성 보이스 정보
    {
        public AudioSource Src; // 사용 중인 오디오소스
        public double EndDspTime; // 이 시간 지나면 풀로 반환
    }

    private void Awake() // 초기화
    {
        WarmPool(_initialPoolSize); // 풀 미리 채우기
    }

    private void WarmPool(int count) // 풀 생성
    {
        for (int i = 0; i < count; i++) // count 만큼
        {
            _pool.Enqueue(CreateSource()); // 새 소스 만들어 큐에 넣기
        }
    }

    private AudioSource CreateSource() // 오디오소스 1개 생성
    {
        AudioSource s = gameObject.AddComponent<AudioSource>(); // 컴포넌트 추가
        s.playOnAwake = false; // 자동재생 끔
        s.loop = false; // 루프 끔
        return s; // 반환
    }

    private AudioSource Rent() // 풀에서 빌리기
    {
        return _pool.Count > 0 ? _pool.Dequeue() : CreateSource(); // 있으면 꺼내고 없으면 생성
    }

    private void Return(AudioSource src) // 풀로 반환
    {
        if (src == null) return; // 안전장치
        src.Stop(); // 혹시 남아있으면 중지
        src.clip = null; // 참조 제거
        _pool.Enqueue(src); // 풀에 넣기
    }

    private void Update() // 매 프레임 정리(가벼움)
    {
        double now = AudioSettings.dspTime; // 현재 dsp 시간

        for (int i = _active.Count - 1; i >= 0; i--) // 뒤에서부터 순회(삭제 안전)
        {
            if (now >= _active[i].EndDspTime) // 끝난 시간 지나면
            {
                Return(_active[i].Src); // 풀로 반환
                _active.RemoveAt(i); // 목록에서 제거
            }
        }
    }

    public void PlaySample(double patternStartDspTime, RythmPatternSO pattern, RythmLevelPresetSO preset) // 핵심: 한 기준(dspStart)로 예약
    {
        if (pattern == null) return; // 안전장치
        if (preset == null) return; // 안전장치
        if (preset.NormalBeatClip == null) return; // 비트 클립 없으면 종료

        List<RythmBeatTime> beats = RythmTimingCalculator.ConvertToTime(pattern, preset.BPM, preset.MeasureCount); // 패턴 >시간 변환
        AudioClip clip = preset.NormalBeatClip; // 샘플 재생에 쓸 클립

        for (int i = 0; i < beats.Count; i++) // 모든 비트 순회
        {
            AudioSource src = Rent(); // 소스 하나 빌림
            src.clip = clip; // 클립 지정
            double playAt = patternStartDspTime + beats[i].Time; // "패턴 시작" 기준으로 예약 시간 계산
            src.PlayScheduled(playAt); // 정확한 dsp 시간에 재생 예약

            _active.Add(new ActiveVoice // 반환 타이밍 등록
            {
                Src = src, // 소스 저장
                EndDspTime = playAt + clip.length + 0.05 // 클립 끝난 뒤 약간 여유
            });
        }
    }
}
