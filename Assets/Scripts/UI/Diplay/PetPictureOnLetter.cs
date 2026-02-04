using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetPictureOnLetter : MonoBehaviour
{
    [Header("펫 파츠")]
    [SerializeField] private PetPartImageList _petImages;

    public void SetPictureOnLetter(PetSaveData pet, GrowthStatus growth)
    {
        GenesContainer genes = pet.Genes;
        PetVisualHelperUI.ApplyVisualUI(genes, _petImages);
        SetImageByGrowth(pet, growth);
    }
    private void SetImageByGrowth(PetSaveData pet, GrowthStatus growth)
    {
        switch (growth)
        {
            case GrowthStatus.Egg:
                _petImages.OffAll();
                _petImages.Acc.gameObject.SetActive(true);
                _petImages.Acc.sprite = Manager.Item.ItemImages.EggRaritySO.GetEggSprite(pet.Rarity);
                break;

            case GrowthStatus.Baby:
                _petImages.OffAll();
                _petImages.SetBaby();
                break;

            case GrowthStatus.Teen:
                _petImages.OffAll();
                _petImages.SetTeen();
                break;

            case GrowthStatus.Adult:
                _petImages.SetAdult();
                break;
        }
    }
}
