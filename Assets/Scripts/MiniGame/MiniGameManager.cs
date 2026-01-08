using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : Singleton<MiniGameManager>
{
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

        if (index < 0 || index >= enumCount)
        {
            Debug.LogError("잘못된 미니게임 인덱스");
            return;
        }

        MiniGame miniGame = (MiniGame)index; 

        switch (miniGame) //씬 이동
        {
            case MiniGame.Jump: SceneManager.LoadScene("JumpGameScene"); break;
            case MiniGame.Rythm: SceneManager.LoadScene("RythmScene"); break;
        }
    }
    public void EndMiniGame(List<RewardData> rewards, int score)
    {

        Manager.Item.GiveMiniGameRewards(rewards); //실제 지급 요청
        //스코어 처리 여기에서
        CurPet = null;
        SceneManager.LoadScene("InGameScene"); 
    }
}
