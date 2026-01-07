using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGamePlatformSpawner : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject _platformPrefab;
    [Header("난이도 프리셋")]
    [SerializeField] private JumpGameDifficultyPreset[] _presets;

    [Header("레인 X 위치")]
    [SerializeField] private float[] _lanesX = { -2.5f, 0f, 2.5f };

    [Header("발판 최대 간격")]
    [SerializeField] private float _maxGapY;

    [Header("시작 여유 Y ")]
    [SerializeField] private float _startSpawnY;

    [Header("마지막 발판 색")]
    [SerializeField] private Color _lastPlatformColor = Color.yellow; //테스트용

    private int _prevChunkLastPlatformLane; //이전 청크의 마지막 발판 레인 위치
    private float _prevChunkLastPlatformY; // 이전 청크의 마지막 플렛폼 높이

    public void Spawn(Chunk chunk, int difficultyLevel)
    {
        if (_presets == null || _presets.Length == 0)
        {
            Debug.LogError("Difficulty presets가 비어있음");
            return;
        }

        int level = Mathf.Min(difficultyLevel, _presets.Length - 1); //레벨 인덱스 
        JumpGameDifficultyPreset preset = _presets[level];

        bool hasPrev = _prevChunkLastPlatformY > 0f; // 이전 청크가 있는지 확인

        float curY = hasPrev? _prevChunkLastPlatformY : chunk.StartY + _startSpawnY; // 이전 청크 있으면 기준으로 잡음, 없으면 청크 시작점 + 여유높이

        int prevLane = hasPrev ? _prevChunkLastPlatformLane : -1; // 마지막 포지션 없으면 -1

        GameObject lastPlatform = null;

        while (true)
        {
            float gapY = Random.Range(preset.MinGapY, _maxGapY); // 플렛폼 간격 결정

            if (curY + gapY >= chunk.EndY) break; //청크 높이보다 높으면 끝냄

            curY += gapY;

            GameObject prefab = (_presets[level].PlatformPrefabs != null && _presets[level].PlatformPrefabs.Length > 0) //프리팹이 있는지 확인
                ? _presets[level].PlatformPrefabs[Random.Range(0, _presets[level].PlatformPrefabs.Length)] // 있으면 현재 레벨 발판중 하나 소환
                : _platformPrefab; //없으면 기본 프리팹

            int lane = GetNextLane(prevLane, preset.DistanceBetweenPlatform); // 다음 x 포지션 
            float x = _lanesX[lane];

            Vector3 pos = new Vector3(x, curY, 0f);

            GameObject platform = Instantiate(prefab, pos, Quaternion.identity, chunk.transform);

            lastPlatform = platform;
            prevLane = lane;
        }

        if (lastPlatform != null)
        {
            SetLastPlatformColor(lastPlatform);
            SaveLastPlatform(curY, prevLane);
        }
    }


    private int GetNextLane(int prev, int distanceBetweenPlatform) //다음 발판위치 정하는 함수
    {
        if (prev < 0) return Random.Range(0, _lanesX.Length);

        int dist = Mathf.Clamp(distanceBetweenPlatform, 1, _lanesX.Length - 1);

        List<int> candidates = new();

        for (int i = 0; i < _lanesX.Length; i++)
        {
            if (i != prev && Mathf.Abs(i - prev) <= distanceBetweenPlatform)
            {
                candidates.Add(i);
            } 
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
    private void SaveLastPlatform(float y, int lane)
    {
        _prevChunkLastPlatformY = y;        // 마지막 발판 Y 저장
        _prevChunkLastPlatformLane = lane;  // 마지막 발판 레인 저장
    }
    private void SetLastPlatformColor(GameObject platform)
    {
        var sr = platform.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        sr.color = _lastPlatformColor; // 마지막 발판 색 적용
    }
}
