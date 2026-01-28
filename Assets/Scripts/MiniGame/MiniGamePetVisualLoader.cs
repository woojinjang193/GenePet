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
        GrowthStatus growth = data.GrowthStage;
        SpriteOnOffByGrowth(growth);

        switch (minigame)
        {
            case MiniGame.Jump: break;
            case MiniGame.Rythm: RythmVisualSetting(); break; //리듬게임은 손 바꿈
            case MiniGame.Pinball: break;
        }
    }

    private void RythmVisualSetting() //리듬게임용 손 교체
    {
        //팔 스프라이트 변경
        _renderers.Arm.sprite = _ArmForRythm;
        _renderers.ArmOut.sprite = _ArmOutlineForRythm;

        //레이어 오더 변경
        _renderers.Arm.sortingOrder = 51;
        _renderers.ArmOut.sortingOrder = 52;

        //켜주기 (안전)
        _renderers.Arm.gameObject.SetActive(true);
        _renderers.ArmOut.gameObject.SetActive(true);
    }

    private void SpriteOnOffByGrowth(GrowthStatus growth)
    {
        if (growth == GrowthStatus.Egg || growth == GrowthStatus.Adult) return;
        PetVisualHelper.SetSpriteByGrowth(_renderers, growth);
    }
}
