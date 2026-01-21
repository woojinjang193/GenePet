using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PinballBrickSpanwer : MonoBehaviour
{
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

    private List<PinballBrick> _allBricks = new List<PinballBrick>(); //전체 브릭

    //==내부 변수==
    private int[] _colorBrickCount = { 0, 0, 0 };
    public void StartSettingBricks(PinballGameLevelPresetSO preset)
    {
        if (preset == null) return;

        _allBricks.Clear(); //리스트 클리어

        GameObject brickMap = Instantiate(preset.BrickMap, _brickParents); //프리팹 소환

        PinballBrick[] bricks = brickMap.GetComponentsInChildren<PinballBrick>(true);

        foreach(PinballBrick brick in bricks) //브릭을 리스트에 넣어줌
        {
            _allBricks.Add(brick);
        }

        SetItemBricks(preset);
        SetNormalItemBricks(preset);
        SetNormalBricks();
    }

    // =================블록 세팅 ========================
    private void SetItemBricks(PinballGameLevelPresetSO preset)  //컬러블록 처리
    {
        int colorBrickNum = Random.Range(preset.MinColorBrickAmount, preset.MaXColorBrickAmount + 1); //컬러브릭 개수 뽑음
        ColorCountSet(colorBrickNum); //각 색 블록 개수 정함

        for (int i = 0; i < colorBrickNum; i++)
        {
            if (_allBricks.Count <= 0) return; //남은 블록 없으면 종료

            int rand = Random.Range(0, _allBricks.Count); //전체 리스트에서 랜덤 인덱스 뽑음

            BrickColor color = GetColor();
            LevelReward item = MiniGameRewardPicker.GetRewardByWeight(preset.ColorBrickItems);

            _allBricks[rand].Init(GetBrickData(color, item));
            _allBricks.RemoveAt(rand);// 중복방지로 리스트에서 지움
        }

    }
    private void SetNormalItemBricks(PinballGameLevelPresetSO preset)  //일반 아이템 블록 처리
    {
        int normalItemBrickNum = Random.Range(preset.MinNormalItemBrickAmount, preset.MaXNormalItemBrickAmount + 1); //노멀 아이템 브릭 개수 뽑음

        for (int i = 0; i < normalItemBrickNum; i++)
        {
            if (_allBricks.Count <= 0) return; //남은 블록 없으면 종료

            int rand = Random.Range(0, _allBricks.Count);
            LevelReward item = MiniGameRewardPicker.GetRewardByWeight(preset.NormalBrickItems);

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
        }
        else
        {
            data.Score = _normalItemBrickScore;
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
            _colorBrickCount[i]++;
        }
    }
    private BrickColor GetColor()
    {
        if (_colorBrickCount[0] > 0) { _colorBrickCount[0]--; return BrickColor.one; }
        if (_colorBrickCount[1] > 0) { _colorBrickCount[1]--; return BrickColor.two; }
        if (_colorBrickCount[2] > 0) { _colorBrickCount[2]--; return BrickColor.three; }

        return BrickColor.one; // 남은 게 없을 때 안전장치
    }
}
