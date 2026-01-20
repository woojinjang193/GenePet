using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RythmVisualController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmPresenter _presenter;
    [SerializeField] private RythmScoring _scoring;
    [SerializeField] private RythmRewardPlanner _rewardPlanner;

    [Header("연출 대상")]
    [SerializeField] private BobberVisual _bobber; // 찌
    [SerializeField] private RodHandVisual _rodHand; // 낚싯대+손
    [Header("프리롤(조금 미리 시작하고 싶으면)")]
    [SerializeField] private double _visualLeadTime = 0.00; // 0이면 정확히 그 타이밍

    [Header("리워드 아이콘")]
    [SerializeField] private SpriteRenderer _rewardIcon;
    [SerializeField] private TMP_Text _rewardAmount;

    [Header("패턴 성공/실패 연출")]
    [SerializeField] private GameObject _successParticle;
    [SerializeField] private GameObject _failParticle;
    [SerializeField] private float _particleLifeTime;

    [Header("판정 글자 연출")]
    [SerializeField] private GameObject _judgeLetterPrefab;
    [SerializeField] private Transform _judgeLetterTransform;

    // 샘플 비트 실행 예약(절대시간 DSP)
    private readonly Queue<double> _sampleBeatQueue = new();
    private readonly Queue<float> _sampleBeatDurationQueue = new(); //beatDuration 같이 저장해야 BPM에 맞춰 속도 조절 가능

    private Vector3 _judgeLetterVector;


    //================초기화=================
    private void Awake()
    {
        if (_presenter != null)
            _presenter.OnSampleBeatScheduled += EnqueueSampleBeat;

        if (_scoring != null)
            _scoring.OnJudged += HandleJudgeResult; // 입력 시점 연출(즉시)

        if (_rewardPlanner != null)
            _rewardPlanner.OnGiveReward += HandleGiveReward;

        _rewardIcon.gameObject.SetActive(false); //리워드 오브젝트 비활성화

        _judgeLetterVector = _judgeLetterTransform.position;
    }

    private void OnDestroy()
    {
        if (_presenter != null)
            _presenter.OnSampleBeatScheduled -= EnqueueSampleBeat;

        if (_scoring != null)
            _scoring.OnJudged -= HandleJudgeResult;

        if (_rewardPlanner != null)
            _rewardPlanner.OnGiveReward -= HandleGiveReward;
    }

    private void EnqueueSampleBeat(double beatDsp, float beatDuration)
    {
        _sampleBeatQueue.Enqueue(beatDsp);
        _sampleBeatDurationQueue.Enqueue(beatDuration);
    }
    //================찌 연출=================
    private void Update()
    {
        // 샘플 비트 타이밍 도달하면 찌 연출 실행
        double now = AudioSettings.dspTime;

        while (_sampleBeatQueue.Count > 0)
        {
            double next = _sampleBeatQueue.Peek();

            if (now < next - _visualLeadTime) break;

            _sampleBeatQueue.Dequeue();

            float beatDuration = 0.25f; //안전 기본값
            if (_sampleBeatDurationQueue.Count > 0) beatDuration = _sampleBeatDurationQueue.Dequeue();

            if (_bobber != null) _bobber.PulseDown(beatDuration); // 찌 아래로
        }
    }
    //================획득 아이템 보여줌=================
    private void HandleGiveReward(RewardType reward, int amount, bool isLast)
    {
        ShowItem(reward, amount);

    }
    public void ShowItem(RewardType reward, int amount)
    {
        if (Manager.Item != null)
        {
            _rewardIcon.sprite = Manager.Item.ItemImages.GetItemSprite(reward);
        }
        else
        {
            _rewardIcon.sprite = null;
        }
    
        _rewardAmount.text = $"x{amount.ToString()}";
    
        _rewardIcon.gameObject.SetActive(true);
    }
    //================판정에 따른 연출=================
    public void HandleJudgeResult(JudgeResult result)
    {
        GameObject letterOBJ = Manager.Pool.Get(_judgeLetterPrefab, _judgeLetterVector, _judgeLetterTransform);
        var go = letterOBJ.GetComponent<RythmJudgeLetter>();
        if(go != null)
        {
            go.OnFinishedMoving += ReturnLetter;
            go.Init(result);
        }

        // 입력 순간(HandlePlayerInput에서 판정 난 시점) : 손/낚싯대 위로
        if (_rodHand != null) _rodHand.PullUp(result);
    }
    //======패턴 성공/실패여부에 따른 연출=============
    public void PatternSuccess(bool success)
    {
        //-- 성공여부에 따라 파티클 on/off--
        GameObject particle = success ? _successParticle : _failParticle;
        particle.SetActive(true); 
        StartCoroutine(ParticleOffDelay(particle));

        //----성공/실패 연출-----


    }
    IEnumerator ParticleOffDelay(GameObject particle) //파티클 자동끄기 코루틴
    {
        yield return new WaitForSeconds(_particleLifeTime);
        particle.SetActive(false);
    }
    private void ReturnLetter(RythmJudgeLetter letter)
    {
        letter.OnFinishedMoving -= ReturnLetter;
        Manager.Pool.Release(letter.gameObject);
    }
}
