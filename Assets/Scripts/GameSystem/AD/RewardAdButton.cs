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

    [Header("tmp")]
    [SerializeField] private TMP_Text _text;

    [Header("랜덤보상 여부")]
    [SerializeField] private bool _isRandom = false;

    [Header("아이템 보상 목록")]
    [SerializeField] private List<ItemRewardEntry> _itemRewards = new();

    [Header("알 보상 목록(프리셋)")]
    [SerializeField] private List<EggRewardEntry> _eggRewards = new();

    private List<RewardData> _rewards = new();
    private Button _button;

    private Coroutine _cooldownRoutine; // 쿨타임 카운트다운 코루틴

    [Serializable]
    public class ItemRewardEntry
    {
        public RewardType Type = RewardType.None;
        public int Amount = 0;
        public float Weight = 0;
    }
    [Serializable]
    public class EggRewardEntry // 알 보상 엔트리
    {
        public RewardEggPresetSO Preset = null; //알 프리셋 참조
        public float Weight = 0f;  //가중치
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
        if (_isDaily) Manager.Game.OnDailyReset += RefreshUI;
    }
    private void OnDisable()
    {
        StopCooldownRoutine(); // 비활성화되면 코루틴 정리
        if (_isDaily) Manager.Game.OnDailyReset -= RefreshUI;
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
            if (!RewardTimeRecordService.CanClaimCoolTimeReward(_rewardID)) // 쿨타임 안됐으면
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
    private List<RewardData> BuildPayoutList() //보상 리스트 만들기
    {
        List<RewardData> payout = new();

        int itemCount = _itemRewards.Count;
        int eggCount = _eggRewards.Count; 
        int total = itemCount + eggCount;

        if (total <= 0) return payout;

        if (_isRandom) // 랜덤일 때만 1개 선택
        {
            bool pickedItem; //아이템/알 구분
            int pickedIndex;  // 선택 인덱스

            if (!TryPickWeighted(out pickedItem, out pickedIndex)) // Weight 기반 선택(실패 시 none)
                return payout;      // 선택 실패면 빈 리스트

            if (pickedItem) // 아이템이면
                TryAddItem(_itemRewards[pickedIndex], payout); // 선택 인덱스로 추가
            else //알이면
                TryAddEgg(_eggRewards[pickedIndex].Preset, payout); //엔트리에서 Preset 꺼내 추가

            return payout;
        }

        // 전부 지급(Weight 무시)
        for (int i = 0; i < itemCount; i++) TryAddItem(_itemRewards[i], payout);
        for (int i = 0; i < eggCount; i++) TryAddEgg(_eggRewards[i].Preset, payout);

        return payout;
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
    private bool TryPickWeighted(out bool isItem, out int index) // 아이템/알 통합 가중치 선택
    {
        isItem = false; // 기본값
        index = -1;  //기본값

        float totalWeight = 0f; // 전체 가중치 합

        // 아이템 가중치 합
        for (int i = 0; i < _itemRewards.Count; i++)
        {
            float w = _itemRewards[i].Weight; // 아이템 Weight 사용
            if (w > 0f) totalWeight += w;  // 양수만 합산
        }

        // 알 가중치 합
        for (int i = 0; i < _eggRewards.Count; i++)
        {
            float w = _eggRewards[i].Weight; // 알 Weight 사용
            if (w > 0f) totalWeight += w;// 양수만 합산
        }

        // 전부 0이면 균등 랜덤으로 폴백
        if (totalWeight <= 0f)
        {
            int total = _itemRewards.Count + _eggRewards.Count; // 전체 개수
            if (total <= 0) return false;     // 아무것도 없으면 실패

            int pick = UnityEngine.Random.Range(0, total);      // 균등 랜덤
            if (pick < _itemRewards.Count) { isItem = true; index = pick; } // 아이템 선택
            else { isItem = false; index = pick - _itemRewards.Count; }     //  알 선택
            return true;
        }

        float random = UnityEngine.Random.value * totalWeight;
        float acc = 0f;  // 누적

        // 아이템 먼저 탐색 
        for (int i = 0; i < _itemRewards.Count; i++)
        {
            float w = _itemRewards[i].Weight;
            if (w <= 0f) continue;   //0 이하는 스킵
            acc += w;
            if (random < acc) { isItem = true; index = i; return true; }
        }

        // 알 탐색
        for (int i = 0; i < _eggRewards.Count; i++)
        {
            float w = _eggRewards[i].Weight;
            if (w <= 0f) continue;
            acc += w; 
            if (random < acc) { isItem = false; index = i; return true; }
        }

        return false;
    }

    private void RefreshUI() //버튼/텍스트 상태를 한 곳에서 갱신
    {
        if (_isDaily) // 데일리
        {
            bool can = RewardTimeRecordService.CanClaimDailyReward(_rewardID);
            _button.interactable = can;
            if (_text != null) _text.text = can ? Manager.Lang.GetText("Button_GetReward") : Manager.Lang.GetText("Button_DailyRewardGranted");
            return;
        }

        // 쿨타임
        int remain = RewardTimeRecordService.GetRemainingCooldownSec(_rewardID); // 남은시간 받기
        bool canClick = (remain <= 0); // 남은시간 0 이하면 클릭 가능

        if (canClick) StopCooldownRoutine();
        _button.interactable = canClick;

        if (_text != null) _text.text = canClick ? Manager.Lang.GetText("Button_GetReward") : RewardTimeRecordService.FormatRemainingHMS(remain);
    }

    private void StartCooldownRoutineIfNeeded() //쿨타임 중일 때만 코루틴 실행
    {
        StopCooldownRoutine(); // 중복 방지

        int remain = RewardTimeRecordService.GetRemainingCooldownSec(_rewardID);
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
