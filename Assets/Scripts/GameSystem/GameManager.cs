using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class GameManager : Singleton<GameManager>
{
    public GameConfig Config { get; private set; }
    private bool _isManagerReady = false;
    public bool IsReady { get { return _isManagerReady; } }

    protected override void Awake()
    {
        base.Awake();
        var handle = Addressables.LoadAssetAsync<GameConfig>("GameConfig");
        handle.Completed += OnConfigLoaded;
    }
    private void OnConfigLoaded(AsyncOperationHandle<GameConfig> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Config = handle.Result;
            Debug.Log("GameConfig 로드 완료");
            _isManagerReady = true;
        }
        else
        {
            Debug.LogError("GameConfig 로드 실패");
        }
    }
    public void CreateRandomPet(bool isMine)
    {
        if (isMine)
        {
            int userMaxAmount = Manager.Save.CurrentData.UserData.MaxPetAmount;
            int curAmount = Manager.Save.CurrentData.UserData.HavePetList.Count;
            int gameMaxAmount = Manager.Game.Config.MaxPetArmount;

            if (curAmount >= gameMaxAmount)
            {
                Debug.Log($"게임이 허용하는 최대 펫 수 : {gameMaxAmount}.\n현재 펫 수 {curAmount}");
                return;
            }
            if (curAmount >= userMaxAmount)
            {
                Debug.Log($"현재 유저가 키울 수 있는 최대 펫 수 : {userMaxAmount}.\n현재 펫 수 {curAmount}");
                //TODO: 여기에 슬롯 구매 바로가기 창 띄우기
                return;
            } 
        }
        PetSaveData newpet = CreateRandomPetData(isMine);
        Manager.Save.RegisterNewPet(newpet, isMine);

        PetManager petManager = FindObjectOfType<PetManager>();
        if(isMine && petManager != null)
        {
            petManager.SpawnPet(newpet);
        }
    }
    private GenePair GetPair(GenesContainer g, PartType part) // GenePair 매핑
    {
        switch (part) // 파트별 GenePair 반환
        {
            case PartType.Body: return g.Body;
            case PartType.Arm: return g.Arm;
            case PartType.Feet: return g.Feet;
            case PartType.Pattern: return g.Pattern;
            case PartType.Eye: return g.Eye;
            case PartType.Mouth: return g.Mouth;
            case PartType.Ear: return g.Ear;
            case PartType.Acc: return g.Acc;
            case PartType.Blush: return g.Blush;
            case PartType.Wing: return g.Wing;
            case PartType.Color: return g.Color;
            case PartType.Personality: return g.Personality;
        }
        return null;
    }
    private PetSaveData CreateRandomPetData(bool isMine)
    {
        PetSaveData newPet = new PetSaveData();

        newPet.ID = Guid.NewGuid().ToString();
        newPet.DisplayName = "";

        RarityType highestRarity = RarityType.Common;

        //PartType 전체 순회
        foreach (PartType part in Enum.GetValues(typeof(PartType))) // enum 전체 루프
        {
            GenePair pair = GetPair(newPet.Genes, part); // GenePair 가져오기
            if (pair == null)
            {
                Debug.LogWarning($"{part.ToString()} 파츠 없음");
                continue; // 안전 처리
            }
            
            PartBaseSO dominant = Manager.Gene.GetRandomPart<PartBaseSO>(part); // 우성 랜덤
            PartBaseSO recessive = Manager.Gene.GetRandomPart<PartBaseSO>(part); // 열성 랜덤

            if (dominant.Rarity > highestRarity) highestRarity = dominant.Rarity; // 최고 레어도 갱신
            if (recessive.Rarity > highestRarity) highestRarity = recessive.Rarity;

            pair.DominantId = dominant.ID; // 우성 유전자 ID 설정
            pair.RecessiveId = recessive.ID; // 열성 유전자 ID 설정
        }

        string dom = newPet.Genes.Color.DominantId;
        string rec = newPet.Genes.Color.RecessiveId;

        //컬러 설정
        newPet.Genes.PartColors.ArmColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.BodyColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.FeetColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.PatternColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.EarColorId = PickColorId(dom, rec);

        newPet.Rarity = highestRarity; //저장 안해도 되면 PetSaveData에서 지우기

        //알 이미지 저장
        switch (highestRarity)
        {
            case RarityType.Legendary:
                newPet.EggSprite = Config.EggRaritySO.Legendary; break;
            case RarityType.Epic:
                newPet.EggSprite = Config.EggRaritySO.Epic; break;
            case RarityType.Rare:
                newPet.EggSprite = Config.EggRaritySO.Rare; break;
            default:
                newPet.EggSprite = Config.EggRaritySO.Common; break;
        }

        return newPet;
    }

    private string PickColorId(string dominant, string recessive)
    {
        float r = UnityEngine.Random.value;
        if (r > 0.5f)
        {
            return dominant;
        }
        return recessive;
    }
}