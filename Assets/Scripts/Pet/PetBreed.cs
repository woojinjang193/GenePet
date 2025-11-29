using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PetBreed : MonoBehaviour
{
    [SerializeField] private Sprite _common;
    [SerializeField] private Sprite _rare;
    [SerializeField] private Sprite _epic;
    [SerializeField] private Sprite _legendary;

    private RarityType _finalRarity = RarityType.Common;
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

        CombinePart(PartType.Color, myPet.Genes.Color, islandPet.Genes.Color, baby.Genes.Color);

        baby.Genes.PartColors.BodyColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.ArmColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.FeetColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.PatternColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        baby.Genes.PartColors.EarColorId = Choose(baby.Genes.Color.DominantId, baby.Genes.Color.RecessiveId);
        //egg.Genes.PartColors.BlushColorId = Choose(egg.Genes.Color.DominantId, egg.Genes.Color.RecessiveId);

        switch (_finalRarity)
        {
            case RarityType.Legendary:
                egg.Image = _legendary;
                break;
            case RarityType.Epic:
                egg.Image = _epic;
                break;
            case RarityType.Rare:
                egg.Image = _rare;
                break;
            default:
                egg.Image = _common;
                break;
        }
        return egg;
    }

    private void CombinePart(PartType type, GenePair myPet, GenePair islandPet, GenePair baby)
    {
        string fatherGene;
        string motherGene;

        //유전자 가위 여부 확인
        if (myPet.IsDominantCut) fatherGene = myPet.RecessiveId;
        else if (myPet.IsRecessiveCut) fatherGene = myPet.DominantId;
        else fatherGene = Choose(myPet.DominantId, myPet.RecessiveId);

        if (islandPet.IsDominantCut) motherGene = islandPet.RecessiveId;
        else if (islandPet.IsRecessiveCut) motherGene = islandPet.DominantId;
        else motherGene = Choose(islandPet.DominantId, islandPet.RecessiveId);

        RarityType curRarity = Manager.Gene.CheckRarity(type, fatherGene, motherGene);

        //최고등급만 저장
        if (curRarity > _finalRarity)
        {
            _finalRarity = curRarity;
        }

        if (UnityEngine.Random.value < 0.5)
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