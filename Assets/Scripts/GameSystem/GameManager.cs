using System;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    //public LeftReason Reason {  get; private set; }

    public void CreateRandomPet(bool isMine)
    {
        if (isMine)
        {
            int maxAmount = Manager.Save.CurrentData.UserData.MaxPetAmount;
            int curAmount = Manager.Save.CurrentData.UserData.HavePetList.Count;

            if (curAmount >= maxAmount)
            {
                Debug.Log($"현재 유저가 키울 수 있는 최대 펫 수 : {maxAmount}.\n현재 펫 수 {curAmount}");
                //TODO: 여기에 슬롯 구매 바로가기 창 띄우기
                return;
            }
        }
        PetSaveData newpet = CreateRandomPetData();
        Manager.Save.RegisterNewPet(newpet, isMine);

        PetManager petManager = FindObjectOfType<PetManager>();
        if(petManager != null)
        {
            petManager.SpawnPet(newpet);
        }
    }
    private PetSaveData CreateRandomPetData()
    {
        PetSaveData newPet = new PetSaveData();
        newPet.DisplayName = "";
        newPet.ID = Guid.NewGuid().ToString();

        newPet.Genes.Acc.DominantId = Manager.Gene.GetRandomAccSO().ID;
        newPet.Genes.Acc.RecessiveId = Manager.Gene.GetRandomAccSO().ID;

        newPet.Genes.Arm.DominantId = Manager.Gene.GetRandomArmSO().ID;
        newPet.Genes.Arm.RecessiveId = Manager.Gene.GetRandomArmSO().ID;

        newPet.Genes.Blush.DominantId = Manager.Gene.GetRandomBlushSO().ID;
        newPet.Genes.Blush.RecessiveId = Manager.Gene.GetRandomBlushSO().ID;

        newPet.Genes.Body.DominantId = Manager.Gene.GetRandomBodySO().ID;
        newPet.Genes.Body.RecessiveId = Manager.Gene.GetRandomBodySO().ID;

        newPet.Genes.Color.DominantId = Manager.Gene.GetRandomColorSO().ID;
        newPet.Genes.Color.RecessiveId = Manager.Gene.GetRandomColorSO().ID;

        newPet.Genes.Ear.DominantId = Manager.Gene.GetRandomEarSO().ID;
        newPet.Genes.Ear.RecessiveId = Manager.Gene.GetRandomEarSO().ID;

        newPet.Genes.Eye.DominantId = Manager.Gene.GetRandomEyeSO().ID;
        newPet.Genes.Eye.RecessiveId = Manager.Gene.GetRandomEyeSO().ID;

        newPet.Genes.Feet.DominantId = Manager.Gene.GetRandomFeetSO().ID;
        newPet.Genes.Feet.RecessiveId = Manager.Gene.GetRandomFeetSO().ID;

        newPet.Genes.Mouth.DominantId = Manager.Gene.GetRandomMouthSO().ID;
        newPet.Genes.Mouth.RecessiveId = Manager.Gene.GetRandomMouthSO().ID;

        newPet.Genes.Pattern.DominantId = Manager.Gene.GetRandomPatternSO().ID;
        newPet.Genes.Pattern.RecessiveId = Manager.Gene.GetRandomPatternSO().ID;

        newPet.Genes.Personality.DominantId = Manager.Gene.GetRandomPersonalitySO().ID;
        newPet.Genes.Personality.RecessiveId = Manager.Gene.GetRandomPersonalitySO().ID;

        newPet.Genes.Wing.DominantId = Manager.Gene.GetRandomWingSO().ID;
        newPet.Genes.Wing.RecessiveId = Manager.Gene.GetRandomWingSO().ID;

        string dom = newPet.Genes.Color.DominantId;
        string rec = newPet.Genes.Color.RecessiveId;

        newPet.Genes.PartColors.ArmColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.BodyColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.FeetColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.PatternColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.EarColorId = PickColorId(dom, rec);
        newPet.Genes.PartColors.BlushColorId = PickColorId(dom, rec);

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

    //public void SetLeftReason(LeftReason reason)
    //{
    //    Reason = reason;
    //}
}