using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject _platformPrefab;

    [Header("레인 X 위치")]
    [SerializeField] private float[] _lanesX = { -2.5f, 0f, 2.5f };

    [Header("Y 간격")]
    [SerializeField] private float _minGapY = 2.5f;
    [SerializeField] private float _maxGapY = 4f;

    [Header("시작 여유 Y ")]
    [SerializeField] private float _startSpawnY = 0f;

    [Header("청크당 발판 수")]
    [SerializeField] private int _minCount = 4;
    [SerializeField] private int _maxCount = 7;

    /// <summary>
    /// 청크 범위 안에 발판 생성
    /// </summary>
    public void Spawn(Chunk chunk)
    {
        float curY = chunk.StartY + _startSpawnY; // 바닥 여유
        int count = Random.Range(_minCount, _maxCount + 1);

        int prevLane = -1;

        for (int i = 0; i < count; i++)
        {
            float gapY = Random.Range(_minGapY, _maxGapY);
            curY += gapY;

            if (curY >= chunk.EndY)
                break;

            int lane = GetNextLane(prevLane);
            float x = _lanesX[lane];

            Vector3 pos = new Vector3(x, curY, 0f);

            GameObject platform = Instantiate(_platformPrefab, pos, Quaternion.identity, chunk.transform);

            prevLane = lane;
        }
    }

    /// <summary>
    /// 이전 레인과 너무 튀지 않게 선택
    /// </summary>
    private int GetNextLane(int prev)
    {
        if (prev < 0)
            return Random.Range(0, _lanesX.Length);

        List<int> candidates = new();

        for (int i = 0; i < _lanesX.Length; i++)
        {
            if (i != prev && Mathf.Abs(i - prev) <= 1)
            {
                candidates.Add(i);
            } 
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}
