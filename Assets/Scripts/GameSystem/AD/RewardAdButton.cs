using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdButton : AdRequestBase
{
    [Header("보상 ID")]
    [SerializeField] private string _rewardID;

    [Header("데일리")]
    [SerializeField] private bool _isDaily = false;

    [Header("쿨타임(시간) 데일리면 무시됨")]
    [SerializeField] private float _coolTimeHour;

    [Header("tmp")]
    [SerializeField] private TMP_Text _text;

    [Header("랜덤보상 여부")]
    [SerializeField] private bool _isRandom = false;

    [Header("아이템 보상 목록")]
    [SerializeField] private List<ItemRewardEntry> _itemRewards = new();

    [Header("알 보상 목록(프리셋)")]
    [SerializeField] private List<RewardEggPresetSO> _eggPresets = new();

    private List<RewardData> _rewards = new();
    private Button _button;

    private Coroutine _cooldownRoutine; // 쿨타임 카운트다운 코루틴

    [Serializable]
    public class ItemRewardEntry
    {
        public RewardType Type = RewardType.None;
        public int Amount = 0;
    }

    private void Awake() 
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClaimRewards);
    }
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_rewardID)) { _button.interactable = false; return; }

        RefreshUI(); // 버튼/텍스트 갱신

        if (!_isDaily) StartCooldownRoutineIfNeeded(); // 쿨타임이면 카운트다운 시작
    }
    private void OnDisable()
    {
        StopCooldownRoutine(); // 비활성화되면 코루틴 정리
    }
    public void ClaimRewards()
    {
        if (Manager.Item == null) return;

        // ===== 수령 가능 체크 =====
        if (_isDaily) //데일리면
        {
            if (!RewardTimeRecordService.TryBeginAdClaim(_rewardID)) // 수령 불가능이면
            {
                RefreshUI(); //UI 갱신
                return;
            }
        }
        else //쿨타임이면
        {
            int cooldownSec = (int)_coolTimeHour * 3600; // 시간 > 초 변환
            if (!RewardTimeRecordService.CanClaimReward(_rewardID, cooldownSec)) // 쿨타임 안됐으면
            {
                RefreshUI(); //UI 갱신(남은시간 표시)
                StartCooldownRoutineIfNeeded(); //카운트다운 시작
                return;
            }
        }

        // ===== 보상 리스트 생성 =====
        _rewards.Clear();
        _rewards = BuildPayoutList();
        if (_rewards.Count == 0) return;

        base.Request(); //광고 요청
    }
    //======리워드 리스트 생성=================
    private List<RewardData> BuildPayoutList()
    {
        List<RewardData> payout = new();

        int itemCount = _itemRewards.Count;
        int eggCount = _eggPresets.Count;
        int total = itemCount + eggCount;

        if (total <= 0) return payout;

        if (_isRandom) //랜덤이면
        {
            int pick = UnityEngine.Random.Range(0, total);

            if (pick < itemCount) // 아이템 영역
                TryAddItem(_itemRewards[pick], payout); // 아이템 추가
            else // 알 영역
                TryAddEgg(_eggPresets[pick - itemCount], payout); // 알 추가

            return payout;
        }

        // 전부 지급
        for (int i = 0; i < itemCount; i++) TryAddItem(_itemRewards[i], payout); // 아이템 전부
        for (int i = 0; i < eggCount; i++) TryAddEgg(_eggPresets[i], payout); // 알 전부

        return payout; //전부 지급 리스트
    }

    private void TryAddItem(ItemRewardEntry entry, List<RewardData> payout) // 아이템 리스트에 담기
    {
        if (entry == null) return;
        if (entry.Type == RewardType.None) return;
        if (entry.Amount <= 0) return;

        payout.Add(RewardData.CreateItem(entry.Type, entry.Amount)); //RewardData로 변환
    }

    private void TryAddEgg(RewardEggPresetSO preset, List<RewardData> payout) //알 변환
    {
        if (preset == null) return;
        EggData egg = EggDataGenerator.GenerateRewardEgg(preset); // 알 생성
        if (egg == null) return;

        payout.Add(RewardData.CreateEgg(egg)); // RewardData로 변환
    }
    //==================광고 후처리=============================

    protected override void OnReward()
    {
        Manager.Item.GiveMiniGameRewards(_rewards); //보상 지급
        RewardTimeRecordService.MarkClaimed(_rewardID, _isDaily); // 기록 저장

        RefreshUI(); //버튼/텍스트 갱신
        if (!_isDaily) StartCooldownRoutineIfNeeded(); //쿨타임이면 카운트다운 재시작
    }
    protected override void OnClosed()
    {
        Manager.Item.NotifyRewardsReady(); // 팝업 신호

        RefreshUI(); //버튼/텍스트 갱신
        //if (!_isDaily) StartCooldownRoutineIfNeeded(); //쿨타임이면 카운트다운 유지
    }
    //===================유틸=========================
    private void RefreshUI() //버튼/텍스트 상태를 한 곳에서 갱신
    {
        if (_isDaily) // 데일리
        {
            bool can = RewardTimeRecordService.CanClaimReward(_rewardID);
            _button.interactable = can;
            if (_text != null) _text.text = can ? "보상 받기" : "오늘 보상 수령";
            return;
        }

        // 쿨타임
        int cooldownSec = (int)_coolTimeHour * 3600; //초로 변환
        int remain = RewardTimeRecordService.GetRemainingCooldownSec(_rewardID, cooldownSec); // 남은시간 받기
        bool canClick = (remain <= 0); // 남은시간 0 이하면 클릭 가능

        if (canClick) StopCooldownRoutine();
        _button.interactable = canClick;

        if (_text != null) _text.text = canClick ? "보상 받기" : RewardTimeRecordService.FormatRemainingHMS(remain);
    }

    private void StartCooldownRoutineIfNeeded() //쿨타임 중일 때만 코루틴 실행
    {
        StopCooldownRoutine(); // 중복 방지

        int cooldownSec = (int)_coolTimeHour * 3600;
        int remain = RewardTimeRecordService.GetRemainingCooldownSec(_rewardID, cooldownSec);
        if (remain <= 0) return; // 이미 가능하면 안 돌림

        _cooldownRoutine = StartCoroutine(CooldownTickRoutine());
    }

    private void StopCooldownRoutine() // 코루틴 정지
    {
        if (_cooldownRoutine == null) return;
        StopCoroutine(_cooldownRoutine);
        _cooldownRoutine = null;
    }

    private IEnumerator CooldownTickRoutine() // 1초마다 남은시간 갱신
    {
        while (isActiveAndEnabled) //켜져있는동안
        {
            RefreshUI(); // 남은시간/버튼 상태 갱신
            yield return new WaitForSeconds(1f); // 1초 주기
        }
    }
}
