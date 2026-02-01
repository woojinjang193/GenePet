using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PetVisualController : MonoBehaviour
{
    private PetUnit _pet;

    [Header("참조")]
    [SerializeField] private PetPetting _petting;

    [Header("알")]
    [SerializeField] private SpriteRenderer _egg;

    [Header("편지")]
    [SerializeField] private Letter _letter;

    [Header("파츠")]
    [SerializeField] private PetPartSpriteList _renderers;

    [Header("웃는 눈 스프라이트")]
    [SerializeField] private Sprite _smileEye;

    [Header("성장 파티클")]
    [SerializeField] private ParticleSystem _growParticle;

    [Header("더러움")]
    [SerializeField] private SpriteRenderer _dirtRenderer;
    [SerializeField] private SpriteMask _dirtMask;
    [SerializeField] private Sprite _dirtLow;
    [SerializeField] private Sprite _dirtMid;
    [SerializeField] private Sprite _dirtHigh;

    [Header("쓰다듬 파티클")]
    [SerializeField] private ParticleSystem _pettingParticle;

    [Header("아픔/ 체력감소")]
    [SerializeField] private GameObject _sickImage;
    [SerializeField] private GameObject _healthReducingParticle;

    private bool _isZoomed;
    private Sprite _ogEye;

    private void Awake()
    {
        _petting.OnPettingChanged += OnPettingPet;
    }
    private void OnDestroy()
    {
        _petting.OnPettingChanged -= OnPettingPet;
    }
    private void OnEnable()
    {
        _pettingParticle.Stop();
    }
    public void Init(PetSaveData save, PetUnit unit)
    {
        if (_letter.gameObject.activeSelf) { _letter.gameObject.SetActive(false); }

        RarityType rarity = save.Rarity;
        _egg.sprite = Manager.Item.ItemImages.EggRaritySO.GetEggSprite(rarity);
        _pet = unit;
        ApplyVisual(save.Genes); //비주얼 로더
        SetSprite(_pet.Status.Growth);

        OnSick(_pet.Status.IsSick); //아픈상태면 아픔 이미지 켜줌

        _ogEye = _renderers.Eye.sprite;
    }
    private void ApplyVisual(GenesContainer save)
    {
        PetVisualHelper.ApplyVisual(save, _renderers);
        _dirtMask.sprite = _renderers.Body.sprite; // 얼룩 마스크 설정
        _dirtRenderer.sortingOrder = 8; //얼룩 레이어 오더 설정
    }

    public void SetSprite(GrowthStatus growth) //스프라이트 끄고킴
    {
        Debug.Log($"[SetSprite] cleanliness={_pet.Status.Cleanliness}");

        if (growth == GrowthStatus.Egg) //알일때
        {
            _egg.gameObject.SetActive(true);
            _dirtRenderer.gameObject.SetActive(false); //알일땐 더러움 꺼두기
            Debug.Log("Egg 상태 스프라이트 세팅");
            return;
        }

        PetVisualHelper.SetSpriteByGrowth(_renderers, growth); //파츠 세팅

        _dirtRenderer.gameObject.SetActive(true); //더러움 켜줌
        _letter.gameObject.SetActive(false); //편지 꺼줌
        _egg.gameObject.SetActive(false); //알 꺼줌

        if (_pet.Status.IsSick) _sickImage.SetActive(true); //아픈상태면 이미지 켜줌

        OnCleanlinessChanged(_pet.Status.Cleanliness);//더러움 설정
    }

    private void HideAllParts()
    {
        if (_egg != null) _egg.gameObject.SetActive(false); //알 끄기
        _letter.gameObject.SetActive(false); //편지 끄기

        PetVisualHelper.ActiveFalseAll(_renderers); //파츠 다 꺼줌

        _dirtRenderer.gameObject.SetActive(false); //더러움 꺼줌
    }
    //편지 켜줌
    public void LetterOn(LeftReason reason)
    {
        HideAllParts();

        _sickImage.SetActive(false);
        _healthReducingParticle.SetActive(false);

        _letter.gameObject.SetActive(true);
        _letter.Init(_pet, reason);

        _letter.SetClickable(_isZoomed); //현재 줌 상태 기준으로 클릭 허용
    }

    public void AllowToClickLetter(bool on)
    {
        _isZoomed = on; //줌인/줌아웃 상태 기억

        if (_letter.gameObject.activeSelf)
        {
            //Debug.Log($"편지한테 알려줌 {on}");
            _letter.SetClickable(on);
        }
    }
    //===============청결도 업데이트 이벤트====================
    public void OnCleanlinessChanged(float newValue)
    {

        if (newValue <= 10f)
        {
            if(_dirtRenderer.sprite != _dirtHigh)
            {
                _dirtRenderer.sprite = _dirtHigh;
            }   
        }
        else if(newValue <= 30f)
        {
            if (_dirtRenderer.sprite != _dirtMid)
            {
                _dirtRenderer.sprite = _dirtMid;
            }
        }
        else if(newValue <= 50f)
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

    //================= 쓰다듬기 이벤트 ==================
    public void OnPettingPet(bool on)
    {
        if (_pet.Status.Growth == GrowthStatus.Egg) return; //알이면 리턴
        if (_pet.Status.IsLeft) return; //떠난 상태면 리턴

        if (_pettingParticle == null) return; //  null 방지

        if (on == true)
        {
            _renderers.Eye.sprite = _smileEye;
            _pettingParticle.Play();
        }
        else
        {
            _renderers.Eye.sprite = _ogEye;
            _pettingParticle.Stop();
        }
    }
    //===============성장 파티클 이벤트=================================
    public void OnGrown(GrowthStatus newGrowth)
    {
        if (_growParticle != null) _growParticle.Emit(30);
    }
}
