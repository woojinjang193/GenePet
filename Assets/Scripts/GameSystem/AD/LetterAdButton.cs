using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LetterAdButton : AdRequestBase
{
    [Header("보상 ID")]
    [SerializeField] private string _rewardID;

    [Header("데일리")]
    [SerializeField] private bool _isDaily = false;

    [Header("tmp")]
    [SerializeField] private TMP_Text _text;

    [Header("참조")]
    [SerializeField] private LetterPanel _letter;
    [SerializeField] private TutorialController _tutorialController;

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
        if (_isDaily) Manager.Game.OnDailyReset += RefreshUI;
    }
    private void OnDisable()
    {
        StopCooldownRoutine(); // 비활성화되면 코루틴 정리
        if (_isDaily) Manager.Game.OnDailyReset += RefreshUI;
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
        base.Request(); //광고 요청
    }
    
    //==================광고 후처리=============================

    protected override void OnReward()
    {
        RewardTimeRecordService.MarkClaimed(_rewardID, _isDaily); // 기록 저장
      //RefreshUI(); //버튼/텍스트 갱신
        _letter.BringPetBack();
        //if (!_isDaily) StartCooldownRoutineIfNeeded(); //쿨타임이면 카운트다운 재시작

    }
    protected override void OnClosed() //닫히니까 없어도 됨
    {

    }
    //===================유틸=========================
    private void RefreshUI() //버튼/텍스트 상태를 한 곳에서 갱신
    {
        if (_isDaily) // 데일리
        {
            bool can = RewardTimeRecordService.CanClaimDailyReward(_rewardID);
            _button.interactable = can;
            if (_text != null) _text.text = can ? "" : Manager.Lang.GetText("Button_DailyRewardGranted");
            return;
        }

        // 쿨타임
        int remain = RewardTimeRecordService.GetRemainingCooldownSec(_rewardID); // 남은시간 받기
        bool canClick = (remain <= 0); // 남은시간 0 이하면 클릭 가능

        if (canClick) StopCooldownRoutine();
        _button.interactable = canClick;

        if (_text != null) _text.text = canClick ? "" : RewardTimeRecordService.FormatRemainingHMS(remain);
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
