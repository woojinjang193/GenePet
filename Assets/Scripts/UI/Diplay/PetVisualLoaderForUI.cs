using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetVisualLoaderForUI : MonoBehaviour
{
    [Header("마이펫 파츠")]
    [SerializeField] private PetPartImageList _mypetImages;
    [Header("섬펫 파츠")]
    [SerializeField] private PetPartImageList _islandPetImages;
    [Header("하트 & 화살표")]
    [SerializeField] private GameObject _heartAndArrow;
    [Header("파트너 없음 이미지")]
    [SerializeField] private GameObject _noPartnerImage;
 
    public void SetParents(GenesContainer myPetParent, GenesContainer islandPetParent)
    {
        PetVisualHelperUI.ApplyVisualUI(myPetParent, _mypetImages);
        PetVisualHelperUI.ApplyVisualUI(islandPetParent, _islandPetImages);
    }
    public void SetMyPet(PetSaveData myPet, GrowthStatus growth)
    {
        var genes = myPet.Genes;

        PetVisualHelperUI.ApplyVisualUI(genes, _mypetImages);
        SetImageByGrowth(myPet, growth);
        _noPartnerImage.SetActive(true);
        _heartAndArrow.SetActive(false);
    }
    private void SetImageByGrowth(PetSaveData pet, GrowthStatus growth)
    {
        switch (growth)
        {
            case GrowthStatus.Egg:
                _mypetImages.OffAll();
                _mypetImages.Acc.gameObject.SetActive(true);
                _mypetImages.Acc.sprite = pet.EggSprite;
                break;

            case GrowthStatus.Baby:
                _mypetImages.OffAll();
                _mypetImages.SetBaby();
                break;

            case GrowthStatus.Teen:
                _mypetImages.OffAll();
                _mypetImages.SetTeen();
                break;

            case GrowthStatus.Adult:
                _mypetImages.SetAdult();
                break;
        }
    }
}
