using UnityEngine;

public class MiniGamePetVisualLoader : MonoBehaviour
{
    [Header("파츠 리스트")]
    [SerializeField] private PetPartSpriteList _renderers;
    [Header("리듬 전용 손")]
    [SerializeField] private Sprite _ArmForRythm;
    [SerializeField] private Sprite _ArmOutlineForRythm;

    public void LoadPetVisual(PetSaveData data, MiniGame minigame)
    {
        if (data == null)
        {
            Debug.LogError("펫 데이터 없음");
            return;
        }
        PetVisualHelper.ApplyVisual(data.Genes, _renderers);

        switch(minigame)
        {
            case MiniGame.Jump: break;
            case MiniGame.Rythm: RythmVisualSetting(); break;
            case MiniGame.Pinball: break;
        }
    }

    private void RythmVisualSetting() //리듬게임용 손 교체
    {
        _renderers.Arm.sprite = _ArmForRythm; 
        _renderers.ArmOut.sprite = _ArmOutlineForRythm; 
    }
}
