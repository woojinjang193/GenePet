using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : Singleton<MiniGameManager>
{
    public PetSaveData CurPet { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }
    public void StartMiniGame(PetSaveData pet, MiniGame miniGame) //플레이어 데이터 세팅, 씬 이동
    {
        if (pet == null)
        {
            Debug.LogError("펫정보 없음");
            return;
        }

        CurPet = pet; //펫정보 저장

        switch (miniGame) //씬 이동
        {
            case MiniGame.Jump: SceneManager.LoadScene("JumpScene"); break;
            case MiniGame.Rythm: SceneManager.LoadScene("RythmScene"); break;
        }
    }
    public void EndMiniGame(List<RewardData> rewards, int score)
    {
        Manager.Item.GiveRewards(rewards); //실제 지급 요청

        CurPet = null;
        SceneManager.LoadScene("InGameScene"); 
    }
}
