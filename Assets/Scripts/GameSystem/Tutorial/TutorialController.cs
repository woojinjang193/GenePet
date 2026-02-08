using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Serializable]
    private class TutorialMapping //시작키와 튜토리얼을 묶어서 저장
    {
        public TutorialTriggerKey Key;  // 시작 신호 키
        public TutorialChunk Chunk;  // 실행할 튜토리얼 덩어리
    }

    [Header("TriggerKey > Chunk 매핑")]
    [SerializeField] private List<TutorialMapping> _mappings = new(); //인스펙터에서 트리거별 덩어리 연결

    private readonly Dictionary<TutorialTriggerKey, TutorialChunk> _map = new(); //빠른 조회용 캐시
    private bool _isRunning = false; //실행 중이면 들어오는 신호를 무시하기 위한 플래그
    public bool IsRunning => _isRunning;
    private TutorialChunk _runningChunk; //현재 실행 중인 튜토리얼

    private void Awake()
    {
        BuildMap(); // 인스펙터 리스트를 딕셔너리로 변환
    }

    private void OnDestroy()
    {
        UnbindRunningChunk(); //씬 종료/파괴 시 이벤트 누수 방지
    }

    // 외부에서 호출용: 튜토리얼 시작 요청
    public void TryStartTutorial(TutorialTriggerKey key)
    {
        Debug.Log("튜토리얼 시작 요청");
        if (key == TutorialTriggerKey.None) return;

        Debug.Log($"튜토리얼 :{_isRunning}");
        if (_isRunning) return;
        
        if (!_map.TryGetValue(key, out var chunk) || chunk == null) { Debug.Log("값없음"); return; } //매핑 없으면 무시
        Debug.Log("튜토리얼 시작");
        _isRunning = true; 
        _runningChunk = chunk;
        _runningChunk.OnChunkCompleted += HandleChunkCompleted; //튜토리얼 종료 구독
        _runningChunk.StartChunk(); //덩어리 시작
    }

    // 튜토리얼 종료 시
    private void HandleChunkCompleted(TutorialChunk chunk)
    {
        UnbindRunningChunk();

        _isRunning = false;
        _runningChunk = null;
    }

    // 인스펙터 매핑 리스트를 딕셔너리로 만듬
    private void BuildMap()
    {
        _map.Clear();

        for (int i = 0; i < _mappings.Count; i++)
        {
            var m = _mappings[i];
            if (m == null) continue;
            if (m.Key == TutorialTriggerKey.None) continue; // None은 매핑 안함
            if (m.Chunk == null) continue;

            _map[m.Key] = m.Chunk; // 중복 키면 덮어씀
        }
    }
    // 실행 중인 튜토리얼에 걸어둔 이벤트를 해제
    private void UnbindRunningChunk()
    {
        if (_runningChunk == null) return;
        _runningChunk.OnChunkCompleted -= HandleChunkCompleted;
    }
}
