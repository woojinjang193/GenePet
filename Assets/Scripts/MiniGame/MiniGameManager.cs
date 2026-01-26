using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : Singleton<MiniGameManager>
{
    [Header("미니게임 성격 효과 테이블")]
    [SerializeField] private MiniGamePersonalityEffectSO[] _effectTables;

    [Header("미니게임 비용(점프, 리듬, 핀볼)")]
    [SerializeField] private int[] _miniGameCosts;
    public int[] MiniGameCosts => _miniGameCosts;

    public MiniGame CurMiniGame { get; private set; }
    public PetSaveData CurPet { get; private set; }

    private readonly Dictionary<RewardType, int> _sessionItemSums = new(); //연속 플레이 보상 합산(아이템)
    private readonly List<EggData> _sessionEggs = new(); //연속 플레이 보상 리스트(알은 개별 표시)

    protected override void Awake()
    {
        base.Awake();
    }
    public void StartMiniGame(PetSaveData pet, int index) //플레이어 데이터 세팅, 씬 이동
    {
        if (pet == null)
        {
            Debug.LogError("펫정보 없음");
            return;
        }
        if (pet.GrowthStage == GrowthStatus.Egg)
        {
            Manager.Game.ShowPopup("It's still an egg");
            return;
        }

        _sessionItemSums.Clear(); //새 미니게임 시작 시 누적 보상 초기화
        _sessionEggs.Clear(); //새 미니게임 시작 시 누적 보상 초기화

        CurPet = pet; //펫정보 저장

        int enumCount = Enum.GetValues(typeof(MiniGame)).Length;

        if (index < 0 || index >= enumCount - 1)
        {
            Debug.LogError("잘못된 미니게임 인덱스");
            return;
        }

        if (!CanPlayMiniGame()) return;
        CurMiniGame = (MiniGame)index; 

        switch (CurMiniGame) //씬 이동
        {
            case MiniGame.Jump: SceneManager.LoadScene("JumpGameScene"); break;
            case MiniGame.Rythm: SceneManager.LoadScene("RythmGameScene"); break;
            case MiniGame.Pinball: SceneManager.LoadScene("PinballGameScene"); break;
        }
    }
    //============ 메인씬으로 돌아감 ==================
    public void EndMiniGame()
    {
        FlushRewardsToPopup(); //메인씬으로 넘어갈 때만 팝업 표시큐에 적재

        CurPet = null;
        CurMiniGame = MiniGame.Null;

        SceneManager.LoadScene("InGameScene");
    }
    public void AccumulateRewards(List<RewardData> rewards) //한 판 끝날 때 보상표시큐 누적만
    {
        if (rewards == null || rewards.Count == 0) return;

        for (int i = 0; i < rewards.Count; i++)
        {
            RewardData reward = rewards[i];
            if (reward == null) continue;

            if (reward.Category == RewardCategory.Egg)
            {
                if (reward.Egg != null) _sessionEggs.Add(reward.Egg);
                continue;
            }

            if (reward.RewardType == RewardType.None) continue;

            if (_sessionItemSums.ContainsKey(reward.RewardType)) //같은 아이템이 이미 있으면 
            {
                _sessionItemSums[reward.RewardType] += reward.Amount; //숫자만 더해줌 
            } 
            else _sessionItemSums[reward.RewardType] = reward.Amount; //처음 얻는거면 추가
        }
    }
    private void FlushRewardsToPopup() //누적된 보상을 "큐에만" 넣고 팝업 트리거
    {
        if (_sessionItemSums.Count == 0 && _sessionEggs.Count == 0) return;

        List<RewardData> list = new();

        foreach (var pair in _sessionItemSums)
        {
            list.Add(RewardData.CreateItem(pair.Key, pair.Value)); //아이템은 합산 1개로
        }

        for (int i = 0; i < _sessionEggs.Count; i++)
        {
            list.Add(RewardData.CreateEgg(_sessionEggs[i])); //알은 개별로
        }

        if (Manager.Item != null) Manager.Item.EnqueuePopupOnly(list); //표시 큐 적재 + 팝업 오픈

        _sessionItemSums.Clear();
        _sessionEggs.Clear();
    }
    // =========================== 결과 저장 유틸 ============================
    public void UpdateMiniGameResult(int score) 
    {
        if (CurMiniGame == MiniGame.Null) return; // Null이면 저장 안 함

        var user = Manager.Save.CurrentData.UserData; // 유저 데이터 참조
        if (user == null) return;

        int needLen = (int)MiniGame.Null; // Null 제외 길이(3)

        if (user.MiniGameResults == null || user.MiniGameResults.Length < needLen) //구세이브/누락 방어
        {
            MiniGameData[] old = user.MiniGameResults; //기존 값 보존용
            user.MiniGameResults = new MiniGameData[needLen]; //새 배열 생성

            int copyLen = (old == null) ? 0 : Mathf.Min(old.Length, needLen); //복사 길이 계산
            for (int i = 0; i < copyLen; i++) user.MiniGameResults[i] = old[i]; //기존 기록 유지 복사

            for (int i = 0; i < needLen; i++) //null 칸만 초기화
            {
                if (user.MiniGameResults[i] == null) user.MiniGameResults[i] = new MiniGameData(); //빈칸 채움
            }
        }

        int idx = (int)CurMiniGame; // enum 인덱스

        if (idx < 0 || idx >= user.MiniGameResults.Length) return; // 방어

        if (user.MiniGameResults[idx] == null) user.MiniGameResults[idx] = new MiniGameData(); // null 방어

        user.MiniGameResults[idx].PlayCount += 1; // 플레이 횟수 증가
        user.MiniGameResults[idx].BestScore = Mathf.Max(user.MiniGameResults[idx].BestScore, score); // 최고점 갱신

        CurPet.GrowthExp += 10;
    }
    public MiniGamePersonalityEffectSO GetEffectTable()
    {
        if (CurMiniGame == MiniGame.Null) 
        { 
            Debug.LogWarning("미니게임타입 이상함");
            return null;
        }

        for (int i = 0; i < _effectTables.Length; i++)
        {
            if (_effectTables[i].miniGame == CurMiniGame)
                return _effectTables[i];
        }
        return null; // 효과 없는 미니게임
    }
    // ====== 현재 유저가 받을 수 있는 보상풀 계산해서 반환================
    public List<LevelReward> GetAvailableRewardPool(LevelReward[] rewards, RewardReservation reservation)
    {
        if (rewards == null || rewards.Length == 0) return new List<LevelReward>(); //null/빈 배열 방어

        var user = Manager.Save.CurrentData.UserData;
        int maxEgg = Manager.Game.Config.MaxEggAmount;
        var pool = MiniGameRewardPicker.BuildAvailableRewards(rewards, user, maxEgg, reservation);
        return pool;
    }
    public int GetBestScore(MiniGame game) //미니게임 최고점수를 반환
    {
        if (game == MiniGame.Null) return 0;
        var user = Manager.Save.CurrentData.UserData;
        if (user == null || user.MiniGameResults == null) return 0;

        int idx = (int)game;
        if (idx < 0 || idx >= user.MiniGameResults.Length) return 0;
        if (user.MiniGameResults[idx] == null) return 0;

        return user.MiniGameResults[idx].BestScore;
    }
    //==== 에너지 사용 가능 여부======
    public bool CanPlayMiniGame()
    {
        int index = (int)CurMiniGame; //미니게임 인덱스
        int cost = _miniGameCosts[index]; //미니게임 가격

        if(Manager.Save == null)
        {
            Manager.Game.ShowPopup("Something Went Wrong. Try Later..");
            return false;
        }

        if (Manager.Save.CurrentData.UserData.Energy < cost) //비용이 없으면
        {
            Manager.Game.ShowPopup("You Don't Have Enough Energy");
            return false;
        }

        Manager.Save.CurrentData.UserData.Energy -= cost; //에너지 감소
        return true;
    }
}
