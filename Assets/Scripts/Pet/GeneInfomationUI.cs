using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneInfomationUI : MonoBehaviour, IConfirmRequester
{
    [Header("베이스")]
    [SerializeField] private Image _dominantGene;
    [SerializeField] private Image _recessiveGene;

    [Header("아웃라인")]
    [SerializeField] private Image _dominantGeneOutline;
    [SerializeField] private Image _recessiveGeneOutline;

    [Header("유전자 자르기 버튼")]
    [SerializeField] private Button _dominantCutButton;
    [SerializeField] private Button _recessiveCutButton;

    [Header("유전자 붙히기 버튼")]
    [SerializeField] private Button _dominantGlueButton;
    [SerializeField] private Button _recessiveGlueButton;

    [Header("개런티 버튼")]
    [SerializeField] private Button _doGuaranteeButton;
    [SerializeField] private Button _reGuaranteeButton;

    [Header("잘림표시 이미지")]
    [SerializeField] private GameObject _dominantCutImage;
    [SerializeField] private GameObject _recessiveCutImage;

    [Header("확정표시 이미지")]
    [SerializeField] private GameObject _doGuaranteedImage;
    [SerializeField] private GameObject _reGuaranteedImage;

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
    [SerializeField] private TMP_Text _testerAmount;
    [SerializeField] private TMP_Text _scissorsAmount;
    [SerializeField] private TMP_Text _glueAmount;
    [SerializeField] private TMP_Text _guaranteeStickerAmount;

    [Header("우성 성격 텍스트")]
    [SerializeField] private TMP_Text _dominantPersonalityText;
    [Header("열성 성격 텍스트")]
    [SerializeField] private TMP_Text _recessivePersonalityText;

    [Header("없음 표시 스프라이트")]
    [SerializeField] private Sprite _noneImage;

    [Header("개런티 버튼 아이콘")]
    [SerializeField] private Image _doGuaranteeButtonImage;
    [SerializeField] private Image _reGuaranteeButtonImage;

    [Header("확정취소 버튼 스프라이트")]
    [SerializeField] private Sprite _cancelGuaranteeSprite;

    private Sprite _guaranteeDefaultSprite; //확정버튼 디폴트 이미지 캐싱용

    //현재 펫
    private PetSaveData _curPet;
    private GenePair _curPair;

    private UserItemData _userItemData;

    //버튼에 달린 컴포넌트 리스트
    private List<PartTypeHolder> _holders = new List<PartTypeHolder>();

    private PartTypeHolder _selectedHolder; //선택된 버튼

    private Color _defaultColor = Color.white; //버튼 디폴트 컬러
    private PartType _curPart; //현재 조회중인 파츠 타입

    private void Awake()
    {
        _holders.AddRange(GetComponentsInChildren<PartTypeHolder>());
        Manager.Item.OnItemConsumed += UpdateAmountText;
        Manager.Item.OnRewardGranted += UpdateAmountText;

        foreach (var h in _holders)
        {
            var btn = h.GetComponent<Button>();
            btn.onClick.AddListener(() => OnClickPart(h.partType, h));
        }
        _geneTesterButton.onClick.AddListener(OnClickedGeneTester);
        _dominantCutButton.onClick.AddListener(OnDominantButtonClick);
        _recessiveCutButton.onClick.AddListener(OnRecessiveButtonClick);
        _dominantGlueButton.onClick.AddListener(OnDominantGlueClick);
        _recessiveGlueButton.onClick.AddListener(OnRecessiveGlueClick);
        _doGuaranteeButton.onClick.AddListener(OnDoGuaranteeClick);
        _reGuaranteeButton.onClick.AddListener(OnReGuaranteeClick);

        _userItemData = Manager.Save.CurrentData.UserData.Items;

        _guaranteeDefaultSprite = _doGuaranteeButtonImage.sprite; //같은 이미지이니 하나만 캐싱해도 됨
    }
    private void OnEnable()
    {
        _scissorsAmount.text = $"x{_userItemData.GeneticScissors}"; //가위 숫자
        _glueAmount.text = $"x{_userItemData.GeneticGlue}"; //풀 숫자
        _guaranteeStickerAmount.text = $"x{_userItemData.GuaranteeSticker}"; //확정 숫자
    }
    private void OnDestroy()
    {
        if(Manager.Item != null)
        {
            Manager.Item.OnItemConsumed -= UpdateAmountText;
            Manager.Item.OnRewardGranted -= UpdateAmountText;
        }
    }
    //===== 유전자 테스터 클릭 =========================
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
    //======================유아이 초기 세팅 =========================
    public void Init(PetSaveData pet) 
    {
        if(!pet.IsInfoUnlocked)
        {
            _recessiveMainUI.SetActive(false);
            _geneTesterButton.gameObject.SetActive(true);
        }

        _curPet = pet;
        int amount = Manager.Save.CurrentData.UserData.Items.geneticTester;
        _testerAmount.text = $"x{amount}";

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
    //======================유전자 버튼 클릭========================
    private void OnDominantButtonClick()
    {
        CutGene(true);
    }
    private void OnRecessiveButtonClick()
    {
        CutGene(false);
    }
    private void OnDominantGlueClick()
    {
        GlueGene(true);
    }
    private void OnRecessiveGlueClick()
    {
        GlueGene(false);
    }
    private void OnDoGuaranteeClick()
    {
        GuaranteeGene(true);
    }
    private void OnReGuaranteeClick()
    {
        GuaranteeGene(false);
    }

    // ================유전자 자르기======================
    private void CutGene(bool isDominant)
    {
        if (_userItemData.GeneticScissors <= 0)
        {
            Manager.Game.ShowConfirmMessage("Asking_MoveToShop", 0, this);
            return;
        }

        if (isDominant)
        {
            _curPair.IsDominantCut = true;
        }
        else
        {
            _curPair.IsRecessiveCut = true;
        }

        Manager.Item.UseItem(RewardType.GeneticScissors, 1);
        ResetButtons();
    }
    // ================유전자 붙히기======================
    private void GlueGene(bool isDominant)
    {
        if (_userItemData.GeneticGlue <= 0)
        {
            Manager.Game.ShowConfirmMessage("Asking_MoveToShop", 0, this);
            return;
        }

        if (isDominant)
        {
            _curPair.IsDominantCut = false;
        }
        else
        {
            _curPair.IsRecessiveCut = false;
        }

        Manager.Item.UseItem(RewardType.GeneticGlue, 1);

        ResetButtons();
    }
    // ================유전자 확정======================
    private void GuaranteeGene(bool isDominant)
    {
        if (_userItemData.GuaranteeSticker <= 0) //아이템 부족시
        {
            Manager.Game.ShowConfirmMessage("Asking_MoveToShop", 0, this);
            return;
        }

        // 이미 확정된 상태면 해제확인 팝업
        if (isDominant && _curPair.IsDoGuaranteed)
        {
            Manager.Game.ShowConfirmMessage("Confirm_GeneGuarantorCancel", 3, this);
            return;
        }
        if (!isDominant && _curPair.IsReGuaranteed)
        {
            Manager.Game.ShowConfirmMessage("Confirm_GeneGuarantorCancel", 4, this);
            return;
        }

        // 다른 유전자가 이미 확정이면 확정 불가
        if (isDominant ? _curPair.IsReGuaranteed : _curPair.IsDoGuaranteed)
        {
            Manager.Game.ShowPopup("PopUp_AleadyGuaranteed");
            return;
        }

        // 잘린 상태면 확정 불가 체크 공통화
        bool isCut = isDominant ? _curPair.IsDominantCut : _curPair.IsRecessiveCut;
        if (isCut)
        {
            Manager.Game.ShowPopup("PopUp_AlreadyCut");
            return;
        }

        if(isDominant)
            Manager.Game.ShowConfirmMessage("Confirm_GeneGuarantor", 1, this);
        else
            Manager.Game.ShowConfirmMessage("Confirm_GeneGuarantor", 2, this);
    }

    // ==================== 파츠 카테고리 버튼 클릭시 =======================
    private void OnClickPart(PartType partType, PartTypeHolder holder) 
    {
        _curPart = partType;

        HandleButtonColor(holder);

        ImageReset();

        _dominantPersonalityText.text = ""; //성격 일때만 문자 들어감
        _recessivePersonalityText.text = "";

        _curPair = GetGenePair(partType);

        ResetButtons();

        if (partType == PartType.Color)
        {
            ShowColor(_curPair);
        }
        else if (partType == PartType.Personality)
        {
            ShowPersonality(_curPair);
        }
        else
        {
            ShowPicture(partType, _curPair);
        }
    }
    //======================이미지 리셋 =========================
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
    //======================버튼 컬러 바꿔주는 함수=========================
    private void HandleButtonColor(PartTypeHolder newHolder)
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
    //======================색 보여주기 전용 =========================
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
    //======================성격 보여주기 전용=========================
    private void ShowPersonality(GenePair pair)
    {
        var dom = Manager.Gene.GetPartSOByID<PersonalitySO>(PartType.Personality, pair.DominantId);
        var rec = Manager.Gene.GetPartSOByID<PersonalitySO>(PartType.Personality, pair.RecessiveId);

        _dominantGene.sprite = null;
        _recessiveGene.sprite = null;

        _dominantGeneOutline.sprite = dom.Sprite;
        _recessiveGeneOutline.sprite = rec.Sprite;

        _dominantPersonalityText.text = dom.Personality.ToString();
        _recessivePersonalityText.text = rec.Personality.ToString();
    }
    //======================신체 파트 보여주기 전용=========================
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
    //======================파츠별 위치=========================
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

    //====================파츠 스프라이트 가져오기=========================
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
            case PartType.Personality: return g.Personality;
            default: return g.Body;
        }
    }
    //================== 버튼 리셋 =====================
    private void ResetButtons()
    {
        bool isDominantCut = _curPair.IsDominantCut;
        bool isRecessiveCut = _curPair.IsRecessiveCut;

        bool isDoGuaranteed = _curPair.IsDoGuaranteed;
        bool isReGuaranteed = _curPair.IsReGuaranteed;

        bool anyGuaranteed = isDoGuaranteed || isReGuaranteed; // 둘 중 하나라도 확정이면 true

        _dominantCutButton.interactable = !anyGuaranteed && !isDominantCut; //확정 없고 안잘린 상태면 활성화(우성 컷)
        _dominantGlueButton.interactable = !anyGuaranteed && isDominantCut; //확정 없고 잘린 상태면 활성화(우성 글루)
        _dominantCutImage.SetActive(isDominantCut); // 잘린 표시(우성)

        _recessiveCutButton.interactable = !anyGuaranteed && !isRecessiveCut; // 확정 없고 안잘린 상태면 활성화(열성 컷)
        _recessiveGlueButton.interactable = !anyGuaranteed && isRecessiveCut; //확정 없고 잘린 상태면 활성화(열성 글루)
        _recessiveCutImage.SetActive(isRecessiveCut); //잘린 표시(열성)

        bool canToggleDominantGuarantee = !isDominantCut && (isDoGuaranteed || !anyGuaranteed);  //우성 확정 상태?
        bool canToggleRecessiveGuarantee = !isRecessiveCut && (isReGuaranteed || !anyGuaranteed); //열성 확정 상태?

        if (_curPart == PartType.Color) //Color 파츠면 확정 기능 비활성화
        {
            _doGuaranteeButton.interactable = false; //Color일 땐 확정 버튼 비활성화(우성)
            _reGuaranteeButton.interactable = false; //Color일 땐 확정 버튼 비활성화(열성)

            _doGuaranteeButtonImage.sprite = _guaranteeDefaultSprite; // 아이콘은 기본으로
            _reGuaranteeButtonImage.sprite = _guaranteeDefaultSprite;
        }
        else
        {
            //확정버튼 이미지 변경
            _doGuaranteeButtonImage.sprite = isDoGuaranteed ? _cancelGuaranteeSprite : _guaranteeDefaultSprite;
            _reGuaranteeButtonImage.sprite = isReGuaranteed ? _cancelGuaranteeSprite : _guaranteeDefaultSprite;

            _doGuaranteeButton.interactable = canToggleDominantGuarantee; //우성 확정이면 활성화 (우성 확정 취소버튼)
            _reGuaranteeButton.interactable = canToggleRecessiveGuarantee;//열성 확정이 면 활성화 (열성 확정 취소버튼)
        }
        _doGuaranteedImage.SetActive(isDoGuaranteed); // 확정 표시(우성)
        _reGuaranteedImage.SetActive(isReGuaranteed); // 확정 표시(열성)
    }

    //============유전자 가위/풀 개수 업데이트용 ===============
    private void UpdateAmountText(RewardType type, int newValue)
    {
        if (type == RewardType.None) return;

        if(type == RewardType.GeneticScissors)
        {
            _scissorsAmount.text = $"x{newValue}";
        }
        else if(type == RewardType.GeneticGlue)
        {
            _glueAmount.text = $"x{newValue}";
        }
        else if(type == RewardType.GuaranteeSticker)
        {
            _guaranteeStickerAmount.text = $"x{newValue}";
        }

        Debug.Log($"아이템 사용: {type}이 {newValue} 남음");
    }

    public void Confirmed(int requestNum)
    {
        if (requestNum == 0) //상점이동 요청
        {
            Manager.Game.OpenUiPanel(UIPanel.Shop, true);
        }
        else if (requestNum == 1) //유전자 스티커 사용 (우성용)
        {
            _curPair.IsDoGuaranteed = true;

            Manager.Item.UseItem(RewardType.GuaranteeSticker, 1);
            ResetButtons();
        }
        else if (requestNum == 2)//유전자 스티커 사용 (열성용)
        {
            _curPair.IsReGuaranteed = true;
            Manager.Item.UseItem(RewardType.GuaranteeSticker, 1);
            ResetButtons();
        }
        else if (requestNum == 3) //우성 확정 해제
        {
            _curPair.IsDoGuaranteed = false;
            ResetButtons();
        }
        else if (requestNum == 4)
        {
            _curPair.IsReGuaranteed = false;
            ResetButtons();
        }
    }

    public void Canceled(int requestNum){}
}
