using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GeneInfomationUI : MonoBehaviour
{
    [Header("베이스")]
    [SerializeField] private Image _dominantGene;
    [SerializeField] private Image _recessiveGene;

    [Header("아웃라인")]
    [SerializeField] private Image _dominantGeneOutline;
    [SerializeField] private Image _recessiveGeneOutline;

    [Header("유전자 가위 버튼")]
    [SerializeField] private Button _dominantButton;
    [SerializeField] private Button _recessiveButton;

    [Header("없음 표시")]
    [SerializeField] private Sprite _noneImage;

    [Header("잘림표시 이미지")]
    [SerializeField] private GameObject _dominantCutImage;
    [SerializeField] private GameObject _recessiveCutImage;

    [Header("레어도 별")]
    [SerializeField] private ShowRarityUI _dominantRarityUI;
    [SerializeField] private ShowRarityUI _recessiveRarityUI;

    [Header("유전자 테스트 버튼")]
    [SerializeField] private Button _geneTesterButton;

    [Header("열성 UI")]
    [SerializeField] private GameObject _recessiveMainUI;

    [Header("버튼 눌리는 색깔")]
    [SerializeField] private Color _selectedColor; //버튼 선택시 컬러

    [Header("아이템 개수")]
    [SerializeField] private TMP_Text _itemAmount;

    //현재 펫
    private PetSaveData _curPet;
    private GenePair _curPair;

    //버튼에 달린 컴포넌트 리스트
    private List<PartTypeHolder> _holders = new List<PartTypeHolder>();

    private PartTypeHolder _selectedHolder; //선택된 버튼

    private Color _defaultColor = Color.white; //버튼 디폴트 컬러
    

    private void Awake()
    {
        _holders.AddRange(GetComponentsInChildren<PartTypeHolder>());

        foreach (var h in _holders)
        {
            var btn = h.GetComponent<Button>();
            btn.onClick.AddListener(() => OnClickPart(h.partType, h));
        }
        _geneTesterButton.onClick.AddListener(OnClickedGeneTester);
        _dominantButton.onClick.AddListener(TryToCutDominantGene);
        _recessiveButton.onClick.AddListener(TryToCutRecessiveGene);
    }

    private void OnClickedGeneTester()
    {
        bool isUnlocked = _curPet.IsInfoUnlocked;

        if (!isUnlocked)
        {
            int itemAmount = Manager.Save.CurrentData.UserData.Items.geneticTester;

            if (itemAmount > 0)
            {
                Manager.Save.CurrentData.UserData.Items.geneticTester--;
                _curPet.IsInfoUnlocked = true;
                Debug.Log($"언락 : {_curPet.IsInfoUnlocked}");
                Debug.Log($"남은 테스터 개수 : {Manager.Save.CurrentData.UserData.Items.geneticTester}");
            }
            else
            {
                return;
            }
        }
        _recessiveMainUI.SetActive(true);
        _geneTesterButton.gameObject.SetActive(false);
    }
    public void Init(PetSaveData pet) //유아이 초기 세팅
    {
        if(!pet.IsInfoUnlocked)
        {
            _recessiveMainUI.SetActive(false);
            _geneTesterButton.gameObject.SetActive(true);
        }

        _curPet = pet;
        _itemAmount.text = Manager.Save.CurrentData.UserData.Items.geneticTester.ToString();

        //초기세팅 바디로 설정
        foreach (var h in _holders)
        {
            if (h.partType == PartType.Body)
            {
                OnClickPart(PartType.Body, h);
                break;
            }
        }
    }
    private void TryToCutDominantGene()
    {
        CutGene(true);
    }
    private void TryToCutRecessiveGene()
    {
        CutGene(false);
    }
    private void CutGene(bool isDominant)
    {
        if (isDominant)
        {
            _curPair.IsDominantCut = true;
            _dominantCutImage.SetActive(true);
        }
        else
        {
            _curPair.IsRecessiveCut = true;
            _recessiveCutImage.SetActive(true);
        }

        _dominantButton.interactable = CanCutGene(_curPair);
        _recessiveButton.interactable = CanCutGene(_curPair);
    }

    private void OnClickPart(PartType partType, PartTypeHolder holder) 
    {
        HandleButtonColor(holder);

        ImageReset();

        _curPair = GetGenePair(partType);

        _dominantButton.interactable = CanCutGene(_curPair);
        _recessiveButton.interactable = CanCutGene(_curPair);

        if (partType == PartType.Color)
        {
            ShowColor(_curPair);
        }
        else
        {
            ShowPicture(partType, _curPair);
        }
    }
    private void ImageReset() //이미지 스케일, 컬러 리셋
    {
        _dominantCutImage.SetActive(false);
        _recessiveCutImage.SetActive(false);

        _dominantGene.transform.localScale = Vector3.one;
        _dominantGeneOutline.transform.localScale = Vector3.one;

        _recessiveGene.transform.localScale = Vector3.one;
        _recessiveGeneOutline.transform.localScale = Vector3.one;

        _dominantGeneOutline.color = Color.white;
        _recessiveGeneOutline.color = Color.white;
    }
    private void HandleButtonColor(PartTypeHolder newHolder) //버튼 컬러 함수
    {
        if (_selectedHolder != null)
        {
            var img = _selectedHolder.GetComponent<Image>();
            img.color = _defaultColor;
        }

        var newImg = newHolder.GetComponent<Image>();
        newImg.color = _selectedColor;

        _selectedHolder = newHolder;
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
        bool isDomNone = pair.DominantId == "00";
        bool isRecNone = pair.RecessiveId == "00";
 
        Sprite dom = isDomNone ? GetNoneSprite(partType) : GetSprite(partType, pair.DominantId);
        Sprite rec = isRecNone ? GetNoneSprite(partType) : GetSprite(partType, pair.RecessiveId);
 
        Sprite domOut = isDomNone ? null : GetOutline(partType, pair.DominantId);
        Sprite recOut = isRecNone ? null : GetOutline(partType, pair.RecessiveId);

        _dominantGene.color = Color.white;
        _recessiveGene.color = Color.white;

        _dominantGene.sprite = dom;
        _recessiveGene.sprite = rec;

        _dominantGeneOutline.sprite = domOut;
        _recessiveGeneOutline.sprite = recOut;

        _dominantGene.transform.localScale = isDomNone ? Vector3.one : GetScale(partType);
        _dominantGeneOutline.transform.localScale = isDomNone ? Vector3.one : GetScale(partType);

        _recessiveGene.transform.localScale = isRecNone ? Vector3.one : GetScale(partType);
        _recessiveGeneOutline.transform.localScale = isRecNone ? Vector3.one : GetScale(partType);

        _dominantGeneOutline.color = domOut == null ? new Color(1, 1, 1, 0) : Color.white;
        _recessiveGeneOutline.color = recOut == null ? new Color(1, 1, 1, 0) : Color.white;

        //레어리티 전달
        var domSO =  Manager.Gene.GetPartSOByID<PartBaseSO>(partType, pair.DominantId);
        var recSO =  Manager.Gene.GetPartSOByID<PartBaseSO>(partType, pair.RecessiveId);

        _dominantRarityUI.ShowRarity(domSO.Rarity);
        _recessiveRarityUI.ShowRarity(recSO.Rarity);
    }
    private Sprite GetNoneSprite(PartType partType)
    {
        switch (partType)
        {
            case PartType.Acc: return _noneImage;
            case PartType.Wing: return _noneImage;
            case PartType.Pattern: return _noneImage;
            case PartType.Tail: return _noneImage;
            case PartType.Whiskers : return _noneImage;
            default: return null;
        }
    }
    private Vector3 GetScale(PartType partType)
    {
        switch (partType)
        {
            case PartType.Acc: return new Vector3(2f,2f,1f);
            case PartType.Body: return new Vector3(1.5f, 1.5f, 1f);
            case PartType.Arm: return new Vector3(1.5f, 1.5f, 1f);
            case PartType.Feet: return new Vector3(1.5f, 1.5f, 1f);
            case PartType.Eye: return new Vector3(2.5f, 2.5f, 1f);
            case PartType.Mouth: return new Vector3(4f, 4f, 1f);
            case PartType.Ear: return new Vector3(1.5f, 1.5f, 1f);
            case PartType.Pattern: return new Vector3(1f, 1f, 1f);
            case PartType.Wing: return new Vector3(1.2f, 1.2f, 1f);
            case PartType.Blush: return new Vector3(2f, 2f, 1f);
            case PartType.Tail: return new Vector3(2f, 2f, 1f);
            case PartType.Whiskers: return new Vector3(2f, 2f, 1f);
        }
        return Vector3.one;
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
            case PartType.Tail: return Manager.Gene.GetPartSOByID<TailSO>(part, id)?.sprite;
            case PartType.Whiskers: return Manager.Gene.GetPartSOByID<WhiskersSO>(part, id)?.sprite;
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
            case PartType.Whiskers: return null;
            case PartType.Body: return Manager.Gene.GetPartSOByID<BodySO>(part, id)?.Outline;
            case PartType.Arm: return Manager.Gene.GetPartSOByID<ArmSO>(part, id)?.Outline;
            case PartType.Feet: return Manager.Gene.GetPartSOByID<FeetSO>(part, id)?.Outline;
            case PartType.Wing: return Manager.Gene.GetPartSOByID<WingSO>(part, id)?.Outline;
            case PartType.Ear: return Manager.Gene.GetPartSOByID<EarSO>(part, id)?.Outline;
            case PartType.Tail: return Manager.Gene.GetPartSOByID<TailSO>(part, id)?.Outline;
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
            case PartType.Tail: return g.Tail;
            case PartType.Whiskers: return g.Whiskers;
            default: return g.Body;
        }
    }

    private bool CanCutGene(GenePair genePair)
    {
        if(genePair.IsDominantCut)
        {
            _dominantCutImage.SetActive(true);
            return false;
        }
        if(genePair.IsRecessiveCut)
        {
            _recessiveCutImage.SetActive(true);
            return false;
        }
        else
        {
            return true;
        }
    }
}
