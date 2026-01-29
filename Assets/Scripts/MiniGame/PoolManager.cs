using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    // 프리팹별 풀 저장용 딕셔너리
    private Dictionary<GameObject, Stack<GameObject>> _pool = new();

    // 생성된 오브젝트 > 원본 프리팹 매핑
    private Dictionary<GameObject, GameObject> _originMap = new();
    protected override void Awake()
    {
        base.Awake();
    }
    // ===================== 가져오기 =====================
    public GameObject Get(GameObject prefab, Vector3 position, Transform parent = null) 
    {
        if (!_pool.ContainsKey(prefab)) // 해당 프리팹 풀 없으면
        {
            _pool[prefab] = new Stack<GameObject>(); // 새 스택 생성
        }

        GameObject obj;

        if (_pool[prefab].Count > 0) // 풀에 사용 가능한 오브젝트 있으면
        {
            obj = _pool[prefab].Pop(); // 하나 꺼냄
            obj.SetActive(true); // 활성화
        }
        else // 없으면
        {
            obj = Instantiate(prefab); // 새로 생성
            _originMap[obj] = prefab;   // 원본 프리팹 기록
        }

        // ---------- 부모/트랜스폼 초기화 ----------
        bool isUI = obj.transform is RectTransform;   // UI(RectTransform)인지 판별
        obj.transform.SetParent(parent, !isUI);  //UI면 worldPositionStays=false, 월드면 true

        if (isUI)// UI는 로컬 기준으로 초기화
        {
            obj.transform.localPosition = Vector3.zero;    //  UI 로컬 위치 초기화
            obj.transform.localRotation = Quaternion.identity;   //  UI 로컬 회전 초기화
            obj.transform.localScale = Vector3.one;    //  UI 스케일 초기화(커지는 문제 방지)
        }
        else // 월드 오브젝트는 월드 위치로 세팅
        {
            obj.transform.position = position;  // 월드 위치 설정
        }

        return obj;
    }

    // ===================== 반환 =====================
    public void Release(GameObject obj)
    {
        if (!_originMap.ContainsKey(obj)) // 풀에서 만든 오브젝트 아니면
        {
            Destroy(obj); // 그냥 제거
            return;
        }

        GameObject prefab = _originMap[obj]; // 원본 프리팹 가져오기

        obj.SetActive(false); // 비활성화
        obj.transform.SetParent(transform); // 풀 매니저 밑으로 정리
        obj.transform.localScale = Vector3.one; //풀 보관 중 스케일 정리(다음 사용 시 튐 방지)
        _pool[prefab].Push(obj); // 풀에 다시 넣기
    }

    // ===================== 전체 정리 =====================
    public void Clear()
    {
        foreach (var stack in _pool.Values) // 모든 풀 순회
        {
            foreach (var obj in stack) // 풀 안 오브젝트 순회
            {
                Destroy(obj); // 제거
            }
        }

        _pool.Clear(); // 풀 초기화
        _originMap.Clear(); // 매핑 초기화
    }
}
