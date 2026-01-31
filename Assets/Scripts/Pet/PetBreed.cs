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
        string fatherGene;
        string motherGene;

        // 확정 우선 처리 (확정이면 무조건 그 유전자 사용)
        if (myPet.IsDoGuaranteed) fatherGene = myPet.DominantId;          // 우성 확정
        else if (myPet.IsReGuaranteed) fatherGene = myPet.RecessiveId;    // 열성 확정
        else if (myPet.IsDominantCut) fatherGene = myPet.RecessiveId;     // 우성 잘림
        else if (myPet.IsRecessiveCut) fatherGene = myPet.DominantId;     // 열성 잘림
        else fatherGene = Choose(myPet.DominantId, myPet.RecessiveId);    // 랜덤 선택

        // 수정: "확정" 우선 처리 (확정이면 무조건 그 유전자 사용)
        if (islandPet.IsDoGuaranteed) motherGene = islandPet.DominantId;        // 우성 확정
        else if (islandPet.IsReGuaranteed) motherGene = islandPet.RecessiveId;  // 열성 확정
        else if (islandPet.IsDominantCut) motherGene = islandPet.RecessiveId;   // 우성 잘림
        else if (islandPet.IsRecessiveCut) motherGene = islandPet.DominantId;   // 열성 잘림
        else motherGene = Choose(islandPet.DominantId, islandPet.RecessiveId);  // 랜덤 선택

        RarityType curRarity = Manager.Gene.CheckRarity(type, fatherGene, motherGene);

        if (curRarity > _finalRarity) _finalRarity = curRarity;

        // 추가: 확정된 유전자가 있으면 baby 우성(Dominant)에 배치
        bool fatherForced = myPet.IsDoGuaranteed || myPet.IsReGuaranteed;
        bool motherForced = islandPet.IsDoGuaranteed || islandPet.IsReGuaranteed;

        if (fatherForced ^ motherForced) // 둘 중 하나만 확정일 때만 확정 유전자=우성을 보장
        {
            baby.DominantId = fatherForced ? fatherGene : motherGene;
            baby.RecessiveId = fatherForced ? motherGene : fatherGene;
            return;
        }

        // 둘 다 확정 아니거나/ 둘 다 확정이면 기존 랜덤 배치
        if (UnityEngine.Random.value < 0.5f)
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