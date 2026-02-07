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
    [SerializeField] private RythmFlowController _flow; // 샘플턴/입력턴 이벤트 받기용

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

    [Header("물고기 스프라이트")]
    [SerializeField] private Sprite _fishSprite;

    [Header("턴 표시 스프라이트")] 
    [SerializeField] private SpriteRenderer _turnSpriteRenderer; //턴 표시용 스프라이트 렌더러
    [SerializeField] private Sprite _sampleTurnSprite;    //샘플턴 스프라이트
    [SerializeField] private Sprite _inputTurnSprite;    //입력턴 스프라이트
    [SerializeField] private Sprite _intermissionSprite; //공백/대기 상태 스프라이트

    // 샘플 비트 실행 예약(절대시간 DSP)
    private readonly Queue<double> _sampleBeatQueue = new();
    private readonly Queue<float> _sampleBeatDurationQueue = new(); //beatDuration 같이 저장해야 BPM에 맞춰 속도 조절 가능

    private Vector3 _judgeLetterVector;

    private bool _isIntermission = false; // 현재 대기 상태 저장(턴 스프라이트 덮어쓰기용)

    //================초기화=================
    private void Awake()
    {
        if (_presenter != null)
            _presenter.OnSampleBeatScheduled += EnqueueSampleBeat;

        if (_scoring != null)
            _scoring.OnJudged += HandleJudgeResult; // 입력 시점 연출(즉시)

        if (_rewardPlanner != null)
            _rewardPlanner.OnGiveReward += HandleGiveReward;

        if (_flow != null) // Flow 이벤트 구독
        {
            _flow.OnLevelStarted += HandleLevelStarted;                 //레벨 시작 = 샘플턴 표시
            _flow.OnInputTurnStarted += HandleInputTurnStarted_Visual;  // 입력턴 시작 표시
            _flow.OnPatternFinished += HandlePatternFinished_Visual;    //입력턴 끝(다음은 샘플턴) 표시
            _flow.OnIntermissionChanged += HandleIntermissionChanged; // 공백/대기 상태 이벤트
        }

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

        if (_flow != null) 
        {
            _flow.OnLevelStarted -= HandleLevelStarted; 
            _flow.OnInputTurnStarted -= HandleInputTurnStarted_Visual;
            _flow.OnPatternFinished -= HandlePatternFinished_Visual;
            _flow.OnIntermissionChanged -= HandleIntermissionChanged;
        }
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
            if(reward != RewardType.None)
            {
                _rewardIcon.sprite = Manager.Item.ItemImages.GetItemSprite(reward);
                _rewardAmount.text = $"x{amount.ToString()}";
                _rewardAmount.gameObject.SetActive(true);
            }
            else
            {
                _rewardIcon.sprite = _fishSprite;
                _rewardAmount.gameObject.SetActive(false);
            }
        }
        else
        {
            _rewardIcon.sprite = null;
            _rewardAmount.gameObject.SetActive(false);
        }

        Manager.Audio.PlaySFX("GetItem");
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
        if(success)
        {
            //_bobber.BobberUp();
        }
        else
        {
            if (_rodHand != null) _rodHand.Fail();
        }

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
    //==================턴 전환 연출 이벤트 수신 ================
    private void HandleLevelStarted(RythmLevelPresetSO preset) // 레벨 시작 턴 표시
    {
        if (_isIntermission) return; //대기 중이면 턴 표시로 덮지 않음
        SetTurnSprite(true);  // 샘플턴 표시
    }

    // [추가] 입력턴 시작 시 스프라이트 변경
    private void HandleInputTurnStarted_Visual(int patternIndex, bool isLastPattern, int totalBeats)
    {
        if (_isIntermission) return; // 대기 중이면 무시
        SetTurnSprite(false); // 입력턴 표시
    }

    // 입력턴 종료(패턴 종료) 시 다음은 샘플턴이므로 샘플 표시로 복귀
    private void HandlePatternFinished_Visual(int patternIndex, bool isLastPattern, int totalBeats)
    {
        if (_isIntermission) return; // 대기 중이면 무시
        SetTurnSprite(true); // 샘플턴 표시
    }

    // 실제 스프라이트 교체 함수(이벤트에서만 호출)
    private void SetTurnSprite(bool isSampleTurn) // 턴 스프라이트 적용
    {
        if (_turnSpriteRenderer == null) return; //대상 없으면 종료

        if (_isIntermission) return; // 대기 중이면 턴 스프라이트로 덮지 않음

        Sprite next = isSampleTurn ? _sampleTurnSprite : _inputTurnSprite; // 턴에 맞는 스프라이트 선택
        _turnSpriteRenderer.sprite = next; //스프라이트 교체
    }
    private void HandleIntermissionChanged(bool isOn) // true면 대기 스프라이트로 덮어씀
    {
        _isIntermission = isOn; // 상태 저장

        if (isOn)
            SetIntermissionSprite(); // 대기 스프라이트 적용
        else
            SetTurnSprite(_flow != null && _flow.IsSampleTurn); //대기 종료 시 현재 턴으로 복귀
    }
    private void SetIntermissionSprite()
    {
        if (_turnSpriteRenderer == null) return;
        _turnSpriteRenderer.sprite = _intermissionSprite;
    }
}
