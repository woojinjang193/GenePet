
using UnityEngine;

public static class PetVisualHelper
{
    public static void ApplyVisual(GenesContainer genes, PetPartSpriteList renderers)
    {
        if (renderers == null || genes == null) return;

        //var g = data.Genes;

        //SO 불러오기
        var acc = Manager.Gene.GetPartSOByID<AccSO>(PartType.Acc, genes.Acc.DominantId);
        var arm = Manager.Gene.GetPartSOByID<ArmSO>(PartType.Arm, genes.Arm.DominantId);
        var blush = Manager.Gene.GetPartSOByID<BlushSO>(PartType.Blush, genes.Blush.DominantId);
        var body = Manager.Gene.GetPartSOByID<BodySO>(PartType.Body, genes.Body.DominantId);
        var pattern = Manager.Gene.GetPartSOByID<PatternSO>(PartType.Pattern, genes.Pattern.DominantId);
        var ear = Manager.Gene.GetPartSOByID<EarSO>(PartType.Ear, genes.Ear.DominantId);
        var eye = Manager.Gene.GetPartSOByID<EyeSO>(PartType.Eye, genes.Eye.DominantId);
        var feet = Manager.Gene.GetPartSOByID<FeetSO>(PartType.Feet, genes.Feet.DominantId);
        var mouth = Manager.Gene.GetPartSOByID<MouthSO>(PartType.Mouth, genes.Mouth.DominantId);
        var wing = Manager.Gene.GetPartSOByID<WingSO>(PartType.Wing, genes.Wing.DominantId);
        var tail = Manager.Gene.GetPartSOByID<TailSO>(PartType.Tail, genes.Tail.DominantId);
        var whiskers = Manager.Gene.GetPartSOByID<WhiskersSO>(PartType.Whiskers, genes.Whiskers.DominantId);

        // 스프라이트 적용
        renderers.Acc.sprite = acc.sprite;
        renderers.Arm.sprite = arm.sprite;
        renderers.Blush.sprite = blush.sprite;
        renderers.Body.sprite = body.sprite;
        renderers.Pattern.sprite = pattern.sprite;
        renderers.Ear.sprite = ear.sprite;
        renderers.Eye.sprite = eye.sprite;
        renderers.Feet.sprite = feet.sprite;
        renderers.Mouth.sprite = mouth.sprite;
        renderers.Wing.sprite = wing.sprite;
        renderers.Tail.sprite = tail.sprite;
        renderers.Whiskers.sprite = whiskers.sprite;

        // 아웃라인
        if (renderers.ArmOut != null) renderers.ArmOut.sprite = arm.Outline;
        if (renderers.BodyOut != null) renderers.BodyOut.sprite = body.Outline;
        if (renderers.EarOut != null) renderers.EarOut.sprite = ear.Outline;
        if (renderers.FeetOut != null) renderers.FeetOut.sprite = feet.Outline;
        if (renderers.WingOut != null) renderers.WingOut.sprite = wing.Outline;
        if (renderers.TailOut != null) renderers.TailOut.sprite = tail.Outline;

        // 레이어 순서
        renderers.Acc.sortingOrder = acc.OrderInLayer;
        renderers.Arm.sortingOrder = arm.OrderInLayer;
        renderers.Blush.sortingOrder = blush.OrderInLayer;
        renderers.Body.sortingOrder = body.OrderInLayer;
        renderers.Pattern.sortingOrder = pattern.OrderInLayer;
        renderers.Ear.sortingOrder = ear.OrderInLayer;
        renderers.Eye.sortingOrder = eye.OrderInLayer;
        renderers.Feet.sortingOrder = feet.OrderInLayer;
        renderers.Mouth.sortingOrder = mouth.OrderInLayer;
        renderers.Wing.sortingOrder = wing.OrderInLayer;
        renderers.Tail.sortingOrder = tail.OrderInLayer;
        renderers.Whiskers.sortingOrder = whiskers.OrderInLayer;

        // 아웃라인 레이어
        if (renderers.ArmOut != null) renderers.ArmOut.sortingOrder = arm.OrderInLayer + 1;
        if (renderers.BodyOut != null) renderers.BodyOut.sortingOrder = body.OrderInLayer + 2;
        if (renderers.EarOut != null) renderers.EarOut.sortingOrder = ear.OrderInLayer + 1;
        if (renderers.FeetOut != null) renderers.FeetOut.sortingOrder = feet.OrderInLayer + 1;
        if (renderers.WingOut != null) renderers.WingOut.sortingOrder = wing.OrderInLayer + 1;
        if (renderers.TailOut != null) renderers.TailOut.sortingOrder = tail.OrderInLayer + 1;

        // 색 적용
        ApplyColors(genes.PartColors, renderers);

        // 마스크
        if (renderers.PatternMask)
            renderers.PatternMask.sprite = renderers.Body.sprite;
    }

