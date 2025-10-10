using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void CreateRandomPet()
    {
        //int have = Manager.Save.CurrentData.UserData.HavePetList.Count;
        //int max = Manager.Save.CurrentData.UserData.MaxPetAmount;
        //
        //if (max <= have)
        //{
        //    Debug.LogWarning($"소지 가능 펫 수 초과. 현재 펫 수: {have}, 최대 펫 수: {max}.");
        //    return;
        //}

        PetSaveData newPet = new PetSaveData();

        newPet.ID = Guid.NewGuid().ToString();
        newPet.DisplayName = "";
        newPet.Seed = UnityEngine.Random.Range(0, 999999);

        newPet.Genes.Body.DominantId = Manager.Gene.GetRandomBodySO().ID;
        newPet.Genes.Body.RecessiveId = Manager.Gene.GetRandomBodySO().ID;

        newPet.Genes.Color.DominantId = Manager.Gene.GetRandomColorSO().ID;
        newPet.Genes.Color.RecessiveId = Manager.Gene.GetRandomColorSO().ID;

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

        Manager.Save.AddNewPet(newPet);
    }
}