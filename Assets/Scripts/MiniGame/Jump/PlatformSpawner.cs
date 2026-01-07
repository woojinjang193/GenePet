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
    [SerializeField] private float _minGapY;
    [SerializeField] private float _maxGapY;

    [Header("시작 여유 Y ")]
    [SerializeField] private float _startSpawnY;

    [Header("청크당 발판 수")]
    [SerializeField] private int _minPlatformCount;
    [SerializeField] private int _maxPlatformCount;

    [Header("청크당 아이템수")]
    [SerializeField] private int _minItemCount;
    [SerializeField] private int _maxItemCount;

    [Header("마지막 발판 색")]
    [SerializeField] private Color _lastPlatformColor = Color.yellow; //테스트용

    [Header("난이도 상승하는 청크 단위")]
    [SerializeField] private int _difficultyStep;
    [Header("최고 난이도 레벨")]
    [SerializeField] private int _maxLevel;

    private int _prevChunkLastPlatformLane;
    private float _prevChunkLastPlatformY;

    public void Spawn(Chunk chunk)
    {
        int count = Random.Range(_minPlatformCount, _maxPlatformCount + 1); // 생성할 발판 개수

        bool hasPrev = _prevChunkLastPlatformY > 0f; // 이전 청크 존재 여부

        // 이전 청크 마지막 발판부터 시작
        // 첫 청크는 기본 시작점
        float curY = hasPrev? _prevChunkLastPlatformY : chunk.StartY + _startSpawnY;            
        int prevLane = hasPrev? _prevChunkLastPlatformLane: -1; //이전 레인 없으면 -1

        GameObject lastPlatform = null; // 마지막 발판 참조

        for (int i = 0; i < count; i++)
        {
            float gapY = Random.Range(_minGapY, _maxGapY); // 다음 발판 간격

            if (curY + gapY >= chunk.EndY) // 청크 끝 초과 시 종료
                break;

            curY += gapY; // Y 이동

            int lane = GetNextLane(prevLane); // 레인 결정
            float x = _lanesX[lane];

            Vector3 pos = new Vector3(x, curY, 0f);

            GameObject platform = Instantiate(_platformPrefab, pos, Quaternion.identity, chunk.transform);

            lastPlatform = platform; // 마지막 발판
            prevLane = lane;
        }

        if (lastPlatform != null) // 마지막 발판 처리
        {
            SetLastPlatformColor(lastPlatform);   // 색 변경
            SaveLastPlatform(curY, prevLane); // 다음 청크용 저장
        }
    }

    private int GetNextLane(int prev) //다음 발판위치 정하는 함수
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

    private void SaveLastPlatform(float y, int lane)
    {
        _prevChunkLastPlatformY = y;        // 마지막 발판 Y 저장 //수정됨!!!!
        _prevChunkLastPlatformLane = lane;  // 마지막 발판 레인 저장 //수정됨!!!!
    }
    private void SetLastPlatformColor(GameObject platform)
    {
        var sr = platform.GetComponent<SpriteRenderer>(); // 스프라이트 찾기 //수정됨!!!!
        if (sr == null) return;

        sr.color = _lastPlatformColor; // 마지막 발판 색 적용 //수정됨!!!!
    }
}
