using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetVisualController : MonoBehaviour
{
    [SerializeField] private PetUnit _pet;

    [SerializeField] private SpriteRenderer _egg;
    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer _pattern;
    [SerializeField] private SpriteRenderer _ear;
    [SerializeField] private SpriteRenderer _tail;
    [SerializeField] private SpriteRenderer _wing;
    [SerializeField] private SpriteRenderer _eye;
    [SerializeField] private SpriteRenderer _mouth;
    [SerializeField] private SpriteRenderer _horn;

    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;

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
    private void Start()
    {
        
    }

    private void ButtonClicked() //테스트용. 지워야함
    {
        LoadPet();
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

    private void LoadPet()
    {

        if (!AreAllRenderersAssigned())
        {
            Debug.LogError("스프라이트 랜더러 없음");
            return;
        }
        var saveData = Manager.Save.CurrentData.UserData.HavePetList[0];
        var pet = saveData.Genes;

        if (Enum.TryParse(saveData.GrowthStage, out GrowthStatus savedGrowth))
        {
            _pet.Status.Growth = savedGrowth;
        }

        _body.sprite = Manager.Gene.GetPartSOByID<BodySO>(PartType.Body, pet.Body.DominantId).sprite;
        _pattern.sprite = Manager.Gene.GetPartSOByID<PatternSO>(PartType.Pattern, pet.Pattern.DominantId).sprite;
        _ear.sprite = Manager.Gene.GetPartSOByID<EarSO>(PartType.Ear, pet.Ear.DominantId).sprite;
        _tail.sprite = Manager.Gene.GetPartSOByID<TailSO>(PartType.Tail, pet.Tail.DominantId).sprite;
        _wing.sprite = Manager.Gene.GetPartSOByID<WingSO>(PartType.Wing, pet.Wing.DominantId).sprite;
        _eye.sprite = Manager.Gene.GetPartSOByID<EyeSO>(PartType.Eye, pet.Eye.DominantId).sprite;
        _mouth.sprite = Manager.Gene.GetPartSOByID<MouthSO>(PartType.Mouth, pet.Mouth.DominantId).sprite;
        _horn.sprite = Manager.Gene.GetPartSOByID<HornSO>(PartType.Horn, pet.Horn.DominantId).sprite;

        _color1 = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, pet.Color.DominantId).color;
        _color2 = Manager.Gene.GetPartSOByID<ColorSO>(PartType.Color, pet.Color.RecessiveId).color;

        ApplyColors();
    }

    private void OnGrowthChanged(GrowthStatus growth)
    {
        SetSprite(growth);
    }

    private void SetSprite(GrowthStatus growth)
    {
        if (!AreAllRenderersAssigned())
        {
            return;
        }

        HideAllParts();

        if (growth == GrowthStatus.Egg)
        {
            _egg.gameObject.SetActive(true);
            return;
        }

        _body.gameObject.SetActive(true);
        _eye.gameObject.SetActive(true);

        if (growth == GrowthStatus.Baby)
        {
            Debug.Log("Baby 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Teen)
        {
            _tail.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            Debug.Log("Teen 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Teen_Rebel)
        {
            _tail.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            Debug.Log("Teen_Rebel 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Adult)
        {
            _pattern.gameObject.SetActive(true);
            _ear.gameObject.SetActive(true);
            _tail.gameObject.SetActive(true);
            _wing.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            _horn.gameObject.SetActive(true);
            Debug.Log("Adult 상태 스프라이트 세팅");
        }
        else
        {
            Debug.LogWarning("성장상태 이상함 확인 해야함.");
        }

        _egg.gameObject.SetActive(false);
    }

    private void ApplyColors()
    {
        Color[] colors = { _color1, _color2 };

        _body.color = colors[UnityEngine.Random.Range(0, colors.Length)];
        _pattern.color = colors[UnityEngine.Random.Range(0, colors.Length)];
        _ear.color = colors[UnityEngine.Random.Range(0, colors.Length)];
        _tail.color = colors[UnityEngine.Random.Range(0, colors.Length)];
        _wing.color = colors[UnityEngine.Random.Range(0, colors.Length)];

        //색 적용 안함
        _eye.color = Color.white;
        _mouth.color = Color.white;
        _horn.color = Color.white;
    }

    private void HideAllParts()                                    
    {
        if (_egg != null) _egg.gameObject.SetActive(false);        
        if (_body != null) _body.gameObject.SetActive(false);      
        if (_pattern != null) _pattern.gameObject.SetActive(false);
        if (_ear != null) _ear.gameObject.SetActive(false);        
        if (_tail != null) _tail.gameObject.SetActive(false);      
        if (_wing != null) _wing.gameObject.SetActive(false);      
        if (_eye != null) _eye.gameObject.SetActive(false);        
        if (_mouth != null) _mouth.gameObject.SetActive(false);    
        if (_horn != null) _horn.gameObject.SetActive(false);      
    }

    private bool AreAllRenderersAssigned()
    {
        if (_egg == null) return false;   
        if (_body == null) return false;
        if (_pattern == null) return false;
        if (_ear == null) return false;   
        if (_tail == null) return false;  
        if (_wing == null) return false;  
        if (_eye == null) return false;   
        if (_mouth == null) return false; 
        if (_horn == null) return false;  
        return true;                      
    }
}
