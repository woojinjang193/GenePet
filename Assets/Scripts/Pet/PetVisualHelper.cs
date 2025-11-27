using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PetVisualHelper
{
    public static void ApplyVisual(PetSaveData data, PetPartSpriteList renderers)
    {
        if (renderers == null || data == null) return;

        var g = data.Genes;

        //SO 불러오기
        var acc = Manager.Gene.GetPartSOByID<AccSO>(PartType.Acc, g.Acc.DominantId);
        var arm = Manager.Gene.GetPartSOByID<ArmSO>(PartType.Arm, g.Arm.DominantId);
        var blush = Manager.Gene.GetPartSOByID<BlushSO>(PartType.Blush, g.Blush.DominantId);
        var body = Manager.Gene.GetPartSOByID<BodySO>(PartType.Body, g.Body.DominantId);
        var pattern = Manager.Gene.GetPartSOByID<PatternSO>(PartType.Pattern, g.Pattern.DominantId);
        var ear = Manager.Gene.GetPartSOByID<EarSO>(PartType.Ear, g.Ear.DominantId);
        var eye = Manager.Gene.GetPartSOByID<EyeSO>(PartType.Eye, g.Eye.DominantId);
        var feet = Manager.Gene.GetPartSOByID<FeetSO>(PartType.Feet, g.Feet.DominantId);
        var mouth = Manager.Gene.GetPartSOByID<MouthSO>(PartType.Mouth, g.Mouth.DominantId);
        var wing = Manager.Gene.GetPartSOByID<WingSO>(PartType.Wing, g.Wing.DominantId);

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

        // 아웃라인
        if (renderers.ArmOut) renderers.ArmOut.sprite = arm.Outline;
        if (renderers.BodyOut) renderers.BodyOut.sprite = body.Outline;
        if (renderers.EarOut) renderers.EarOut.sprite = ear.Outline;
        if (renderers.FeetOut) renderers.FeetOut.sprite = feet.Outline;
        if (renderers.WingOut) renderers.WingOut.sprite = wing.Outline;

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

        // 아웃라인 레이어
        if (renderers.ArmOut) renderers.ArmOut.sortingOrder = arm.OrderInLayer + 1;
        if (renderers.BodyOut) renderers.BodyOut.sortingOrder = body.OrderInLayer + 2;
        if (renderers.EarOut) renderers.EarOut.sortingOrder = ear.OrderInLayer + 1;
        if (renderers.FeetOut) renderers.FeetOut.sortingOrder = feet.OrderInLayer + 1;
        if (renderers.WingOut) renderers.WingOut.sortingOrder = wing.OrderInLayer + 1;

        // 색 적용
        ApplyColors(g.PartColors, renderers);

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
    }
}
