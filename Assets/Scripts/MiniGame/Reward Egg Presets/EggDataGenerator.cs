using System.Collections.Generic;
using UnityEngine;

public static class EggDataGenerator
{
    // [추가] 알 생성 시 적용할 PartType 목록(GenesContainer의 GenePair만 대상으로 함)
    private static readonly PartType[] _genePartTypes =
    {
        PartType.Body, PartType.Arm, PartType.Feet, PartType.Pattern,
        PartType.Eye, PartType.Mouth, PartType.Ear, PartType.Acc,
        PartType.Blush, PartType.Wing, PartType.Tail, PartType.Whiskers,
        PartType.Color, PartType.Personality
    };

    private static Dictionary<PartType, PartInfoForEggPreset> _presetMap = new();

    //보상 알 생성
    public static EggData GenerateRewardEgg(RewardEggPresetSO preset) 
    {
        BuildPresetMap(preset); // 프리셋 빠른 조회용

        EggData egg = new EggData();

        if (egg.PetSaveData == null) egg.PetSaveData = new PetSaveData(); // PetSaveData가 null 이면 새로 만듬

        PetSaveData pet = egg.PetSaveData;

        if (pet.Genes == null) pet.Genes = new GenesContainer(); // Genes 없으면 새로 만듬

        pet.GrowthStage = GrowthStatus.Egg; //알상태 (혹시몰라서)

        // 파츠 유전자(우성/열성) 채우기
        for (int i = 0; i < _genePartTypes.Length; i++)
        {
            PartType partType = _genePartTypes[i]; //현재 파츠 타입

            string dominantId = ResolveDominantId(partType); // 우성 결정(실패 시 랜덤)
            string recessiveId = PickRandomId(partType);  // 열성은 항상 랜덤

            SetGenePair(pet.Genes, partType, dominantId, recessiveId); //GenesContainer에 반영
        }

        //파츠 색상 유전자(PartColors)는 전부 랜덤
        FillRandomPartColors(pet.Genes);

        pet.Rarity = GetEggRarity(pet.Genes); // 레어리티 추가

        //pet.EggSprite = Manager.Game.Config.EggRaritySO.GetEggSprite(pet.Rarity);

        return egg;
    }

    private static void BuildPresetMap(RewardEggPresetSO preset)
    {
        if (preset == null || preset.PartInfo == null) return;

        _presetMap.Clear(); //딕셔너리 초기화

        var arr = preset.PartInfo; //SO 에 있는 파츠 배열
        for (int i = 0; i < arr.Length; i++) //배열 순회
        {
            var info = arr[i];

            if (_presetMap.ContainsKey(info.PartType)) // 중복 PartType 방지
            {
                Debug.LogWarning($"RewardEggPreset 중복 PartType 무시: {info.PartType} (첫 항목 유지)");
                continue;
            }

            _presetMap.Add(info.PartType, info); //딕셔너리에 저장
        }
    }

    // 우성 ID 결정
    private static string ResolveDominantId(PartType partType)
    {
        if (_presetMap.TryGetValue(partType, out var info)) //프리셋 지정 파츠면
        {
            if (info.Mode == EggPresetMode.ById) // ID 지정 모드
            {
                if (string.IsNullOrEmpty(info.DominantID)) //ID 비었으면
                {
                    return PickRandomId(partType); //랜덤 뽑기
                } 

                var so = Manager.Gene.GetPartSOByID<PartBaseSO>(partType, info.DominantID); // 매니저에게 존재 확인
                if (so == null)

                {
                    return PickRandomId(partType); // 없는 ID면 랜덤
                }        

                return info.DominantID; //유효하면 그대로 사용
            }

            // 레어도 지정 모드
            string byRarity = PickRandomIdByRarity(partType, info.RarityType);
            if (string.IsNullOrEmpty(byRarity))
            {
                return PickRandomId(partType); // 레어도 뽑기 실패 시 랜덤
            } 

            return byRarity;
        }

        return PickRandomId(partType); // 프리셋에 없으면 랜덤
    }

