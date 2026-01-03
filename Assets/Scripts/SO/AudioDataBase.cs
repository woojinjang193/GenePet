using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioDatabase")]
public class AudioDataBase : ScriptableObject
{
    [System.Serializable]
    public class AudioData
    {
        public string key;                 // 호출용 키 문자열
        public AudioClip clip;             // 오디오 클립
        [Range(0f, 1f)]
        public float baseVolume = 1f;       // 클립별 기본 볼륨 보정
    }

    [SerializeField]
    private List<AudioData> _audioList;    // 오디오 데이터 목록

    private Dictionary<string, AudioData> _cache; // 빠른 조회용 캐시

    public AudioData Get(string key)
    {
        if (_cache == null)                // 캐시가 아직 없으면
        {
            _cache = new Dictionary<string, AudioData>(); // 딕셔너리 생성
            foreach (var data in _audioList)              // 리스트 순회
            {
                if (!_cache.ContainsKey(data.key))        // 중복 키 방지
                    _cache.Add(data.key, data);            // 캐시에 등록
            }
        }

        if (_cache.TryGetValue(key, out var result))       // 키 조회
            return result;                                 // 있으면 반환

        Debug.LogWarning($"Audio key not found: {key}");   // 없으면 경고
        return null;                                       // null 반환
    }
}
