using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
            case MiniGame.Rythm: SceneManager.LoadScene("RythmScene"); break;
        }
    }
    public void EndMiniGame(List<RewardData> rewards, int score)
    {
        if (rewards != null && rewards.Count > 0)
        {
            Manager.Item.GiveMiniGameRewards(rewards); //실제 지급 요청
        }
        //스코어 처리 여기에서
        CurPet = null;
        CurMiniGame = MiniGame.Null;

        SceneManager.LoadScene("InGameScene"); 
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
}
