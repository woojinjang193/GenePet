using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoomInUiController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PetManager _petManager;

    [Header("네임판넬 활성화시 숨길 UI")]
    [SerializeField] private GameObject[] _hiddenUIs;

    [Header("성장 촉진 버튼")]
    [SerializeField] private GameObject _growthBooster;

    [Header("네임패널")]
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private GameObject _namePanel;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private int _nameLengthLimit = 10;

    private bool _isSubscribed = false;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(OnclickedConfirm);
    }
    private void Start()
    {
        _petManager.OnPetComeBack += OnPetComeBack;
        _petManager.OnPetLeft += OnPetLeft;
    }
    private void OnDestroy()
    {
        if( _petManager != null )
        {
            _petManager.OnPetComeBack -= OnPetComeBack;
            _petManager.OnPetLeft -= OnPetLeft;
        }    
    }
    private void OnEnable()
    {
        if (_petManager.ZoomedUnit == null) return;

        if( _petManager.ZoomedUnit.Status.IsLeft) //펫 떠난생태일때
        {
            TurnOnUIs(false, false);
            _growthBooster.SetActive(false);
            return;
        }

        //네임판넬 처리
        if (string.IsNullOrWhiteSpace(_petManager.ZoomedPet.DisplayName)) //이름이 없을때
        {
            _nameText.text = null;
            _petManager.ZoomedUnit.Status.OnGrown += OnGrown;
            _isSubscribed = true; //구독중 플레그 변경

            if (_petManager.ZoomedUnit.Status.Growth == GrowthStatus.Egg) //알이라면
            {
                TurnOnUIs(false, false);
                _growthBooster.SetActive(true);
            }
            else //알이 아니라면
            {
                TurnOnUIs(true, false);
                _input.text = null;
                _errorText.text = "";
                _growthBooster.SetActive(false);
            }
        }
        else // 이름 있으면
        {
            _nameText.text = _petManager.ZoomedPet.DisplayName;
            TurnOnUIs(false, true);
            _growthBooster.SetActive(true);
        }

    }
    public void CancelSubscribe()
    {
        if (!_isSubscribed) return; //구독중아니면 리턴

        _petManager.ZoomedUnit.Status.OnGrown -= OnGrown;
        _isSubscribed = false;
    }
    private void OnclickedConfirm()
    {
        if (string.IsNullOrWhiteSpace(_input.text))
        {
            _errorText.text = Manager.Lang.GetText("PopUp_EmptyName");
            return;
        }
        if(_input.text.Length > _nameLengthLimit)
        {
            _errorText.text = Manager.Lang.GetText("PopUp_TooLongName");
            return;
        }

        _nameText.text = _petManager.ZoomedPet.DisplayName = _input.text;
        TurnOnUIs(false, true);
        _growthBooster.SetActive(true);
        Debug.Log($"이름설정 : {_petManager.ZoomedPet.DisplayName} ");
    }

    private void TurnOnUIs(bool namePanel, bool others)
    {
        _namePanel.SetActive(namePanel);

        if (namePanel == true)
        {
            _input.text = null;
        }

        foreach (GameObject go in _hiddenUIs)
        {
            go.SetActive(others);
        }
    }
    //=============외부 호출=======================
    private void OnGrown(GrowthStatus growth) //성장 이벤트
    {
        if (growth != GrowthStatus.Egg)
        {
            if (string.IsNullOrWhiteSpace(_petManager.ZoomedPet.DisplayName) && !_namePanel.activeSelf) //이름이 없으면
            {
                TurnOnUIs(true, false);
                _growthBooster.SetActive(false);
            }   
        }
    }
    private void OnPetComeBack(PetUnit pet)
    {
        if (pet != _petManager.ZoomedUnit) return;

        if (string.IsNullOrWhiteSpace(_petManager.ZoomedPet.DisplayName)) //이름 없을때
        {
            TurnOnUIs(true, false);
            _growthBooster.SetActive(false);
        }
        else //이름 있을때
        {
            TurnOnUIs(false, true);
            _growthBooster.SetActive(true);
        }
    }
    private void OnPetLeft(PetUnit pet)
    {
        if (pet != _petManager.ZoomedUnit) return;

        TurnOnUIs(false, false);
        _growthBooster.SetActive(false);
    }
}
