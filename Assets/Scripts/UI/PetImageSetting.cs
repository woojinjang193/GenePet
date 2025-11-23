using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetImageSetting : MonoBehaviour
{

    [Header("파츠 베이스")]
    [SerializeField] private Image _acc;
    [SerializeField] private Image _arm;
    [SerializeField] private Image _blush;
    [SerializeField] private Image _body;
    [SerializeField] private Image _ear;
    [SerializeField] private Image _eye;
    [SerializeField] private Image _feet;
    [SerializeField] private Image _mouth;
    [SerializeField] private Image _pattern;
    [SerializeField] private Image _wing;

    [Header("파츠 아웃라인")]
    [SerializeField] private Image _armOut;
    //[SerializeField] private Image _blushOut;
    [SerializeField] private Image _bodyOut;
    [SerializeField] private Image _earOut;
    [SerializeField] private Image _feetOut;
    [SerializeField] private Image _wingOut;

    [Header("패턴 마스크")]
    [SerializeField] private SpriteMask _patternMask;
    public void DisplayPetImage(PetSaveData petdata)
    {
        var pet = petdata.Genes;

        var acc = Manager.Gene.GetPartSOByID<AccSO>(PartType.Acc, pet.Acc.DominantId);
        var arm = Manager.Gene.GetPartSOByID<ArmSO>(PartType.Arm, pet.Arm.DominantId);
        var blush = Manager.Gene.GetPartSOByID<BlushSO>(PartType.Blush, pet.Blush.DominantId);
        var body = Manager.Gene.GetPartSOByID<BodySO>(PartType.Body, pet.Body.DominantId);
        var pattern = Manager.Gene.GetPartSOByID<PatternSO>(PartType.Pattern, pet.Pattern.DominantId);
        var ear = Manager.Gene.GetPartSOByID<EarSO>(PartType.Ear, pet.Ear.DominantId);
        var eye = Manager.Gene.GetPartSOByID<EyeSO>(PartType.Eye, pet.Eye.DominantId);
        var feet = Manager.Gene.GetPartSOByID<FeetSO>(PartType.Feet, pet.Feet.DominantId);
        var mouth = Manager.Gene.GetPartSOByID<MouthSO>(PartType.Mouth, pet.Mouth.DominantId);
        var wing = Manager.Gene.GetPartSOByID<WingSO>(PartType.Wing, pet.Wing.DominantId);

        //베이스 스프라이트 셋
        _acc.sprite = acc.sprite;
        _arm.sprite = arm.sprite;
        _blush.sprite = blush.sprite;
        _body.sprite = body.sprite;
        _pattern.sprite = pattern.sprite;
        _ear.sprite = ear.sprite;
        _eye.sprite = eye.sprite;
        _feet.sprite = feet.sprite;
        _mouth.sprite = mouth.sprite;
        _wing.sprite = wing.sprite;

        //아웃라인 셋
        //_accOut.sprite = acc.Outline;
        _armOut.sprite = arm.Outline;
        //_blushOut.sprite = blush.Outline;
        _bodyOut.sprite = body.Outline;
        _earOut.sprite = ear.Outline;
        _feetOut.sprite = feet.Outline;
        _wingOut.sprite = wing.Outline;

        //베이스 레이어 순서 셋
        _acc.transform.SetSiblingIndex(acc.OrderInLayer);
        _arm.transform.SetSiblingIndex(arm.OrderInLayer);
        _blush.transform.SetSiblingIndex(blush.OrderInLayer);
        _body.transform.SetSiblingIndex(body.OrderInLayer);
        _pattern.transform.SetSiblingIndex(pattern.OrderInLayer);
        _ear.transform.SetSiblingIndex(ear.OrderInLayer);
        _eye.transform.SetSiblingIndex(eye.OrderInLayer);
        _feet.transform.SetSiblingIndex(feet.OrderInLayer);
        _mouth.transform.SetSiblingIndex(mouth.OrderInLayer);
        _wing.transform.SetSiblingIndex(wing.OrderInLayer);

        //아웃라인 레이어 순서 셋
        _armOut.transform.SetSiblingIndex(arm.OrderInLayer + 1);
        //_blushOut.transform.SetSiblingIndex(blush.OrderInLayer + 1);
        _bodyOut.transform.SetSiblingIndex(body.OrderInLayer + 2);
        _earOut.transform.SetSiblingIndex(ear.OrderInLayer + 1);
        _feetOut.transform.SetSiblingIndex(feet.OrderInLayer + 1);
        _wingOut.transform.SetSiblingIndex(wing.OrderInLayer + 1);

        //마스크 적용
        if (_patternMask == null)
        {
            _patternMask = _pattern.GetComponent<SpriteMask>();
        }
        _patternMask.sprite = _body.sprite;

        ApplyColorsFromGenes(pet.PartColors);
    }
    private void ApplyColorsFromGenes(PartColorGenes colors)
    {
        if (colors == null)
        {
            Debug.LogWarning("PartColorGenes 없음");
            return;
        }

        _arm.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, colors.ArmColorId).color;
        _body.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, colors.BodyColorId).color;
        _blush.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, colors.BlushColorId).color;
        _ear.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, colors.EarColorId).color;
        _feet.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, colors.FeetColorId).color;
        _pattern.color = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, colors.PatternColorId).color;
    }
}
