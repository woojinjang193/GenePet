using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PetBreed : MonoBehaviour
{
    public PetSaveData BreedPet(PetSaveData father, PetSaveData mother)
    {
        var egg = new PetSaveData();

        egg.ID = Guid.NewGuid().ToString();
        egg.FatherId = father.ID;
        egg.MotherId = mother.ID;

        CombinePart(father.Genes.Body, mother.Genes.Body, egg.Genes.Body);
        CombinePart(father.Genes.Arm, mother.Genes.Arm, egg.Genes.Arm);
        CombinePart(father.Genes.Feet, mother.Genes.Feet, egg.Genes.Feet);
        CombinePart(father.Genes.Pattern, mother.Genes.Pattern, egg.Genes.Pattern);
        CombinePart(father.Genes.Eye, mother.Genes.Eye, egg.Genes.Eye);
        CombinePart(father.Genes.Mouth, mother.Genes.Mouth, egg.Genes.Mouth);
        CombinePart(father.Genes.Ear, mother.Genes.Ear, egg.Genes.Ear);
        CombinePart(father.Genes.Acc, mother.Genes.Acc, egg.Genes.Acc);
        CombinePart(father.Genes.Blush, mother.Genes.Blush, egg.Genes.Blush);
        CombinePart(father.Genes.Wing, mother.Genes.Wing, egg.Genes.Wing);

        CombinePart(father.Genes.Color, mother.Genes.Color, egg.Genes.Color);

        egg.Genes.PartColors.BodyColorId = Choose(egg.Genes.Color.DominantId, egg.Genes.Color.RecessiveId);
        egg.Genes.PartColors.ArmColorId = Choose(egg.Genes.Color.DominantId, egg.Genes.Color.RecessiveId);
        egg.Genes.PartColors.FeetColorId = Choose(egg.Genes.Color.DominantId, egg.Genes.Color.RecessiveId);
        egg.Genes.PartColors.PatternColorId = Choose(egg.Genes.Color.DominantId, egg.Genes.Color.RecessiveId);
        egg.Genes.PartColors.EarColorId = Choose(egg.Genes.Color.DominantId, egg.Genes.Color.RecessiveId);
        //egg.Genes.PartColors.BlushColorId = Choose(egg.Genes.Color.DominantId, egg.Genes.Color.RecessiveId);

        return egg;
    }

    private void CombinePart(GenePair father, GenePair mother, GenePair target)
    {
        string fatherGene;
        string motherGene;

        //유전자 가위 여부 확인
        if (father.IsDominantCut) fatherGene = father.RecessiveId;
        else if (father.IsRecessiveCut) fatherGene = father.DominantId;
        else fatherGene = Choose(father.DominantId, father.RecessiveId);

        if (mother.IsDominantCut) motherGene = mother.RecessiveId;
        else if (mother.IsRecessiveCut) motherGene = mother.DominantId;
        else motherGene = Choose(mother.DominantId, mother.RecessiveId);

        if (UnityEngine.Random.value < 0.5)
        {
            target.DominantId = fatherGene;
            target.RecessiveId = motherGene;
        }
        else
        {
            target.DominantId = motherGene;
            target.RecessiveId = fatherGene;
        }
    }

    private string Choose(string father, string mother)
    {
        return(UnityEngine.Random.value < 0.5) ? father : mother;
    }
}