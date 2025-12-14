using UnityEngine;

public class PetVisualController : MonoBehaviour
{
    private PetUnit _pet;

    [Header("알")]
    [SerializeField] private SpriteRenderer _egg;

    [Header("편지")]
    [SerializeField] private Letter _letter;

    [Header("파츠")]
    [SerializeField] private PetPartSpriteList _renderers;

    [Header("더러움")]
    [SerializeField] private SpriteRenderer _dirtRenderer;
    [SerializeField] private SpriteMask _dirtMask;
    [SerializeField] private Sprite _dirtLow;
    [SerializeField] private Sprite _dirtMid;
    [SerializeField] private Sprite _dirtHigh;

    [Header("아픔/ 체력감소")]
    [SerializeField] private GameObject _sickImage;
    [SerializeField] private GameObject _healthReducingParticle;

    public void Init(PetSaveData save, PetUnit unit)
    {
        if (_letter.gameObject.activeSelf) { _letter.gameObject.SetActive(false); }
        _egg.sprite = save.EggSprite;
        _pet = unit;
        ApplyVisual(save.Genes); //비주얼 로더
        SetSprite(_pet.Status.Growth);

        OnSick(_pet.Status.IsSick); //아픈상태면 아픔 이미지 켜줌
    }
    private void ApplyVisual(GenesContainer save)
    {
        PetVisualHelper.ApplyVisual(save, _renderers);
        _dirtMask.sprite = _renderers.Body.sprite; // 얼룩 마스크 설정
        _dirtRenderer.sortingOrder = 8; //얼룩 레이어 오더 설정
    }

    public void SetSprite(GrowthStatus growth) //스프라이트 끄고킴
    {
        HideAllParts();

        if (growth == GrowthStatus.Egg) //알일때
        {
            _egg.gameObject.SetActive(true);

            Debug.Log("Egg 상태 스프라이트 세팅");
            return;
        }

        if (growth == GrowthStatus.Baby) //애기일때
        {
            _renderers.Eye.gameObject.SetActive(true);
            _renderers.Body.gameObject.SetActive(true);
            _renderers.Ear.gameObject.SetActive(true);
            _renderers.Blush.gameObject.SetActive(true);
            _renderers.Mouth.gameObject.SetActive(true);
            _renderers.Tail.gameObject.SetActive(true);

            _renderers.BodyOut.gameObject.SetActive(true);
            _renderers.EarOut.gameObject.SetActive(true);
            _renderers.TailOut.gameObject.SetActive(true);

            _dirtRenderer.gameObject.SetActive(true);

            Debug.Log("Baby 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Teen) //성장기
        {
            _renderers.Blush.gameObject.SetActive(true);
            _renderers.Body.gameObject.SetActive(true);
            _renderers.Ear.gameObject.SetActive(true);
            _renderers.Eye.gameObject.SetActive(true);
            _renderers.Feet.gameObject.SetActive(true);
            _renderers.Mouth.gameObject.SetActive(true);
            _renderers.Tail.gameObject.SetActive(true);
            _renderers.Whiskers.gameObject.SetActive(true);

            _renderers.BodyOut.gameObject.SetActive(true);
            _renderers.EarOut.gameObject.SetActive(true);
            _renderers.FeetOut.gameObject.SetActive(true);
            _renderers.TailOut.gameObject.SetActive(true);

            _dirtRenderer.gameObject.SetActive(true);
            Debug.Log("Teen 상태 스프라이트 세팅");
        }
        else if (growth == GrowthStatus.Adult) //어른
        {
            _renderers.Acc.gameObject.SetActive(true);
            _renderers.Arm.gameObject.SetActive(true);
            _renderers.Blush.gameObject.SetActive(true);
            _renderers.Body.gameObject.SetActive(true);
            _renderers.Ear.gameObject.SetActive(true);
            _renderers.Eye.gameObject.SetActive(true);
            _renderers.Feet.gameObject.SetActive(true);
            _renderers.Mouth.gameObject.SetActive(true);
            _renderers.Pattern.gameObject.SetActive(true);
            _renderers.Wing.gameObject.SetActive(true);
            _renderers.Tail.gameObject.SetActive(true);
            _renderers.Whiskers.gameObject.SetActive(true);

            _renderers.ArmOut.gameObject.SetActive(true);
            _renderers.BodyOut.gameObject.SetActive(true);
            _renderers.EarOut.gameObject.SetActive(true);
            _renderers.FeetOut.gameObject.SetActive(true);
            _renderers.WingOut.gameObject.SetActive(true);
            _renderers.TailOut.gameObject.SetActive(true);

            _dirtRenderer.gameObject.SetActive(true);
            Debug.Log("Adult 상태 스프라이트 세팅");
        }
        else
        {
            Debug.LogWarning("성장상태 이상함 확인 해야함.");
        }

        _egg.gameObject.SetActive(false);
    }

    private void HideAllParts()
    {
        if (_egg != null) _egg.gameObject.SetActive(false);
        _letter.gameObject.SetActive(false);
        // 베이스 끄기
        _renderers.Acc?.gameObject.SetActive(false);
        _renderers.Arm?.gameObject.SetActive(false);
        _renderers.Blush?.gameObject.SetActive(false);
        _renderers.Body?.gameObject.SetActive(false);
        _renderers.Ear?.gameObject.SetActive(false);
        _renderers.Eye?.gameObject.SetActive(false);
        _renderers.Feet?.gameObject.SetActive(false);
        _renderers.Mouth?.gameObject.SetActive(false);
        _renderers.Pattern?.gameObject.SetActive(false);
        _renderers.Wing?.gameObject.SetActive(false);
        _renderers.Tail?.gameObject.SetActive(false);
        _renderers.Whiskers?.gameObject.SetActive(false);
        _dirtRenderer?.gameObject.SetActive(false);

        // 아웃라인 끄기
        _renderers.ArmOut?.gameObject.SetActive(false);
        _renderers.BodyOut?.gameObject.SetActive(false);
        _renderers.EarOut?.gameObject.SetActive(false);
        _renderers.FeetOut?.gameObject.SetActive(false);
        _renderers.WingOut?.gameObject.SetActive(false);
        _renderers.TailOut?.gameObject.SetActive(false);
    }
    public void LetterOn(LeftReason reason)
    {
        HideAllParts();
        _letter.gameObject.SetActive(true);
        _letter.Init(_pet, reason);
    }
    public void AllowToClickLetter(bool on)
    {
        if(_letter.gameObject.activeSelf)
        {
            //Debug.Log($"편지한테 알려줌 {on}");
            _letter.SetClickable(on);
        }
    }
    public void OnCleanlinessChanged(float newValue)
    {
        if(newValue < 10f)
        {
            if(_dirtRenderer.sprite != _dirtHigh)
            {
                _dirtRenderer.sprite = _dirtHigh;
            }   
        }
        else if(newValue < 30f)
        {
            if (_dirtRenderer.sprite != _dirtMid)
            {
                _dirtRenderer.sprite = _dirtMid;
            }
        }
        else if(newValue < 50f)
        {
            if (_dirtRenderer.sprite != _dirtLow)
            {
                _dirtRenderer.sprite = _dirtLow;
            }
        }
        else
        {
            if (_dirtRenderer.sprite != null)
            {
                _dirtRenderer.sprite = null;
            }
        }
    }

    public void OnSick(bool on)
    {
        _sickImage.SetActive(on);
        Debug.Log($"아픔이미지: {on}");
    }

    public void OnHealthReducing(bool on)
    {
        _healthReducingParticle.SetActive(on);
        Debug.Log($"체력감소 파티클: {on}");
    }
}
