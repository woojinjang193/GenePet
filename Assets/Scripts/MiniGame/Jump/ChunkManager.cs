using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private JumpGamePlatformSpawner _platformSpawner;
    [SerializeField] private JumpGameDifficultyController _difficulty;
    [SerializeField] private JumpPlayerController _player;
    [SerializeField] private Transform _chunkRoot;

    [Header("청크 설정")]
    [SerializeField] private float _chunkHeight = 15f; //청크 높이
    [SerializeField] private int _keepChunkCount = 4; //유지하는 청크 개수
    [SerializeField] private float _spawnChunkAt = 0.7f; //청크 일정높이 도달시 청크 생성

    [Header("프리팹")]
    [SerializeField] private Chunk _chunkPrefab;

    private List<Chunk> _activeChunks = new();
    private int _currentTopChunkIndex = -1;

    private void Start()
    {
        // 초기 청크 2개 생성
        CreateNextChunk();
        //CreateNextChunk();
    }
    private void Update()
    {
        float playerY = _player.GetHeight();

        // 다음 청크 생성 조건
        float nextChunkTriggerY = (_currentTopChunkIndex * _chunkHeight) + _chunkHeight * _spawnChunkAt; //플레이어의 높이가 가장 위 청크의 시작 Y 의 _spawnChunkAt% 이상 되면 생성
        if (playerY > nextChunkTriggerY)
        {
            CreateNextChunk();
            RemoveOldChunks();
        }
    }
    private void CreateNextChunk()
    {
        _currentTopChunkIndex++;

        float startY = _currentTopChunkIndex * _chunkHeight; //청크 시작점
        float endY = startY + _chunkHeight; //청크 끝지점

        DifficultyResult difficulty = _difficulty.GetLevel(_currentTopChunkIndex);

        int level = difficulty.Level;
        bool isLastChunk = difficulty.IsLastChunkOfLevel;

        Chunk chunk = Instantiate(_chunkPrefab, _chunkRoot);
        chunk.Init(startY, endY);

        
        _platformSpawner.Spawn(chunk, level, isLastChunk);

        _activeChunks.Add(chunk);
    }
    private void RemoveOldChunks()
    {
        while (_activeChunks.Count > _keepChunkCount)
        {
            Chunk old = _activeChunks[0];
            _activeChunks.RemoveAt(0);
            Destroy(old.gameObject); // 나중에 풀링으로 교체
        }
    }
}
