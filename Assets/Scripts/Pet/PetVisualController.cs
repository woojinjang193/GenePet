using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetVisualController : MonoBehaviour
{
    [SerializeField] private PetUnit _pet;

    [SerializeField] private GeneticInfoSO _genSO;

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

    public PetStatusCore Status
    {
        get { return _pet != null ? _pet.Status : null; }
    }

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        if (Status != null)
        {
            Status.OnGrowthChanged += OnGrowthChanged;
            SetSprite(Status.Growth);
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

    private void Init()
    {
        if (_genSO == null)
        {
            Debug.LogError("GeneticInfoSO 없음");
            return;
        }

        if (!AreAllRenderersAssigned())
        {
            Debug.LogError("스프라이트 랜더러 없음");
            return;
        }

        _body.sprite = _genSO.bodyDominant != null ? _genSO.bodyDominant.sprite : null;
        _pattern.sprite = _genSO.patternDominant != null ? _genSO.patternDominant.sprite : null;
        _ear.sprite = _genSO.EarDominant != null ? _genSO.EarDominant.sprite : null;
        _tail.sprite = _genSO.tailDominant != null ? _genSO.tailDominant.sprite : null;
        _wing.sprite = _genSO.wingDominant != null ? _genSO.wingDominant.sprite : null;
        _eye.sprite = _genSO.eyeDominant != null ? _genSO.eyeDominant.sprite : null;
        _mouth.sprite = _genSO.mouthDominant != null ? _genSO.mouthDominant.sprite : null;
        _horn.sprite = _genSO.hornDominant != null ? _genSO.hornDominant.sprite : null;

        _color1 = _genSO.colorDominant != null ? _genSO.colorDominant.color : Color.white;
        _color2 = _genSO.colorRecessive != null ? _genSO.colorRecessive.color : Color.white;

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
        _body.color = _color1;
        _pattern.color = _color2;
        _ear.color = _color1;
        _tail.color = _color1;
        _wing.color = _color1;

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
