using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : Singleton<MiniGameManager>
{
    [Header("미니게임 성격 효과 테이블")]
    [SerializeField] private MiniGamePersonalityEffectSO[] _effectTables;

    public MiniGame CurMiniGame { get; private set; }
    public PetSaveData CurPet { get; private set; }

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

        CurPet = pet; //펫정보 저장

        int enumCount = Enum.GetValues(typeof(MiniGame)).Length;

        if (index < 0 || index >= enumCount - 1)
        {
            Debug.LogError("잘못된 미니게임 인덱스");
            return;
        }

        CurMiniGame = (MiniGame)index; 

        switch (CurMiniGame) //씬 이동
        {
            case MiniGame.Jump: SceneManager.LoadScene("JumpGameScene"); break;
            case MiniGame.Rythm: SceneManager.LoadScene("RythmGameScene"); break;
            case MiniGame.Pinball: SceneManager.LoadScene("PinballGameScene"); break;
        }
    }
    public void EndMiniGame(List<RewardData> rewards, int score)
    {
        if (rewards != null && rewards.Count > 0)
        {
            Manager.Item.GiveMiniGameRewards(rewards); //실제 지급 요청
        }

        Manager.Save.SaveGame(); //저장

        CurPet = null;
        CurMiniGame = MiniGame.Null;

        SceneManager.LoadScene("InGameScene"); 
    }
    public void UpdateMiniGameResult(int score) // 결과 저장 유틸
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

        Manager.Save.SaveGame(); //저장
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
    public List<LevelReward> GetAvailableRewardPool(LevelReward[] rewards)
    {
        if (rewards == null || rewards.Length == 0) return new List<LevelReward>(); //null/빈 배열 방어

        var user = Manager.Save.CurrentData.UserData;
        int maxEgg = Manager.Game.Config.MaxEggAmount;
        var pool = MiniGameRewardPicker.BuildAvailableRewards(rewards, user, maxEgg);
        return pool;
    }
    public int GetBestScore(MiniGame game) //미니게임 최고점수를 반환
    {
        if (game != MiniGame.Null) return 0;
        if (Manager.Save.CurrentData.UserData.MiniGameResults == null) return 0;
        int bestScore = 0;
        int minigameIndex = (int)game;

        bestScore = Manager.Save.CurrentData.UserData.MiniGameResults[(int)CurMiniGame].BestScore;
        return bestScore;
    }
}
