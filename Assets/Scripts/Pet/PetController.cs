using UnityEngine;

public sealed class PetController : MonoBehaviour
{
    private PetUnit _pet;

    [Header("입 파츠")]
    [SerializeField] private SpriteRenderer _mouth;
    [Header("눈 파츠")]
    [SerializeField] private SpriteRenderer _eye;
    [Header("일반 입 벌림 스프라이트 ")]
    [SerializeField] private Sprite _openMouthSprite;
    [Header("간식 입 벌림 스프라이트 ")]
    [SerializeField] private Sprite _openMouthForSnackSprite;
    [Header("씹는 입 스프라이트 ")]
    [SerializeField] private Sprite _chewingMouthSprite;
    [Header("약 입 벌림 스프라이트 ")]
    [SerializeField] private Sprite _openMouthForMedicine;
    [Header("눈물 눈")]
    [SerializeField] private Sprite _closeEyesWithTear;
    [Header("감은 눈")]
    [SerializeField] private Sprite _closeEyesSprite;

    [SerializeField] private Animator _mouthAnim;

    private Sprite _ogMouth;
    private Sprite _ogEye;

    private void Awake()
    {
        _pet = GetComponent<PetUnit>();
    }

    public PetStatusCore Status
    {
        get { return _pet != null ? _pet.Status : null; }
    }

    public void Feed()
    {
        if (_pet == null || Status == null ) return;

        if (Status.Hunger > 95f)
        {
            Debug.Log("이미 배부름");
            _mouthAnim.SetTrigger("Full");
            return;
        }
        
        Status.IncreaseStat(PetStat.Hunger, 10f); //식사 포만도 오르는 수치
        Status.DecreaseStat(PetStat.Cleanliness, 5f); //식사시 감소하는 청결도 수치
        _mouthAnim.SetTrigger("Eat");
        _pet.Petmanager.UpdateStatus();
        Debug.Log($"밥먹음. 허기짐 : {Status.Hunger}, 청결도 : {Status.Cleanliness}");
    }

    public void Play()
    {
        //미니게임 실행
    }

    public void Clean()
    {
        if (_pet == null || Status == null) return;
        Status.IncreaseStat(PetStat.Cleanliness, 50f); // 목욕시 증가하는 청결도 수치
        _pet.Petmanager.UpdateStatus();
        Debug.Log($"목욕. 청결도 : {Status.Cleanliness}");
    }
    public void Heal()
    {
        if (_pet == null || Status == null) return;

        bool isSick = Status.IsSick;
        if (!isSick)
        {
            Debug.Log("안아픔");
            return;
        }
        
        Status.SetFlag(PetFlag.IsSick, false);
        Status.IncreaseStat(PetStat.Health, 20f); //치료시 증가하는 체력 수치
        _pet.Petmanager.UpdateStatus();
        Debug.Log($"아픔 : {Status.IsSick}");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            _ogMouth = _mouth.sprite;
            _ogEye = _eye.sprite;
            _mouth.sprite = _openMouthSprite;
            _eye.sprite = _closeEyesSprite;
            //Debug.Log("음식 트리거 충돌");
        }
        else if (collision.CompareTag("Snack"))
        {
            _ogMouth = _mouth.sprite;
            _ogEye = _eye.sprite;
            _mouth.sprite = _openMouthForSnackSprite;
            _eye.sprite = _closeEyesSprite;
            //Debug.Log("간식 트리거 충돌");

        }
        else if (collision.CompareTag("Medicine"))
        {
            _ogMouth = _mouth.sprite;
            _ogEye = _eye.sprite;
            _mouth.sprite = _openMouthForMedicine;
            _eye.sprite = _closeEyesWithTear;
            //Debug.Log("약 트리거 충돌");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            _mouth.sprite = _ogMouth;
            _eye.sprite = _ogEye;
            //Debug.Log("음식 멀어짐");
        }
        else if (collision.CompareTag("Snack"))
        {
            _mouth.sprite = _ogMouth;
            _eye.sprite = _ogEye;
            //Debug.Log("간식 멀어짐");

        }
        else if (collision.CompareTag("Medicine"))
        {
            _mouth.sprite = _ogMouth;
            _eye.sprite = _ogEye;
            //Debug.Log("약 멀어짐");
        }
    }
}
