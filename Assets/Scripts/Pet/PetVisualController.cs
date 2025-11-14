using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetVisualController : MonoBehaviour
{
    [SerializeField] private PetUnit _pet;

    [Header("알")]
    [SerializeField] private SpriteRenderer _egg;

    [Header("파츠 베이스")]
    [SerializeField] private SpriteRenderer _acc;
    [SerializeField] private SpriteRenderer _arm;
    [SerializeField] private SpriteRenderer _blush;
    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer _ear;
    [SerializeField] private SpriteRenderer _eye;
    [SerializeField] private SpriteRenderer _feet;
    [SerializeField] private SpriteRenderer _mouth;
    [SerializeField] private SpriteRenderer _pattern;
    [SerializeField] private SpriteRenderer _wing;

    [Header("파츠 아웃라인")]
    [SerializeField] private SpriteRenderer _accOut;
    [SerializeField] private SpriteRenderer _armOut;
    [SerializeField] private SpriteRenderer _blushOut;
    [SerializeField] private SpriteRenderer _bodyOut;
    [SerializeField] private SpriteRenderer _earOut;
    [SerializeField] private SpriteRenderer _feetOut;
    [SerializeField] private SpriteRenderer _wingOut;

    [Header("패턴 마스크")]
    [SerializeField] private SpriteMask _patternMask;

    //[Header("테스트 버튼")]
    //[SerializeField] private Button _button; //테스트용. 지워야함

    public PetStatusCore Status
    {
        get { return _pet != null ? _pet.Status : null; }
    }

    private void Awake()
    {
        //Status.OnGrowthChanged += OnGrowthChanged;
        //_button.onClick.AddListener(ButtonClicked); //테스트용. 지워야함
    }

    public void VisualizePet()
    {
        LoadOwnPet();
        SetSprite(Status.Growth);
    }

    private void LoadOwnPet()
    {
        if (!AreAllRenderersAssigned())
        {
            Debug.LogError("스프라이트 랜더러 없음");
            return;
        }
        var saveData = Manager.Save.CurrentData.UserData.HavePetList[0];
        var pet = saveData.Genes;

        _pet.Status.Growth = saveData.GrowthStage;

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
        _accOut.sprite = acc.Outline;
        _armOut.sprite = arm.Outline;
        //_blushOut.sprite = blush.Outline;
        _bodyOut.sprite = body.Outline;
        _earOut.sprite = ear.Outline;
        _feetOut.sprite = feet.Outline;
        _wingOut.sprite = wing.Outline;

        //베이스 레이어 순서 셋
        _acc.sortingOrder = acc.OrderInLayer;
        _arm.sortingOrder = arm.OrderInLayer;
        _blush.sortingOrder = blush.OrderInLayer;
        _body.sortingOrder = body.OrderInLayer;
        _pattern.sortingOrder = pattern.OrderInLayer;
        _ear.sortingOrder = ear.OrderInLayer;
        _eye.sortingOrder = eye.OrderInLayer;
        _feet.sortingOrder = feet.OrderInLayer;
        _mouth.sortingOrder = mouth.OrderInLayer;
        _wing.sortingOrder = wing.OrderInLayer;

        //아웃라인 레이어 순서 셋
        _accOut.sortingOrder = acc.OrderInLayer + 1;
        _armOut.sortingOrder = arm.OrderInLayer + 1;
        _blushOut.sortingOrder = blush.OrderInLayer + 1;
        _bodyOut.sortingOrder = body.OrderInLayer + 2;
        _earOut.sortingOrder = ear.OrderInLayer + 1;
        _feetOut.sortingOrder = feet.OrderInLayer + 1;
        _wingOut.sortingOrder = wing.OrderInLayer + 1;

        //마스크 적용
        if(_patternMask == null)
        {
            _patternMask = _pattern.GetComponent<SpriteMask>();
        }    
        _patternMask.sprite = _body.sprite;

        ApplyColorsFromGenes(pet.PartColors);
    }

    private void OnGrowthChanged(GrowthStatus growth)
    {
        SetSprite(growth);
    }

    private void SetSprite(GrowthStatus growth) //스프라이트 끄고킴
    {
        if (!AreAllRenderersAssigned())
        {
            Debug.Log("랜더러 준비 안됨");
            return;
        }

        HideAllParts();

        if (growth == GrowthStatus.Egg) //알일때
        {
            _egg.gameObject.SetActive(true);
            Debug.Log("Egg 상태 스프라이트 세팅");
            return;
        }

        if (growth == GrowthStatus.Baby) //애기일때
        {
            _eye.gameObject.SetActive(true);
            _body.gameObject.SetActive(true);
            _ear.gameObject.SetActive(true);
            _blush.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);

            _bodyOut.gameObject.SetActive(true);
            _blushOut.gameObject.SetActive(true);
            Debug.Log("Baby 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Teen) //성장기
        {
            _blush.gameObject.SetActive(true);
            _body.gameObject.SetActive(true);
            _ear.gameObject.SetActive(true);
            _eye.gameObject.SetActive(true);
            _feet.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);

            _blushOut.gameObject.SetActive(true);
            _bodyOut.gameObject.SetActive(true);
            _earOut.gameObject.SetActive(true);
            _feetOut.gameObject.SetActive(true);
            Debug.Log("Teen 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Adult) //어른
        {
            _acc.gameObject.SetActive(true);
            _arm.gameObject.SetActive(true);
            _blush.gameObject.SetActive(true);
            _body.gameObject.SetActive(true);
            _ear.gameObject.SetActive(true);
            _eye.gameObject.SetActive(true);
            _feet.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            _pattern.gameObject.SetActive(true);
            _wing.gameObject.SetActive(true);

            _accOut.gameObject.SetActive(true);
            _armOut.gameObject.SetActive(true);
            _blushOut.gameObject.SetActive(true);
            _bodyOut.gameObject.SetActive(true);
            _earOut.gameObject.SetActive(true);
            _feetOut.gameObject.SetActive(true);
            _wingOut.gameObject.SetActive(true);

            Debug.Log("Adult 상태 스프라이트 세팅");
        }
        else
        {
            Debug.LogWarning("성장상태 이상함 확인 해야함.");
        }

        _egg.gameObject.SetActive(false);
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

    private void HideAllParts()                                    
    {
        if (_egg != null) _egg.gameObject.SetActive(false);        
        if (_acc != null) _acc.gameObject.SetActive(false);     
        if (_arm != null) _arm.gameObject.SetActive(false);
        if (_blush != null) _blush.gameObject.SetActive(false);
        if (_body != null) _body.gameObject.SetActive(false);      
        if (_ear != null) _ear.gameObject.SetActive(false);
        if (_eye != null) _eye.gameObject.SetActive(false);
        if (_feet != null) _feet.gameObject.SetActive(false);
        if (_mouth != null) _mouth.gameObject.SetActive(false);
        if (_pattern != null) _pattern.gameObject.SetActive(false);
        if (_wing != null) _wing.gameObject.SetActive(false);

        if (_accOut != null) _acc.gameObject.SetActive(false);
        if (_armOut != null) _arm.gameObject.SetActive(false);
        if (_blushOut != null) _blush.gameObject.SetActive(false);
        if (_bodyOut != null) _body.gameObject.SetActive(false);
        if (_earOut != null) _ear.gameObject.SetActive(false);
        if (_feetOut != null) _feet.gameObject.SetActive(false);
        if (_wingOut != null) _wing.gameObject.SetActive(false);
    }

    private bool AreAllRenderersAssigned()
    {
        if (_egg == null) return false;   
        if (_acc == null) return false;
        if (_arm == null) return false;
        if (_body == null) return false;
        if (_blush == null) return false;
        if (_ear == null) return false;
        if (_eye == null) return false;
        if (_feet == null) return false;
        if (_mouth == null) return false;
        if (_pattern == null) return false;
        if (_wing == null) return false;  
     
        return true;                      
    }


}
