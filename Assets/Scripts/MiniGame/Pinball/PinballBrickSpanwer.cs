using System.Collections.Generic;
using UnityEngine;

public class PinballBrickSpanwer : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PinballGameManager _pinballManager;
    [SerializeField] private PinballVisualManager _pinballVisual;

    [Header("컬러0")]
    [SerializeField] private Color _color0;
    [Header("컬러1")]
    [SerializeField] private Color _color1;
    [Header("컬러2")]
    [SerializeField] private Color _color2;
    [Header("컬러3")]
    [SerializeField] private Color _color3;

    [Header("블록 부모")]
    [SerializeField] private Transform _brickParents;

    [Header("기본 블록 점수")]
    [SerializeField] private int _normalBrickScore = 5;
    [Header("기본 아이템 블록 점수")]
    [SerializeField] private int _normalItemBrickScore = 3;
    [Header("컬러 블록 점수")]
    [SerializeField] private int _colorBrickScore = 5;

    private List<PinballBrick> _allBricks = new List<PinballBrick>(); //전체 브릭 (세팅용. 제거하면서 아이템 할당함)
    private List<PinballBrick> _spawnedBricks = new(); //이벤트 해제용

    private GameObject _spawnedMap;

    //==내부 변수==
    private int[] _colorBrickCount = { 0, 0, 0 };
    private int _normalHP = 1;
    private int _normalItemHP = 1;
    private int _colorHP = 1;

    public void StartSettingBricks(PinballGamePresetSO preset)
    {
        if (preset == null) return;
        if(preset.BrickMaps.Length  == 0) return;

        for (int i = 0; i < _colorBrickCount.Length; i++) _colorBrickCount[i] = 0; //_colorBrickCount 배열 초기화

        UnregisterAll(); // 이전 레벨 정리

        SetHP(preset); //HP 세팅 

        int rand = Random.Range(0, preset.BrickMaps.Length); // 랜덤으로 맵 뽑음

        _spawnedMap = Manager.Pool.Get(preset.BrickMaps[rand], _brickParents.position, _brickParents); // 풀로 맵 소환
        _spawnedMap.transform.localPosition = Vector3.zero; //부모 기준 위치 정리
        _spawnedMap.transform.localRotation = Quaternion.identity; //회전 정리
        _spawnedMap.transform.localScale = Vector3.one; //스케일 정리

        PinballBrick[] bricks = _spawnedMap.GetComponentsInChildren<PinballBrick>(true);
        _spawnedBricks.AddRange(bricks); //아이템 세팅용 브릭 리스트

        foreach (PinballBrick brick in bricks) //브릭을 리스트에 넣어줌
        {
            _allBricks.Add(brick);
            _pinballManager.RegisterBrick(brick); //매니저에게 브릭 등록시킴
            _pinballVisual.RegisterBrick(brick); // 연출매니저한테 등록
        }

        SetItemBricks(preset);
        SetNormalItemBricks(preset);
        SetNormalBricks();
    }
    public void UnregisterAll()
    {
        for (int i = 0; i < _spawnedBricks.Count; i++)
        {
            var brick = _spawnedBricks[i];
            if (brick == null) continue;

            _pinballManager.UnregisterBrick(brick);
            _pinballVisual.UnregisterBrick(brick);
        }

        _spawnedBricks.Clear();
        _allBricks.Clear();

        if (_spawnedMap != null)
        {
            Manager.Pool.Release(_spawnedMap);//풀 반환
            _spawnedMap = null;
        }
    }

    // =================블록 세팅 ========================
    private void SetItemBricks(PinballGamePresetSO preset)  //컬러블록 처리
    {
        int colorBrickNum = Random.Range(preset.MinColorBrickAmount, preset.MaXColorBrickAmount + 1); //컬러브릭 개수 뽑음
        ColorCountSet(colorBrickNum); //각 색 블록 개수 정함

        var pool = Manager.Mini.GetAvailableRewardPool(preset.ColorBrickItems, _pinballManager.RewardReservation); //필터된 보상 풀

        if (pool == null || pool.Count == 0) { Debug.LogError("풀 비었음"); return; }

        for (int i = 0; i < colorBrickNum; i++)
        {
            if (_allBricks.Count <= 0) break; //남은 블록 없으면 종료

            int rand = Random.Range(0, _allBricks.Count); //전체 리스트에서 랜덤 인덱스 뽑음

            BrickColor color = GetColor();

            //아이템 뽑기
            LevelReward item = MiniGameRewardPicker.GetRandomReward(pool, _pinballManager.RewardReservation);

            if (item.RewardType == RewardType.None) { Debug.LogError("풀 확인해야함"); return; }

            _allBricks[rand].Init(GetBrickData(color, item));
            _allBricks.RemoveAt(rand);// 중복방지로 리스트에서 지움
        }

    }
    private void SetNormalItemBricks(PinballGamePresetSO preset)  //일반 아이템 블록 처리
    {
        int normalItemBrickNum = Random.Range(preset.MinNormalItemBrickAmount, preset.MaXNormalItemBrickAmount + 1); //노멀 아이템 브릭 개수 뽑음
        var pool = Manager.Mini.GetAvailableRewardPool(preset.NormalBrickItems, _pinballManager.RewardReservation);

        if (pool == null || pool.Count == 0) { Debug.LogError("풀 비었음"); return; }

        for (int i = 0; i < normalItemBrickNum; i++)
        {
            if (_allBricks.Count <= 0) break; //남은 블록 없으면 종료

            int rand = Random.Range(0, _allBricks.Count);

            LevelReward item = MiniGameRewardPicker.GetRandomReward(pool, _pinballManager.RewardReservation);

            if (item.RewardType == RewardType.None) { Debug.LogError("풀 확인해야함"); return; }

            _allBricks[rand].Init(GetBrickData(BrickColor.None, item)); //노멀 블록은 색없음
            _allBricks.RemoveAt(rand);
        }

    }
    private void SetNormalBricks()
    {
        BrickData data = new BrickData();

        LevelReward reward = new LevelReward(); //리워드 설정
        reward.RewardType = RewardType.None;
        reward.Amount = 0;
        reward.Weight = 0f;

        data.ColorName = BrickColor.None;
        data.Color = _color0;
        data.Reward = reward;
        data.Score = _normalBrickScore;

        for (int i = 0; i < _allBricks.Count;i++)
        {
            _allBricks[i].Init(data);
        }
    }
    // ================ 유틸 ========================
    private BrickData GetBrickData(BrickColor colorType, LevelReward item)
    {
        BrickData data = new BrickData();

        switch(colorType) //색 넣기
        {
            case BrickColor.None: data.ColorName = BrickColor.None; data.Color = _color0; break;
            case BrickColor.one: data.ColorName = BrickColor.one; data.Color = _color1; break;
            case BrickColor.two: data.ColorName = BrickColor.two; data.Color = _color2; break;
            case BrickColor.three: data.ColorName = BrickColor.three; data.Color = _color3; break;
        }
        data.Reward = item;

        if (colorType != BrickColor.None)
        {
            data.Score = _colorBrickScore;
            data.HP = _colorHP; //색 벽돌 HP 추가
        }
        else
        {
            data.Score = _normalItemBrickScore;

            if(item.RewardType != RewardType.None)
            {
                data.HP = _normalItemHP; // 노멀 아이템블록 HP 추가
            }
            else
            {
                data.HP = _normalHP; // 노멀 블록 HP 추가
            }
        }

        return data;
    }
    private void ColorCountSet(int colorBrickNum) //색별 숫자 정하기
    {
        int devision = colorBrickNum / 3;
        int remainder = colorBrickNum % 3;

        for (int i = 0; i < _colorBrickCount.Length; i++) //색 배분
        {
            _colorBrickCount[i] = devision;
        }

        for (int i = 0; i < remainder; i++) //나머지 색은 랜덤으로 할당
        {
            int rand = Random.Range(0, _colorBrickCount.Length);
            _colorBrickCount[rand]++;
        }
    }
    private BrickColor GetColor()
    {
        if (_colorBrickCount[0] > 0) { _colorBrickCount[0]--; return BrickColor.one; }
        if (_colorBrickCount[1] > 0) { _colorBrickCount[1]--; return BrickColor.two; }
        if (_colorBrickCount[2] > 0) { _colorBrickCount[2]--; return BrickColor.three; }

        return BrickColor.one; // 남은 게 없을 때 안전장치
    }
    private void SetHP(PinballGamePresetSO preset)
    {
        _normalHP = preset.NormalHP;
        _normalItemHP = preset.NormalItemHP;
        _colorHP = preset.ColorHP;
    }
}
