using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Database")]
    [SerializeField] private AudioDataBase _database;

    [Header("Mixer")]
    [SerializeField] private AudioMixer _audioMixer;         // 오디오 믹서
    [SerializeField] private AudioMixerGroup _bgmGroup;      // BGM 믹서 그룹
    [SerializeField] private AudioMixerGroup _sfxGroup;      // SFX 믹서 그룹

    [Header("Sources")]
    [SerializeField] private AudioSource _bgmSource;         // BGM 전용 오디오 소스
    [SerializeField] private int _sfxPoolSize = 10;           // SFX 풀 크기

    // ===== 음소거 상태 =====
    private bool _isBGMMuted = false;        // BGM 음소거 상태
    private bool _isSFXMuted = false;        // SFX 음소거 상태

    private float _savedBGMVolume = 0f;      //음소거 전 BGM 볼륨(dB)
    private float _savedSFXVolume = 0f;      //음소거 전 SFX 볼륨(dB)

    private List<AudioSource> _sfxSources;  // SFX 오디오 소스 풀

    public bool IsReady { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        IsReady = true; //테스트용, 지워야함
        //어드레서블 세팅

        InitBGMSource();    //초기화
        InitSFXSources();
    }
    private void InitBGMSource()
    {
        _bgmSource.loop = true;    // BGM은 루프
        _bgmSource.outputAudioMixerGroup = _bgmGroup; // BGM 믹서 연결
    }
    private void InitSFXSources()
    {
        _sfxSources = new List<AudioSource>(); // 리스트 생성

        for (int i = 0; i < _sfxPoolSize; i++) // 풀 크기만큼
        {
            AudioSource src = gameObject.AddComponent<AudioSource>(); // 소스 생성
            src.playOnAwake = false;           // 자동 재생 끔
            src.outputAudioMixerGroup = _sfxGroup; // SFX 믹서 연결
            _sfxSources.Add(src);              // 풀에 추가
        }
    }

    // =========================
    // BGM
    // =========================

    public void PlayBGM(string key)
    {
        var data = _database.Get(key);       // 데이터베이스에서 조회
        if (data == null) return;            // 없으면 종료

        if (_bgmSource.isPlaying)            // 이미 재생 중이면
            _bgmSource.Stop();               // 정지

        _bgmSource.clip = data.clip;         // 클립 할당
        _bgmSource.volume = data.baseVolume; // 기본 볼륨 적용
        _bgmSource.Play();                   // 재생
    }

    public void StopBGM()
    {
        _bgmSource.Stop();                   // BGM 정지
    }

    // =========================
    // SFX
    // =========================

    public void PlaySFX(string key)
    {
        var data = _database.Get(key);        // 데이터베이스 조회
        if (data == null) return;             // 없으면 종료

        AudioSource src = GetAvailableSFXSource(); // 사용 가능한 소스 찾기
        if (src == null)
        {
            Debug.LogWarning("오디오 너무 많이 동시 재생중");
            return;              // 없으면 재생 안 함
        }
        src.clip = data.clip;                // 클립 설정
        src.volume = data.baseVolume;         // 기본 볼륨 적용
        src.Play();                           // 재생
    }

    private AudioSource GetAvailableSFXSource() //오디오소스 빈자리 찾기
    {
        foreach (var src in _sfxSources)     // 풀 순회
        {
            if (!src.isPlaying)              // 안 쓰는 소스 찾기
                return src;                  // 반환
        }

        return null;                          // 전부 사용 중이면 null
    }

    public void SetBGMMute(bool mute)  //BGM 뮤트 세팅
    {
        if (mute)                             // 음소거 ON이면
        {
            _audioMixer.GetFloat("BGM", out _savedBGMVolume); // 현재 볼륨 저장
            _audioMixer.SetFloat("BGM", -80f);                // 완전 음소거
        }
        else                                 // 음소거 OFF이면
        {
            _audioMixer.SetFloat("BGM", _savedBGMVolume);     // 이전 볼륨 복원
        }

        _isBGMMuted = mute;                  // 상태 저장
    }

    public void SetSFXMute(bool mute)         //SFX 뮤트 세팅
    {
        if (mute)                             // 음소거 ON이면
        {
            _audioMixer.GetFloat("SFX", out _savedSFXVolume); // 현재 볼륨 저장
            _audioMixer.SetFloat("SFX", -80f);                // 완전 음소거
        }
        else                                 // 음소거 OFF이면
        {
            _audioMixer.SetFloat("SFX", _savedSFXVolume);     // 이전 볼륨 복원
        }

        _isSFXMuted = mute;                  // 상태 저장
    }

}
