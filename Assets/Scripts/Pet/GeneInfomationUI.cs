using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneInfomationUI : MonoBehaviour
{
    [Header("스프라이트")]
    [SerializeField] private Image _dominantGene;
    [SerializeField] private Image _recessiveGene;
    [SerializeField] private Image _dominantGeneOutline;
    [SerializeField] private Image _recessiveGeneOutline;

    private PetSaveData _curPet;

    private List<PartTypeHolder> _holders = new List<PartTypeHolder>();

    private void Awake()
    {
        _holders.AddRange(GetComponentsInChildren<PartTypeHolder>());

        foreach (var h in _holders)
        {
            var btn = h.GetComponent<Button>();
            btn.onClick.AddListener(() => OnClickPart(h.partType));
        }
    }

    public void Init(PetSaveData pet)
    {
        _curPet = pet;
    }

    private void OnClickPart(PartType partType)
    {
        _dominantGeneOutline.color = Color.white;
        _recessiveGeneOutline.color = Color.white;

        GenePair pair = GetGenePair(partType);

        if (partType == PartType.Color)
        {
            ShowColor(pair);
        }
        else
        {
            ShowPicture(partType, pair);
        }
    }
    private void ShowColor(GenePair pair)
    {
        var dom = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, pair.DominantId);
        var rec = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, pair.RecessiveId);

        _dominantGeneOutline.color = dom.color;
        _recessiveGeneOutline.color = rec.color;

        _dominantGene.sprite = null;
        _recessiveGene.sprite = null;

        _dominantGeneOutline.sprite = null;
        _recessiveGeneOutline.sprite = null;
    }
    private void ShowPicture(PartType partType, GenePair pair)
    {
        Sprite dom = GetSprite(partType, pair.DominantId);
        Sprite rec = GetSprite(partType, pair.RecessiveId);

        Sprite domOut = GetOutline(partType, pair.DominantId);
        Sprite recOut = GetOutline(partType, pair.RecessiveId);

        _dominantGene.color = Color.white;
        _recessiveGene.color = Color.white;

        _dominantGene.sprite = dom;
        _recessiveGene.sprite = rec;

        _dominantGeneOutline.sprite = domOut;
        _recessiveGeneOutline.sprite = recOut;

        _dominantGeneOutline.color = domOut == null ? new Color(1, 1, 1, 0) : Color.white;
        _recessiveGeneOutline.color = recOut == null ? new Color(1, 1, 1, 0) : Color.white;

    }

    private Sprite GetSprite(PartType part, string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        switch (part)
        {
            case PartType.Acc: return Manager.Gene.GetPartSOByID<AccSO>(part, id)?.sprite;
            case PartType.Body: return Manager.Gene.GetPartSOByID<BodySO>(part, id)?.sprite;
            case PartType.Arm: return Manager.Gene.GetPartSOByID<ArmSO>(part, id)?.sprite;
            case PartType.Feet: return Manager.Gene.GetPartSOByID<FeetSO>(part, id)?.sprite;
            case PartType.Eye: return Manager.Gene.GetPartSOByID<EyeSO>(part, id)?.sprite;
            case PartType.Mouth: return Manager.Gene.GetPartSOByID<MouthSO>(part, id)?.sprite;
            case PartType.Ear: return Manager.Gene.GetPartSOByID<EarSO>(part, id)?.sprite;
            case PartType.Pattern: return Manager.Gene.GetPartSOByID<PatternSO>(part, id)?.sprite;
            case PartType.Wing: return Manager.Gene.GetPartSOByID<WingSO>(part, id)?.sprite;
            case PartType.Blush: return Manager.Gene.GetPartSOByID<BlushSO>(part, id)?.sprite;
        }
        return null;
    }
    private Sprite GetOutline(PartType part, string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        switch (part)
        {
            case PartType.Acc: return null;
            case PartType.Eye: return null;
            case PartType.Mouth: return null;
            case PartType.Pattern: return null;
            case PartType.Blush: return null;
            case PartType.Body: return Manager.Gene.GetPartSOByID<BodySO>(part, id)?.Outline;
            case PartType.Arm: return Manager.Gene.GetPartSOByID<ArmSO>(part, id)?.Outline;
            case PartType.Feet: return Manager.Gene.GetPartSOByID<FeetSO>(part, id)?.Outline;
            case PartType.Wing: return Manager.Gene.GetPartSOByID<WingSO>(part, id)?.Outline;
            case PartType.Ear: return Manager.Gene.GetPartSOByID<EarSO>(part, id)?.Outline;
        }
        return null;
    }
    private GenePair GetGenePair(PartType part)
    {
        GenesContainer g = _curPet.Genes;

        switch (part)
        {
            case PartType.Body: return g.Body;
            case PartType.Arm: return g.Arm;
            case PartType.Feet: return g.Feet;
            case PartType.Pattern: return g.Pattern;
            case PartType.Eye: return g.Eye;
            case PartType.Mouth: return g.Mouth;
            case PartType.Ear: return g.Ear;
            case PartType.Acc: return g.Acc;
            case PartType.Wing: return g.Wing;
            case PartType.Blush: return g.Blush;
            case PartType.Color: return g.Color;
            default: return g.Body;
        }
    }
}
