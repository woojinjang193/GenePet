using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PetBreed : MonoBehaviour
{
    private RarityType _finalRarity = RarityType.Common;

    //============교배된 알 만들기============
    public EggData BreedPet(PetSaveData myPet, PetSaveData islandPet)
    {
        _finalRarity = RarityType.Common;

        var egg = new EggData();
        var baby = egg.PetSaveData;
        baby.ID = Guid.NewGuid().ToString();
        baby.FatherId = myPet.ID;
        baby.MotherId = islandPet.ID;

        CombinePart(PartType.Body, myPet.Genes.Body, islandPet.Genes.Body, baby.Genes.Body);
        CombinePart(PartType.Arm, myPet.Genes.Arm, islandPet.Genes.Arm, baby.Genes.Arm);
        CombinePart(PartType.Feet, myPet.Genes.Feet, islandPet.Genes.Feet, baby.Genes.Feet);
        CombinePart(PartType.Pattern, myPet.Genes.Pattern, islandPet.Genes.Pattern, baby.Genes.Pattern);
        CombinePart(PartType.Eye, myPet.Genes.Eye, islandPet.Genes.Eye, baby.Genes.Eye);
        CombinePart(PartType.Mouth, myPet.Genes.Mouth, islandPet.Genes.Mouth, baby.Genes.Mouth);
        CombinePart(PartType.Ear, myPet.Genes.Ear, islandPet.Genes.Ear, baby.Genes.Ear);
        CombinePart(PartType.Acc, myPet.Genes.Acc, islandPet.Genes.Acc, baby.Genes.Acc);
        CombinePart(PartType.Blush, myPet.Genes.Blush, islandPet.Genes.Blush, baby.Genes.Blush);
        CombinePart(PartType.Wing, myPet.Genes.Wing, islandPet.Genes.Wing, baby.Genes.Wing);
        CombinePart(PartType.Tail, myPet.Genes.Tail, islandPet.Genes.Tail, baby.Genes.Tail);
        CombinePart(PartType.Whiskers, myPet.Genes.Whiskers, islandPet.Genes.Whiskers, baby.Genes.Whiskers);

        CombinePart(PartType.Personality, myPet.Genes.Personality, islandPet.Genes.Personality, baby.Genes.Personality);

        CombinePart(PartType.Color, myPet.Genes.Color, islandPet.Genes.Color, baby.Genes.Color);

        baby.Genes.PartColors.BodyColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.ArmColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.FeetColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.PatternColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.EarColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.WingColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.TailColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);

        return egg;
    }
    // ======== 유전자 합치기======
    private void CombinePart(PartType type, GenePair myPet, GenePair islandPet, GenePair baby)
    {
        bool fatherForced = myPet.IsDoGuaranteed || myPet.IsReGuaranteed;  // 내 펫 확정 여부
        bool motherForced = islandPet.IsDoGuaranteed || islandPet.IsReGuaranteed;   // 섬펫 확정 여부

        string fatherGene;
        string motherGene;

        // 내 펫(아버지 역할)
        if (myPet.IsDoGuaranteed) fatherGene = myPet.DominantId;          // 우성 확정
        else if (myPet.IsReGuaranteed) fatherGene = myPet.RecessiveId;    // 열성 확정
        else if (myPet.IsDominantCut) fatherGene = myPet.RecessiveId;     // 우성 잘림
        else if (myPet.IsRecessiveCut) fatherGene = myPet.DominantId;     // 열성 잘림
        else fatherGene = Choose(myPet.DominantId, myPet.RecessiveId);    // 랜덤 선택

        // 섬 펫(어머니 역할)
        if (islandPet.IsDoGuaranteed) motherGene = islandPet.DominantId;        // 우성 확정
        else if (islandPet.IsReGuaranteed) motherGene = islandPet.RecessiveId;  // 열성 확정
        else if (islandPet.IsDominantCut) motherGene = islandPet.RecessiveId;   // 우성 잘림
        else if (islandPet.IsRecessiveCut) motherGene = islandPet.DominantId;   // 열성 잘림
        else motherGene = Choose(islandPet.DominantId, islandPet.RecessiveId);  // 랜덤 선택

        // 희귀도 계산은 "선택된 두 유전자"로 ---
        RarityType curRarity = Manager.Gene.CheckRarity(type, fatherGene, motherGene);
        if (curRarity > _finalRarity) _finalRarity = curRarity;

        // --- baby에 배치 --- 
        if (fatherForced && motherForced) //같은 파츠에 엄마아빠 모두 유전자 확정된경우
        {          
            if (UnityEngine.Random.value < 0.5f) // 우성 열성 랜덤
            {
                baby.DominantId = fatherGene;  
                baby.RecessiveId = motherGene;
            }
            else
            {
                baby.DominantId = motherGene;
                baby.RecessiveId = fatherGene;
            }
            return;  //조기 종료
        }

        if (fatherForced && !motherForced) //아빠만 확정
        {
            baby.DominantId = fatherGene;  // 내펫 유전자 우성
            baby.RecessiveId = motherGene;
            return;
        }

        if (!fatherForced && motherForced) // 엄마만 확정
        {
            baby.DominantId = motherGene;  // 섬펫 유전자 우성
            baby.RecessiveId = fatherGene;
            return;
        }

        if (UnityEngine.Random.value < 0.5f) // 둘 다 확정 아니면 랜덤 배치
        {
            baby.DominantId = fatherGene;
            baby.RecessiveId = motherGene;
        }
        else
        {
            baby.DominantId = motherGene;
            baby.RecessiveId = fatherGene;
        }
    }

    private string Choose(string father, string mother)
    {
        return(UnityEngine.Random.value < 0.5) ? father : mother;
    }

}