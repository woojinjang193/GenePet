using System.Collections.Generic;
using UnityEngine;

public class JumpGamePlatformSpawner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private MiniGameBase _game; // 예약 상태 접근용

    [Header("프리팹")]
    [SerializeField] private GameObject _platformPrefab;

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
    public void Spawn(Chunk chunk, JumpGameDifficultyPreset preset, bool isLastChunk)
    {
        //if (_presets == null || _presets.Length == 0)
        //{
        //    Debug.LogError("Difficulty presets가 비어있음");
        //    return;
        //}

        _platforms.Clear(); //리스트 클리어

        //int level = Mathf.Min(difficultyLevel, _presets.Length - 1); //레벨 인덱스 
        //JumpGameDifficultyPreset preset = _presets[level];

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
            GameObject platform = Manager.Pool.Get(prefab, pos, chunk.transform);

            _platforms.Add(platform); //리스트에 추가
            lastPlatform = platform;
            prevLane = lane;
        }

        if (lastPlatform == null) return;

        SetLastPlatformColor(lastPlatform);
        SaveLastPlatform(curY, prevLane);

        PlaceItems(preset, isLastChunk);
    }

    // ===== 청크 비활성화 시 플랫폼 반환 =====
    public void ReleasePlatforms(Chunk chunk)
    {
        if (chunk == null) return;

        int count = chunk.transform.childCount; // 자식 개수를 미리 가져옴 (순회 중 변해도 안전)

        for (int i = count - 1; i >= 2; i--) //뒤에서 부터 순회 (벽 2개 제외)
        {
            Transform platformTr = chunk.transform.GetChild(i);
            GameObject platform = platformTr.gameObject;

            for (int j = 0; j < platformTr.childCount; j++) //플렛폼 모든 자식 끔
            {
                platformTr.GetChild(j).gameObject.SetActive(false);
            }
            // 플랫폼만 풀로 반환
            Manager.Pool.Release(platform);
        }
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
        var reservation = _game.RewardReservation;

        // 마지막 청크면 마지막 플랫폼에만 클리어 보상 1개 먼저 배치하고, 그 플랫폼은 일반 보상 대상에서 제외
        if (isLastChunk && _platforms.Count > 0) // 마지막 청크 처리
        {
            var clearPool = Manager.Mini.GetAvailableRewardPool(preset.LevelClearRewards, reservation); // 예약 반영 풀
            LevelReward clearPicked = MiniGameRewardPicker.GetRandomReward(clearPool, reservation);

            if (clearPicked.RewardType != RewardType.None)
            {
                PlaceItemOnPlatform(_platforms[^1], clearPicked); // 뽑은 보상 그대로 배치
                _platforms.RemoveAt(_platforms.Count - 1); // 마지막 플랫폼 제거
            }
        }

        if (_platforms.Count == 0) return;

        var levelRewardsPool = Manager.Mini.GetAvailableRewardPool(preset.LevelRewards, reservation); //예약 반영 풀 생성
        if (_platforms.Count == 0 || levelRewardsPool.Count == 0) return; //레벨 리워드 0이면 리턴

        int maxItemCount = Mathf.Min(preset.MaxItemCount, _platforms.Count); //최대 아이템 개수 (발판수보다 아이템수가 많으면 발판수로 설정)
        int totalItemCount = Random.Range(preset.MinItemCount, maxItemCount + 1); // 총 아이템 개수 랜덤 결정

        for (int i = 0; i < totalItemCount; i++) //소환할 아이템 수만큼 반복
        {
            if (_platforms.Count == 0) break;
            if (levelRewardsPool.Count == 0) break;

            LevelReward picked = MiniGameRewardPicker.GetRandomReward(levelRewardsPool, reservation);
            if (picked.RewardType == RewardType.None) break;

            int idx = Random.Range(0, _platforms.Count); //랜덤 플렛폼 선택
            PlaceItemOnPlatform(_platforms[idx], picked); //아이템 초기화
            _platforms.RemoveAt(idx);
        }
    }

    private void PlaceItemOnPlatform(GameObject platform, LevelReward reward) //배치된 아이템 초기화
    {
        if (reward.RewardType == RewardType.None) return;

        ItemForMiniGame item = platform.GetComponentInChildren<ItemForMiniGame>(true); //컴포넌트 찾기
        if (item == null) { Debug.Log($"{platform.name} 플렛폼에 아이템 없음"); return; }
        
        item.Init(reward.RewardType, reward.Amount); //아이템 초기화
    }

    // ======================= 유틸 ======================
    private GameObject GetRandomPlatformPrefab(JumpGameDifficultyPreset preset)
    {
        if (preset.PlatformPrefabs != null && preset.PlatformPrefabs.Length > 0)
            return preset.PlatformPrefabs[Random.Range(0, preset.PlatformPrefabs.Length)];

        return _platformPrefab;
    }

    public void ResetPrevChunkData()
    {
        _prevChunkLastPlatformY = 0f;   //이전 게임 높이 초기화
        _prevChunkLastPlatformLane = 0; //이전 게임 레인 초기화
    }
}
