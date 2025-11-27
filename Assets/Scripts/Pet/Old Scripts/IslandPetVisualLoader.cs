using UnityEngine;

public class IslandPetVisualLoader : MonoBehaviour
{
    [Header("파츠 리스트")]
    [SerializeField] private PetPartSpriteList _renderers;

    public void LoadIslandPet(PetSaveData data)
    {
        PetVisualHelper.ApplyVisual(data, _renderers);
    }
}
