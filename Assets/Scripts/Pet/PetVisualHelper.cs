using System.Collections;
using System.Collections.Generic;
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
}