    private static void ApplyColors(PartColorGenes c, PetPartSpriteList t)
    {
        if (c == null) return;

        t.Body.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.BodyColorId).color;
        t.Arm.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.ArmColorId).color;
        //t.Blush.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.BlushColorId).color;
        t.Ear.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.EarColorId).color;
        t.Feet.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.FeetColorId).color;
        t.Pattern.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.PatternColorId).color;
        t.Wing.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.WingColorId).color;
        t.Tail.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.TailColorId).color;
    }

    //파츠 비활성화 
    /// <summary>
    /// 모든 랜더러를 초기화합니다 (null로 만듬)
    /// </summary>
    /// <param name="renderers"></param>
    public static void OffAllParts(PetPartSpriteList renderers)
    {
        renderers.Acc.sprite = null;
        renderers.Arm.sprite = null;
        renderers.Blush.sprite = null;
        renderers.Body.sprite = null;
        renderers.Pattern.sprite = null;
        renderers.Ear.sprite = null;
        renderers.Eye.sprite = null;
        renderers.Feet.sprite = null;
        renderers.Mouth.sprite = null;
        renderers.Wing.sprite = null;
        renderers.Tail.sprite = null; 
        renderers.Whiskers.sprite = null;

        renderers.ArmOut.sprite = null;
        renderers.BodyOut.sprite = null;
        renderers.EarOut.sprite = null;
        renderers.FeetOut.sprite = null;
        renderers.WingOut.sprite = null;
        renderers.TailOut.sprite = null;
    }

    public static void SetSpriteByGrowth(PetPartSpriteList renderers, GrowthStatus growth) //스프라이트 끄고킴
    {
        ActiveFalseAll(renderers); //먼저 전부 꺼줌

        if (growth == GrowthStatus.Egg) //알일때
        {
            return; //알이면 리턴
        }

        if (growth == GrowthStatus.Baby) //애기일때
        {
            renderers.Eye.gameObject.SetActive(true);
            renderers.Body.gameObject.SetActive(true);
            renderers.Ear.gameObject.SetActive(true);
            renderers.Blush.gameObject.SetActive(true);
            renderers.Mouth.gameObject.SetActive(true);
            renderers.Tail.gameObject.SetActive(true);

            renderers.BodyOut.gameObject.SetActive(true);
            renderers.EarOut.gameObject.SetActive(true);
            renderers.TailOut.gameObject.SetActive(true);

        }
        else if (growth == GrowthStatus.Teen) //성장기
        {
            renderers.Blush.gameObject.SetActive(true);
            renderers.Body.gameObject.SetActive(true);
            renderers.Ear.gameObject.SetActive(true);
            renderers.Eye.gameObject.SetActive(true);
            renderers.Mouth.gameObject.SetActive(true);
            renderers.Tail.gameObject.SetActive(true);
            renderers.Whiskers.gameObject.SetActive(true);
            renderers.Arm.gameObject.SetActive(true);

            renderers.ArmOut.gameObject.SetActive(true);
            renderers.BodyOut.gameObject.SetActive(true);
            renderers.EarOut.gameObject.SetActive(true);
            renderers.TailOut.gameObject.SetActive(true);
        }
        else if (growth == GrowthStatus.Adult) //어른
        {
            renderers.Acc.gameObject.SetActive(true);
            renderers.Arm.gameObject.SetActive(true);
            renderers.Blush.gameObject.SetActive(true);
            renderers.Body.gameObject.SetActive(true);
            renderers.Ear.gameObject.SetActive(true);
            renderers.Eye.gameObject.SetActive(true);
            renderers.Feet.gameObject.SetActive(true);
            renderers.Mouth.gameObject.SetActive(true);
            renderers.Pattern.gameObject.SetActive(true);
            renderers.Wing.gameObject.SetActive(true);
            renderers.Tail.gameObject.SetActive(true);
            renderers.Whiskers.gameObject.SetActive(true);

            renderers.ArmOut.gameObject.SetActive(true);
            renderers.BodyOut.gameObject.SetActive(true);
            renderers.EarOut.gameObject.SetActive(true);
            renderers.FeetOut.gameObject.SetActive(true);
            renderers.WingOut.gameObject.SetActive(true);
            renderers.TailOut.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("성장상태 이상함 확인 해야함.");
        }
    }

    /// <summary>
    /// 모든 오브젝트를 Off 합니다
    /// </summary>
    /// <param name="renderers"></param>
    public static void ActiveFalseAll(PetPartSpriteList renderers)
    {
        renderers.Acc.gameObject.SetActive(false);
        renderers.Arm.gameObject.SetActive(false);
        renderers.Blush.gameObject.SetActive(false);
        renderers.Body.gameObject.SetActive(false);
        renderers.Pattern.gameObject.SetActive(false);
        renderers.Ear.gameObject.SetActive(false);
        renderers.Eye.gameObject.SetActive(false);
        renderers.Feet.gameObject.SetActive(false);
        renderers.Mouth.gameObject.SetActive(false);
        renderers.Wing.gameObject.SetActive(false);
        renderers.Tail.gameObject.SetActive(false);
        renderers.Whiskers.gameObject.SetActive(false);

        renderers.ArmOut.gameObject.SetActive(false);
        renderers.BodyOut.gameObject.SetActive(false);
        renderers.EarOut.gameObject.SetActive(false);
        renderers.FeetOut.gameObject.SetActive(false);
        renderers.WingOut.gameObject.SetActive(false);
        renderers.TailOut.gameObject.SetActive(false);
    }
}
