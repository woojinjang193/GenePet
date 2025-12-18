using UnityEngine;

public class IslandPetVisualLoader : MonoBehaviour
{
    [Header("파츠 리스트")]
    [SerializeField] private PetPartSpriteList _renderers;
    [SerializeField] private Sprite _leftPetImage;

    public void LoadIslandPet(PetSaveData data)
    {
        if (data == null)
        {
            _renderers.Body.sprite = _leftPetImage;
            return;
        }
        PetVisualHelper.ApplyVisual(data.Genes, _renderers);
    }
}
