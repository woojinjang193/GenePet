using UnityEngine;

public class MiniGamePetVisualLoader : MonoBehaviour
{
    [Header("파츠 리스트")]
    [SerializeField] private PetPartSpriteList _renderers;

    public void LoadPetVisual(PetSaveData data)
    {
        if (data == null)
        {
            Debug.LogError("펫 데이터 없음");
            return;
        }
        PetVisualHelper.ApplyVisual(data.Genes, _renderers);
    }
}
