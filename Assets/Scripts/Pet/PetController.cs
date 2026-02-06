using UnityEngine;

public sealed class PetController : MonoBehaviour
{
    private PetUnit _pet;
    private float _cleaningAccum = 0f;

    [Header("입 파츠")]
    [SerializeField] private SpriteRenderer _mouth;
    [Header("눈 파츠")]
    [SerializeField] private SpriteRenderer _eye;
    [Header("일반 입 벌림 스프라이트 ")]
    [SerializeField] private Sprite _openMouthSprite;
    [Header("간식 입 벌림 스프라이트 ")]
    [SerializeField] private Sprite _openMouthForSnackSprite;
    [Header("약 입 벌림 스프라이트 ")]
    [SerializeField] private Sprite _openMouthForMedicine;
    [Header("눈물 눈")]
    [SerializeField] private Sprite _closeEyesWithTear;
    [Header("감은 눈")]
    [SerializeField] private Sprite _closeEyesSprite;

    [SerializeField] private Animator _mouthAnim;

    private Sprite _ogMouth;
    private Sprite _ogEye;

    //상호작용 수치들 
    private float _mealFullnessGain;  //밥 먹으면 오르는 포만도 양
    private float _mealCleanlinessDecrease;  //밥 먹으면 내려가는 청결도 양
    private float _snackFullnessGain;  //간식 먹으면 오르는 포만도 양
    private float _snackExpGain;   //간식 먹으면 오르는 경험치 양
    private float _snackCleanlinessDecrease;  //간식 먹으면 내려가는 청결도 양
    private float _canFeedPetBelow;  //밥 먹일 수 있는 포만도 기준

    private void Awake()
    {
        _pet = GetComponent<PetUnit>();
    }
    private void Start()
    {
        GameConfig config = Manager.Game.Config;

        _mealFullnessGain = config.MealFullnessGain;
        _mealCleanlinessDecrease = config.MealCleanlinessDecrease;
        _snackFullnessGain = config.SnackFullnessGain;
        _snackExpGain = config.SnackExpGain;
        _snackCleanlinessDecrease = config.SnackCleanlinessDecrease;
        _canFeedPetBelow = config.CanFeedPetBelow;
    }

    public PetStatusCore Status
    {
        get { return _pet != null ? _pet.Status : null; }
    }

    public void FeedFood()
    {
        if (_pet == null || Status == null ) return;

        if (Status.Hunger > _canFeedPetBelow)
        {
            Debug.Log("이미 배부름");
            _mouthAnim.SetTrigger("Full");
            Manager.Audio.PlaySFXExclusive("Full"); // SFX Full Exclusive
            return;
        }

        Eat(_mealFullnessGain, _mealCleanlinessDecrease);
        Manager.Audio.PlaySFXExclusive("Feed"); // SFX Feed Exclusive
        Debug.Log($"밥먹음. 허기짐 : {Status.Hunger}, 청결도 : {Status.Cleanliness}");

        Debug.Log($"{Status.Hunger}, {_canFeedPetBelow}");
    }

    public void FeedSnack() //스낵 먹었을떄
    {
        if (_pet == null || Status == null) return;

        if (Status.Hunger > _canFeedPetBelow)
        {
            Debug.Log("이미 배부름");
            _mouthAnim.SetTrigger("Full");
            Manager.Audio.PlaySFXExclusive("Full"); // SFX Full Exclusive
            return;
        }

        _pet.Status.IncreaseEXP(_snackExpGain);
        Eat(_snackFullnessGain, _snackCleanlinessDecrease);
        Manager.Item.UseItem(RewardType.Snack, 1);
        Manager.Audio.PlaySFXExclusive("Feed"); // SFX Feed Exclusive
        Debug.Log($"스낵먹음. 허기짐 : {Status.Hunger}, 청결도 : {Status.Cleanliness}");
    }

    private void Eat(float Increasehunger, float decreaseCleanliness) // 오르는 포만도 수치, 감소하는 청결도 수치
    {
        Status.IncreaseStat(PetStat.Hunger, Increasehunger); //스낵 포만도 오르는 수치
        Status.DecreaseStat(PetStat.Cleanliness, decreaseCleanliness); //식사시 감소하는 청결도 수치
        _mouthAnim.SetTrigger("Eat");
        _pet.Petmanager.UpdateStatus();
    }
    //===================씻기기==========================
    public void Clean(float amount) 
    {
        _cleaningAccum += amount; //이동거리 누적

        if (_cleaningAccum >= 0.6f) //0.6 이상 문지르면
        {
            Status.IncreaseStat(PetStat.Cleanliness, 2f); //청결도 +2
            _pet.Petmanager.UpdateStatus(); //UI 갱신
            _cleaningAccum = 0f;  //리셋
        }
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
        Status.IncreaseStat(PetStat.Health, 10f); //치료시 증가하는 체력 수치
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
        else if (collision.CompareTag("CleaningTool"))
        {
            _ogEye = _eye.sprite;
            _eye.sprite = _closeEyesSprite;
            //Debug.Log("씻는중");
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
        else if (collision.CompareTag("CleaningTool"))
        {
            _eye.sprite = _ogEye;
            //Debug.Log("샤워도구 멀어짐");
        }
    }
}
