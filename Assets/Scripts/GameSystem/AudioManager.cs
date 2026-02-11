using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("오디오 DB")]
    [SerializeField] private AudioDataBase _database;

    [Header("믹서")]
    [SerializeField] private AudioMixer _audioMixer;         // 오디오 믹서
    [SerializeField] private AudioMixerGroup _bgmGroup;      // BGM 믹서 그룹
    [SerializeField] private AudioMixerGroup _sfxGroup;      // SFX 믹서 그룹

    [Header("소스")]
    [SerializeField] private AudioSource _bgmSource;         // BGM 전용 오디오 소스
    [SerializeField] private AudioSource _exclusiveSfxSource; //동시재생 안될 SFX
    [SerializeField] private int _sfxPoolSize = 10;           // SFX 풀 크기

    // ===== 음소거 상태 =====
    // 저장 키
    private const string PREF_BGM_MUTED = "PREF_BGM_MUTED"; // BGM 뮤트 저장키
    private const string PREF_SFX_MUTED = "PREF_SFX_MUTED"; //SFX 뮤트 저장키

    private bool _isBGMMuted = false;        // BGM 음소거 상태
    private bool _isSFXMuted = false;        // SFX 음소거 상태

    private float _savedBGMVolume = 0f;      //음소거 전 BGM 볼륨(dB)
    private float _savedSFXVolume = 0f;      //음소거 전 SFX 볼륨(dB)

    private List<AudioSource> _sfxSources;  // SFX 오디오 소스 풀

    public string CurBgmKey { get; private set; }
    public bool IsReady { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        IsReady = true; //테스트용, 어드레서블 세팅 후 지워야함

        InitBGMSource();    //초기화
        InitSFXSources();

        LoadMuteStateAndApply(); //저장된 뮤트 상태 로드,즉시 적용
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
        CurBgmKey = key; //키 저장 (외부 조회용)
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
        if (_isSFXMuted) return;

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

    public void PlaySFXExclusive(string key, bool loop = false) // 동시에 재생되면 안되는 SFX 재생
    {
        if (_isSFXMuted) return;

        var data = _database.Get(key);
        if (data == null) return;

        _exclusiveSfxSource.Stop();  // 이전 사운드 즉시 끊기
        _exclusiveSfxSource.clip = data.clip;
        _exclusiveSfxSource.volume = data.baseVolume;
        _exclusiveSfxSource.loop = loop;
        _exclusiveSfxSource.Play();
    }
    public void StopSFXExclusive()     // 전용 채널 정지
    {
        _exclusiveSfxSource.Stop();
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

    public void SetBGMMute(bool mute)
    {
        _isBGMMuted = mute; // 상태 저장
        _bgmSource.mute = mute; // BGM 소스 뮤트

        PlayerPrefs.SetInt(PREF_BGM_MUTED, mute ? 1 : 0);
        PlayerPrefs.Save(); //저장
    }

    public void SetSFXMute(bool mute)
    {
        _isSFXMuted = mute; // 상태 저장

        _exclusiveSfxSource.mute = mute; // 전용 SFX도 뮤트
        for (int i = 0; i < _sfxSources.Count; i++) //풀 SFX 전부 뮤트
            _sfxSources[i].mute = mute;

        PlayerPrefs.SetInt(PREF_SFX_MUTED, mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadMuteStateAndApply() // 시작 시 뮤트 상태 복원
    {
        _isBGMMuted = PlayerPrefs.GetInt(PREF_BGM_MUTED, 0) == 1;
        _isSFXMuted = PlayerPrefs.GetInt(PREF_SFX_MUTED, 0) == 1;

        //실제 믹서에 반영
        SetBGMMute(_isBGMMuted);
        SetSFXMute(_isSFXMuted);
    }
    public bool GetBGMMuted() => _isBGMMuted; // UI에서 초기 토글 세팅용
    public bool GetSFXMuted() => _isSFXMuted; //UI에서 초기 토글 세팅용

}
