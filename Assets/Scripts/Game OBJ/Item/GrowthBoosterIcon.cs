using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrowthBoosterIcon : MonoBehaviour, IConfirmRequester
{
    [Header("참조")]
    [SerializeField] private PetManager _petManager;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _amountText;

    private ItemManager _itemManager;
    private int _curHave;
    private PetUnit _curPet;

    private void Awake()
    {
        _itemManager = Manager.Item;

        if (_itemManager != null)
        {
            _itemManager.OnItemConsumed += UpdateUI;
            _itemManager.OnRewardGranted += UpdateUI;
        }
        if (_button == null) _button = GetComponent<Button>();

        _button.onClick.AddListener(TryBoost);

        _curHave = Manager.Save.CurrentData.UserData.Items.GrowthBooster;
        _amountText.text = $"x{_curHave.ToString()}";
    }
    private void OnEnable()
    {
        _curPet = _petManager.ZoomedUnit;
    }
    private void OnDisable()
    {
        _curPet = null;
    }
    private void OnDestroy()
    {
        if( _itemManager != null )
        {
            _itemManager.OnItemConsumed -= UpdateUI;
            _itemManager.OnRewardGranted -= UpdateUI;
        }
    }
    private void TryBoost()
    {
        if(_curPet == null ) return;

        if(_curPet.Status.Growth == GrowthStatus.Adult)
        {
            Manager.Game.ShowPopup("PopUp_AlreadyGrown"); //TODO: 로컬라이제이션
            return;
        }

        if(_curHave <= 0)
        {
            Manager.Game.ShowConfirmMessage("Asking_MoveToShop", 1, this);
            return;
        }

        Manager.Game.ShowConfirmMessage("Confirm_UseBooster",0, this); //컨멈 메세지 보내기
    }
    // =====인터페이스 구현 =================
    public void Canceled(int requestNum)
    {

    }

    public void Confirmed(int requestNum)
    {
        if(requestNum == 0) //부스터 사용
        {
            if (_curPet == null) return; // 방어
            if (_petManager == null) return; // 방어

            bool success = _petManager.ApplyGrowthBooster(_curPet); // 성장 적용
            if (!success) return; // 실패면 아이템 차감 안함

            Manager.Item.UseItem(RewardType.GrowthBooster, 1); //컨펌시 아이템 --
        }
        else if (requestNum == 1) //상점이동 요청
        {
            Manager.Game.OpenUiPanel(UIPanel.Shop, false);
        }
    }

    private void UpdateUI(RewardType type, int newValue)
    {
        if (type != RewardType.GrowthBooster) return; //부스터 아니면 리턴

        _curHave = newValue;
        _amountText.text = $"x{newValue}";
    }
}
