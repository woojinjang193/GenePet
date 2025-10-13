using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void CreateRandomPet()
    {
        PetSaveData newPet = new PetSaveData();
        newPet.ID = Guid.NewGuid().ToString();
        newPet.DisplayName = "";
        newPet.Seed = UnityEngine.Random.Range(0, 999999);

        newPet.Genes.Body.DominantId = Manager.Gene.GetRandomBodySO().ID;
        newPet.Genes.Body.RecessiveId = Manager.Gene.GetRandomBodySO().ID;

        newPet.Genes.Ear.DominantId = Manager.Gene.GetRandomEarSO().ID;
        newPet.Genes.Ear.RecessiveId = Manager.Gene.GetRandomEarSO().ID;

        newPet.Genes.Eye.DominantId = Manager.Gene.GetRandomEyeSO().ID;
        newPet.Genes.Eye.RecessiveId = Manager.Gene.GetRandomEyeSO().ID;

        newPet.Genes.Horn.DominantId = Manager.Gene.GetRandomHornSO().ID;
        newPet.Genes.Horn.RecessiveId = Manager.Gene.GetRandomHornSO().ID;

        newPet.Genes.Mouth.DominantId = Manager.Gene.GetRandomMouthSO().ID;
        newPet.Genes.Mouth.RecessiveId = Manager.Gene.GetRandomMouthSO().ID;

        newPet.Genes.Pattern.DominantId = Manager.Gene.GetRandomPatternSO().ID;
        newPet.Genes.Pattern.RecessiveId = Manager.Gene.GetRandomPatternSO().ID;

        newPet.Genes.Personality.DominantId = Manager.Gene.GetRandomPersonalitySO().ID;
        newPet.Genes.Personality.RecessiveId = Manager.Gene.GetRandomPersonalitySO().ID;

        newPet.Genes.Tail.DominantId = Manager.Gene.GetRandomTailSO().ID;
        newPet.Genes.Tail.RecessiveId = Manager.Gene.GetRandomTailSO().ID;

        newPet.Genes.Wing.DominantId = Manager.Gene.GetRandomWingSO().ID;
        newPet.Genes.Wing.RecessiveId = Manager.Gene.GetRandomWingSO().ID;

        newPet.Genes.PartColors.BodyColorId = Manager.Gene.GetRandomColorSO().ID;
        newPet.Genes.PartColors.PatternColorId = Manager.Gene.GetRandomColorSO().ID;
        newPet.Genes.PartColors.EarColorId = Manager.Gene.GetRandomColorSO().ID;
        newPet.Genes.PartColors.TailColorId = Manager.Gene.GetRandomColorSO().ID;
        newPet.Genes.PartColors.WingColorId = Manager.Gene.GetRandomColorSO().ID;

        Manager.Save.AddNewPet(newPet);
    }
}