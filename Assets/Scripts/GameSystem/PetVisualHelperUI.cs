using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Burst.Intrinsics;
using UnityEngine;

public static class PetVisualHelperUI
{
    public static void ApplyVisualUI(GenesContainer genes, PetPartImageList images)
    {
        if (images == null || genes == null) return;

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

        images.Acc.sprite = acc.sprite;
        images.Arm.sprite = arm.sprite;
        images.Blush.sprite = blush.sprite;
        images.Body.sprite = body.sprite;
        images.Pattern.sprite = pattern.sprite;
        images.Ear.sprite = ear.sprite;
        images.Eye.sprite = eye.sprite;
        images.Feet.sprite = feet.sprite;
        images.Mouth.sprite = mouth.sprite;
        images.Wing.sprite = wing.sprite;

        if (images.ArmOut) images.ArmOut.sprite = arm.Outline;
        if (images.BodyOut) images.BodyOut.sprite = body.Outline;
        if (images.EarOut) images.EarOut.sprite = ear.Outline;
        if (images.FeetOut) images.FeetOut.sprite = feet.Outline;
        if (images.WingOut) images.WingOut.sprite = wing.Outline;

        //베이스 레이어 순서 셋
        images.Acc.transform.SetSiblingIndex(acc.OrderInLayer);
        images.Arm.transform.SetSiblingIndex(arm.OrderInLayer);
        images.Blush.transform.SetSiblingIndex(blush.OrderInLayer);
        images.Body.transform.SetSiblingIndex(body.OrderInLayer);
        images.Pattern.transform.SetSiblingIndex(pattern.OrderInLayer);
        images.Ear.transform.SetSiblingIndex(ear.OrderInLayer);
        images.Eye.transform.SetSiblingIndex(eye.OrderInLayer);
        images.Feet.transform.SetSiblingIndex(feet.OrderInLayer);
        images.Mouth.transform.SetSiblingIndex(mouth.OrderInLayer);
        images.Wing.transform.SetSiblingIndex(wing.OrderInLayer);

        //아웃라인 레이어 순서 셋
        images.ArmOut.transform.SetSiblingIndex(arm.OrderInLayer + 1);
        //_blushOut.transform.SetSiblingIndex(blush.OrderInLayer + 1);
        images.BodyOut.transform.SetSiblingIndex(body.OrderInLayer + 2);
        images.EarOut.transform.SetSiblingIndex(ear.OrderInLayer + 1);
        images.FeetOut.transform.SetSiblingIndex(feet.OrderInLayer + 1);
        images.WingOut.transform.SetSiblingIndex(wing.OrderInLayer + 1);

        ApplyColorsUI(genes.PartColors, images);

        if (images.PatternMask)
            images.PatternMask.sprite = images.Body.sprite;
    }

    private static void ApplyColorsUI(PartColorGenes c, PetPartImageList t)
    {
        t.Body.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.BodyColorId).color;
        t.Arm.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.ArmColorId).color;
        t.Ear.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.EarColorId).color;
        t.Feet.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.FeetColorId).color;
        t.Pattern.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, c.PatternColorId).color;
    }
}
