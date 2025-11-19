using UnityEngine;

public sealed class PetController : MonoBehaviour
{
    private PetUnit _pet;

    [Header("입")]
    [SerializeField] private SpriteRenderer _mouth;
    [SerializeField] private SpriteRenderer _eye;
    [SerializeField] private Sprite _openMouthSprite;
    [SerializeField] private Sprite _openMouthForMedicine;
    [SerializeField] private Sprite _closeEyesWithTear;
    [SerializeField] private Sprite _closeEyesSprite;

    [SerializeField] private Animator _mouthAnim;

    private Sprite _ogMouth;
    private Sprite _ogEye;

    private PetManager _petManager;

    private void Awake()
    {
        _pet = GetComponent<PetUnit>();
        _petManager = FindObjectOfType<PetManager>();
    }

    public PetStatusCore Status
    {
        get { return _pet != null ? _pet.Status : null; }
    }
    public void OnMouseDown()
    {
        if(_petManager == null) return;
        _petManager.ZoomInPet(_pet.Status.ID);
        
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
        Status.DecreaseStat(PetStat.Cleanliness, -5f); //식사시 감소하는 청결도 수치
        _mouthAnim.SetTrigger("Eat");
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
        Debug.Log($"아픔 : {Status.IsSick}");
    }
    public void ShowStatus()
    {
        if (Status == null)
        {
            Debug.LogError("스테이터스 없음");
            return;
        }

        string msg =
            "건강: " + Status.Health.ToString("F1") +
            ", 포만: " + Status.Hunger.ToString("F1") +
            ", 행복: " + Status.Happiness.ToString("F1") +
            ", 청결: " + Status.Cleanliness.ToString("F1") +
            ", 아픔: " + (Status.IsSick ? "T" : "F");
        Debug.Log("[펫 스테이터스] " + msg);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            _ogMouth = _mouth.sprite;
            _ogEye = _eye.sprite;
            _mouth.sprite = _openMouthSprite;
            _eye.sprite = _closeEyesSprite;
            Debug.Log("음식 트리거 충돌");
        }
        else if (collision.CompareTag("Snack"))
        {
            _ogMouth = _mouth.sprite;
            _ogEye = _eye.sprite;
            _mouth.sprite = _openMouthSprite;
            _eye.sprite = _closeEyesSprite;
            Debug.Log("간식 트리거 충돌");

        }
        else if (collision.CompareTag("Medicine"))
        {
            _ogMouth = _mouth.sprite;
            _ogEye = _eye.sprite;
            _mouth.sprite = _openMouthForMedicine;
            _eye.sprite = _closeEyesWithTear;
            Debug.Log("약 트리거 충돌");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            _mouth.sprite = _ogMouth;
            _eye.sprite = _ogEye;
            Debug.Log("음식 멀어짐");
        }
        else if (collision.CompareTag("Snack"))
        {
            _mouth.sprite = _ogMouth;
            _eye.sprite = _ogEye;
            Debug.Log("간식 멀어짐");

        }
        else if (collision.CompareTag("Medicine"))
        {
            _mouth.sprite = _ogMouth;
            _eye.sprite = _ogEye;
            Debug.Log("약 멀어짐");
        }
    }
}
