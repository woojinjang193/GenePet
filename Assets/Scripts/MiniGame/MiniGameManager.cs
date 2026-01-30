using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class MiniGameManager : Singleton<MiniGameManager>, IConfirmRequester
{
    [Header("미니게임 성격 효과 테이블")]
    [SerializeField] private MiniGamePersonalityEffectSO[] _effectTables;

    [Header("미니게임 비용(점프, 리듬, 핀볼)")]
    [SerializeField] private int[] _miniGameCosts;
    public int[] MiniGameCosts => _miniGameCosts;

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
        if (pet.GrowthStage == GrowthStatus.Egg)
        {
            Manager.Game.ShowPopup("It's still an egg");
            return;
        }

        CurPet = pet; //펫정보 저장

        if (_miniGameCosts == null || index < 0 || index >= _miniGameCosts.Length) // 비용 배열 기준으로 검사
        {
            Debug.LogError("잘못된 미니게임 인덱스");
            return;
        }

        CurMiniGame = (MiniGame)index; // 먼저 현재 미니게임 설정

        if (pet.IsLeft) return; //떠난펫일땐 리턴

        if (!Manager.Mini.CanPlayMiniGame(out int cost)) return; //비용검사

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
        CurPet = null;
        CurMiniGame = MiniGame.Null;

        Manager.Pool.Clear(); // 풀 비워줌
        SceneManager.LoadScene("InGameScene");
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

        //펫 스텟 증가
        CurPet.GrowthExp += Manager.Game.Config.MiniGameEXP;
        CurPet.Happiness += Manager.Game.Config.MiniGameHappiness;
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
    public bool CanPlayMiniGame(out int cost)
    {
        cost = 0; // out은 모든 경로에서 할당돼야 함

        int index = (int)CurMiniGame; //미니게임 인덱스

        if (_miniGameCosts == null || index < 0 || index >= _miniGameCosts.Length) // 인덱스 방어
        {
            Debug.LogError("미니게임 비용 배열 인덱스 오류");
            return false;
        }

        cost = _miniGameCosts[index]; //미니게임 가격

        if(Manager.Save == null)
        {
            Manager.Game.ShowPopup("Something Went Wrong. Please Restart Game.."); //TODO: 로컬라이제이션
            return false;
        }

        if (Manager.Save.CurrentData.UserData.Energy < cost) //비용이 없으면
        {
            Manager.Game.ShowConfirmMessage("Asking_MoveToShopForEnergy", 0, this);
            //Manager.Game.ShowPopup("You Don't Have Enough Energy"); //TODO: 로컬라이제이션

            return false;
        }
        return true;
    }

    public void Confirmed(int requestNum)
    {
        if(requestNum == 0)
        {
            if (SceneManager.GetActiveScene().name == "InGameScene") //인게임 씬이면
            {
                Manager.Game.OpenUiPanel(UIPanel.Shop);
            }
            else   //인게임 씬 아니면
            {
                var shop = FindObjectOfType<MoneyAmountShower>(); //샵에 붙은 컴포넌트 찾아봄
                if (shop != null) // 찾으면
                {
                    shop.gameObject.SetActive(true);
                }
                else// 못찾으면 메인씬으로 감
                {
                    Manager.Game.ReserveMainSceneUI(UIPanel.Shop);
                    EndMiniGame();
                }
            }
        }
    }

    public void Canceled(int requestNum)
    {

    }
}
