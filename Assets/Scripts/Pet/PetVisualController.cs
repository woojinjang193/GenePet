using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetVisualController : MonoBehaviour
{
    [SerializeField] private PetUnit _pet;

    [SerializeField] private SpriteRenderer _egg;

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

    //[SerializeField] private Color _color1;
    //[SerializeField] private Color _color2;

    [SerializeField] private Button _button; //테스트용. 지워야함

    public PetStatusCore Status
    {
        get { return _pet != null ? _pet.Status : null; }
    }

    private void Awake()
    {
        Status.OnGrowthChanged += OnGrowthChanged;
        _button.onClick.AddListener(ButtonClicked); //테스트용. 지워야함
    }

    private void ButtonClicked() //테스트용. 지워야함
    {
        LoadOwnPet();
        SetSprite(Status.Growth);
    }

    private void OnEnable()
    {
        if (Status != null)
        {
            Status.OnGrowthChanged += OnGrowthChanged;
            //SetSprite(Status.Growth);
        }
        else
        {
            Debug.LogWarning("스테이터스 없음");
        }
    }

    private void OnDisable()
    {
        if (Status != null)
        {
            Status.OnGrowthChanged -= OnGrowthChanged;
        }
    }

    private void LoadOwnPet()
    {
        if (!AreAllRenderersAssigned())
        {
            Debug.LogError("스프라이트 랜더러 없음");
            return;
        }
        var saveData = Manager.Save.CurrentData.UserData.HavePet;
        var pet = saveData.Genes;

        if (Enum.TryParse(saveData.GrowthStage, out GrowthStatus savedGrowth))
        {
            _pet.Status.Growth = savedGrowth;
        }

        _acc.sprite = Manager.Gene.GetPartSOByID<AccSO>(PartType.Acc, pet.Acc.DominantId).sprite;
        _arm.sprite = Manager.Gene.GetPartSOByID<ArmSO>(PartType.Arm, pet.Arm.DominantId).sprite;
        _blush.sprite = Manager.Gene.GetPartSOByID<BlushSO>(PartType.Blush, pet.Blush.DominantId).sprite;
        _body.sprite = Manager.Gene.GetPartSOByID<BodySO>(PartType.Body, pet.Body.DominantId).sprite;
        _pattern.sprite = Manager.Gene.GetPartSOByID<PatternSO>(PartType.Pattern, pet.Pattern.DominantId).sprite;
        _ear.sprite = Manager.Gene.GetPartSOByID<EarSO>(PartType.Ear, pet.Ear.DominantId).sprite;
        _eye.sprite = Manager.Gene.GetPartSOByID<EyeSO>(PartType.Eye, pet.Eye.DominantId).sprite;
        _feet.sprite = Manager.Gene.GetPartSOByID<FeetSO>(PartType.Feet, pet.Feet.DominantId).sprite;
        _mouth.sprite = Manager.Gene.GetPartSOByID<MouthSO>(PartType.Mouth, pet.Mouth.DominantId).sprite;
        _wing.sprite = Manager.Gene.GetPartSOByID<WingSO>(PartType.Wing, pet.Wing.DominantId).sprite;
        
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
            return;
        }

        HideAllParts();

        if (growth == GrowthStatus.Egg) //알일때
        {
            _egg.gameObject.SetActive(true);
            return;
        }

        if (growth == GrowthStatus.Baby) //애기일때
        {
            _eye.gameObject.SetActive(true);
            _body.gameObject.SetActive(true);
            _blush.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(false); //고민중

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
            Debug.Log("Teen 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Teen_Rebel) //반항기
        {
            _acc.gameObject.SetActive(true);
            _blush.gameObject.SetActive(true);
            _body.gameObject.SetActive(true);
            _ear.gameObject.SetActive(true);
            _eye.gameObject.SetActive(true);
            _feet.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            Debug.Log("Teen_Rebel 상태 스프라이트 세팅");
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
