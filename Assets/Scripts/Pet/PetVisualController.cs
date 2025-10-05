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

    private void Init()
    {
        _body.sprite = _genSO.bodyDominant.sprite;
        _pattern.sprite = _genSO.patternDominant.sprite;
        _ear.sprite = _genSO.EarDominant.sprite;
        _tail.sprite = _genSO.tailDominant.sprite;
        _wing.sprite = _genSO.wingDominant.sprite;
        _eye.sprite = _genSO.eyeDominant.sprite;
        _mouth.sprite = _genSO.mouthDominant.sprite;
        _horn.sprite = _genSO.hornDominant.sprite;
        _color1 = _genSO.colorDominant.color;
        _color2 = _genSO.colorRecessive.color;

        _body.color = _color1;
        _pattern.color = _color2;
        _ear.color = _color1;
        _tail.color = _color1;

        _egg.gameObject.SetActive(true);
        _body.gameObject.SetActive(false);
        _pattern.gameObject.SetActive(false);
        _ear.gameObject.SetActive(false);
        _tail.gameObject.SetActive(false);
        _wing.gameObject.SetActive(false);
        _eye.gameObject.SetActive(false);
        _mouth.gameObject.SetActive(false);
        _horn.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Status.OnGrowthChanged += OnGrowthChanged;
        SetSprite(Status.Growth);
    }

    private void OnDisable()
    {
        Status.OnGrowthChanged -= OnGrowthChanged;
    }

    private void OnGrowthChanged(GrowthStatus growth)
    {
        SetSprite(growth);
    }

    private void SetSprite(GrowthStatus growth)
    {
        if (growth == GrowthStatus.Baby)
        {
            _body.gameObject.SetActive(true);
            _pattern.gameObject.SetActive(false);
            _ear.gameObject.SetActive(false);
            _tail.gameObject.SetActive(false);
            _wing.gameObject.SetActive(false);
            _eye.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(false);
            _horn.gameObject.SetActive(false);
        }
        else if (growth == GrowthStatus.Teen)
        {
            _body.gameObject.SetActive(true);
            _pattern.gameObject.SetActive(false);
            _ear.gameObject.SetActive(false);
            _tail.gameObject.SetActive(true);
            _wing.gameObject.SetActive(false);
            _eye.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            _horn.gameObject.SetActive(false);
        }
        else if (growth == GrowthStatus.Teen_Rebel)
        {
            _body.gameObject.SetActive(true);
            _pattern.gameObject.SetActive(false);
            _ear.gameObject.SetActive(false);
            _tail.gameObject.SetActive(true);
            _wing.gameObject.SetActive(false);
            _eye.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            _horn.gameObject.SetActive(false);
        }
        else if (growth == GrowthStatus.Adult)
        {
            _body.gameObject.SetActive(true);
            _pattern.gameObject.SetActive(true);
            _ear.gameObject.SetActive(true);
            _tail.gameObject.SetActive(true);
            _wing.gameObject.SetActive(true);
            _eye.gameObject.SetActive(true);
            _mouth.gameObject.SetActive(true);
            _horn.gameObject.SetActive(true);
        }
    }
}
