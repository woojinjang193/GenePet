using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static JumpGameDifficultyPreset;

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

    private List<GameObject> _platforms = new List<GameObject>(); //소환한 발판 넣을 리스트

    //=======================청크에 발판 소환========================
    public void Spawn(Chunk chunk, int difficultyLevel, bool isLastChunk)
    {
        if (_presets == null || _presets.Length == 0)
        {
            Debug.LogError("Difficulty presets가 비어있음");
            return;
        }

        _platforms.Clear(); //리스트 클리어

        int level = Mathf.Min(difficultyLevel, _presets.Length - 1); //레벨 인덱스 
        JumpGameDifficultyPreset preset = _presets[level];

        bool hasPrev = _prevChunkLastPlatformY > 0f; // 이전 청크가 있는지 확인

        float curY = hasPrev ? _prevChunkLastPlatformY : chunk.StartY + _startSpawnY; // 이전 청크 있으면 기준으로 잡음, 없으면 청크 시작점 + 여유높이

        int prevLane = hasPrev ? _prevChunkLastPlatformLane : -1; // 마지막 포지션 없으면 -1

        GameObject lastPlatform = null;

        while (true)
        {
            float gapY = Random.Range(preset.MinGapY, _maxGapY); // 플렛폼 간격 결정

            if (curY + gapY >= chunk.EndY) break; //청크 높이보다 높으면 끝냄

            curY += gapY;

            GameObject prefab = GetRandomPlatformPrefab(preset);
            int lane = GetNextLane(prevLane, preset.DistanceBetweenPlatform); // 다음 x 포지션 

            Vector3 pos = new(_lanesX[lane], curY, 0f);
            GameObject platform = Instantiate(prefab, pos, Quaternion.identity, chunk.transform);

            _platforms.Add(platform); //리스트에 추가
            lastPlatform = platform;
            prevLane = lane;
        }

        if (lastPlatform == null) return;

        SetLastPlatformColor(lastPlatform);
        SaveLastPlatform(curY, prevLane);

        PlaceItems(preset, isLastChunk);
    }

    //==============다음 발판위치 정하는 함수======================
    private int GetNextLane(int prev, int distance) 
    {
        if (prev < 0) return Random.Range(0, _lanesX.Length);

        int maxDist = Mathf.Clamp(distance, 1, _lanesX.Length - 1);

        List<int> candidates = new();

        for (int i = 0; i < _lanesX.Length; i++)
        {
            if (i != prev && Mathf.Abs(i - prev) <= maxDist)
                candidates.Add(i);
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    //==============청크 마지막 발판 저장======================
    private void SaveLastPlatform(float y, int lane)
    {
        _prevChunkLastPlatformY = y;        // 마지막 발판 Y 저장
        _prevChunkLastPlatformLane = lane;  // 마지막 발판 레인 저장
    }

    //==============청크 마지막 타일 색 변경======================
    private void SetLastPlatformColor(GameObject platform)
    {
        var sr = platform.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        sr.color = _lastPlatformColor; // 마지막 발판 색 적용
    }

    //==============아이템 배치======================
    private void PlaceItems(JumpGameDifficultyPreset preset, bool isLastChunk)
    {
        if (_platforms.Count == 0) return; //플렛폼 없으면 리턴 

        if (isLastChunk && preset.LevelClearRewards.Length > 0) //마지막 청크 & 리워드가 0이상일때
        {
            PlaceItemOnPlatform(_platforms[^1], GetWeightedRandomReward(preset.LevelClearRewards)); //아이템 배치
            _platforms.RemoveAt(_platforms.Count - 1); //리스트에서 삭제
        }

        if (_platforms.Count == 0 || preset.LevelRewards.Length == 0) return; //레벨 리워드 0이면 리턴

        int maxItemCount = Mathf.Min(preset.MaxItemCount, _platforms.Count); //최대 아이템 개수 (발판수보다 아이템수가 많으면 발판수로 설정)
        int totalItemCount = Random.Range(preset.MinItemCount, maxItemCount + 1); // 총 아이템 개수 랜덤 결정

        for (int i = 0; i < totalItemCount; i++) //소환할 아이템 수만큼 반복
        {
            if (_platforms.Count == 0) break;

            int idx = Random.Range(0, _platforms.Count); //랜덤 플렛폼 선택
            PlaceItemOnPlatform(_platforms[idx], GetWeightedRandomReward(preset.LevelRewards)); //아이템 배치
            _platforms.RemoveAt(idx); // 플렛폼 리스트에서 삭제
        }
    }

    private void PlaceItemOnPlatform(GameObject platform, JumpGameDifficultyPreset.LevelReward reward)
    {
        ItemForMiniGame item = platform.GetComponentInChildren<ItemForMiniGame>(true); //컴포넌트 찾기
        if (item == null) { Debug.Log($"{platform.name} 플렛폼에 아이템 없음"); return; }
        
        Sprite icon = Manager.Item.ItemImages.GetItemSprite(reward.RewardType); //아이콘 받아오기
        item.Init(icon, reward.RewardType, reward.Amount); //아이템 초기화
    }

    // ======================= 유틸 ======================
    private GameObject GetRandomPlatformPrefab(JumpGameDifficultyPreset preset)
    {
        if (preset.PlatformPrefabs != null && preset.PlatformPrefabs.Length > 0)
            return preset.PlatformPrefabs[Random.Range(0, preset.PlatformPrefabs.Length)];

        return _platformPrefab;
    }

    private JumpGameDifficultyPreset.LevelReward GetWeightedRandomReward(JumpGameDifficultyPreset.LevelReward[] rewards)
    {
        float totalWeight = 0f;

        for (int i = 0; i < rewards.Length; i++)
        {
            totalWeight += rewards[i].Weight; // 각 보상의 Weight를 전부 더함
        }

        float rand = Random.Range(0f, totalWeight); //랜덤 숫자 뽑음
        float acc = 0f;

        for (int i = 0; i < rewards.Length; i++) //리워드 배열 체크
        {
            acc += rewards[i].Weight; // 각 보상의 Weight를 누적
            if (rand <= acc) //범위안에 들어오면
            {
                return rewards[i]; //보상 뽑음
            }  
        }

        return rewards[rewards.Length - 1]; // 안전장치
    }
}