    //랜덤파츠 ID 뽑기
    private static string PickRandomId(PartType partType)
    {
        PartBaseSO pick = Manager.Gene.GetRandomPart<PartBaseSO>(partType);
        return pick != null ? pick.ID : "";
    }
    //레어리티 기반 랜덤파츠 ID 뽑기
    private static string PickRandomIdByRarity(PartType partType, RarityType rarity)
    {
        PartBaseSO pick = Manager.Gene.GetRandomPartByRarity(partType, rarity);
        return pick != null ? pick.ID : "";
    }

    // PartType > GenePair 매핑
    private static void SetGenePair(GenesContainer genes, PartType partType, string dominantId, string recessiveId) 
    {
        if (genes == null) return;

        GenePair pair = GetGenePair(genes, partType);
        if (pair == null) return;

        pair.DominantId = string.IsNullOrEmpty(dominantId) ? "00" : dominantId; //빈값 방어
        pair.RecessiveId = string.IsNullOrEmpty(recessiveId) ? "00" : recessiveId;
    }

    // GenesContainer에서 해당 GenePair 가져오기
    private static GenePair GetGenePair(GenesContainer genes, PartType partType) 
    {
        switch (partType)
        {
            case PartType.Body: return genes.Body;
            case PartType.Arm: return genes.Arm;
            case PartType.Feet: return genes.Feet;
            case PartType.Pattern: return genes.Pattern;
            case PartType.Eye: return genes.Eye;
            case PartType.Mouth: return genes.Mouth;
            case PartType.Ear: return genes.Ear;
            case PartType.Acc: return genes.Acc;
            case PartType.Blush: return genes.Blush;
            case PartType.Wing: return genes.Wing;
            case PartType.Tail: return genes.Tail;
            case PartType.Whiskers: return genes.Whiskers;
            case PartType.Color: return genes.Color;
            case PartType.Personality: return genes.Personality;
            default: return null;
        }
    }

    // 파츠 컬러 지정
    private static void FillRandomPartColors(GenesContainer genes) 
    {
        if (genes == null) return;
        if (genes.PartColors == null) genes.PartColors = new PartColorGenes();

        // 색상은 PartType.Color에서 뽑은 ID를 사용
        genes.PartColors.BodyColorId = PickRandomId(PartType.Color);
        genes.PartColors.ArmColorId = PickRandomId(PartType.Color);
        genes.PartColors.FeetColorId = PickRandomId(PartType.Color);
        genes.PartColors.PatternColorId = PickRandomId(PartType.Color);
        genes.PartColors.EarColorId = PickRandomId(PartType.Color);
        genes.PartColors.WingColorId = PickRandomId(PartType.Color);
        genes.PartColors.TailColorId = PickRandomId(PartType.Color);
    }

    // 우성 파츠들의 최고 레어도 계산
    private static RarityType GetEggRarity(GenesContainer genes) 
    {
        if (genes == null) return RarityType.Common;

        RarityType best = RarityType.Common;

        for (int i = 0; i < _genePartTypes.Length; i++)
        {
            PartType type = _genePartTypes[i];
            GenePair pair = GetGenePair(genes, type);

            if (pair == null || string.IsNullOrEmpty(pair.DominantId) || string.IsNullOrEmpty(pair.RecessiveId)) continue;

            if (pair.DominantId == "00" || pair.RecessiveId == "00") continue; // 기본값이면 조회 스킵

            PartBaseSO DominantSO = Manager.Gene.GetPartSOByID<PartBaseSO>(type, pair.DominantId);
            PartBaseSO RecessiveSO = Manager.Gene.GetPartSOByID<PartBaseSO>(type, pair.RecessiveId);

            if (DominantSO == null || RecessiveSO == null) continue;

            if ((int)DominantSO.Rarity > (int)best) best = DominantSO.Rarity;
            if ((int)RecessiveSO.Rarity > (int)best) best = RecessiveSO.Rarity;
        }

        return best;
    }
}
