using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomChanger : MonoBehaviour
{
    [Header("펫매니저")]
    [SerializeField] private PetManager _petManager;
    [Header("배경 컨트롤러")]
    [SerializeField] private BackgroundRoomController _backgroundController;

    [Header("판넬 열기 버튼")]
    [SerializeField] private Button _roomChangeButton;

    [Header("판넬")]
    [SerializeField] private GameObject _roomChangePanel;

    [Header("판넬 활성화시 꺼져야하는 UI 창들")]
    [SerializeField] private GameObject[] _otherUIs;

    [Header("배경 좌우 버튼")]
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _leftButton;

    [Header("확인/취소 버튼")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    private List<Room> _roomList = new();
    private int _curIndex;
    private int _ogIndex;
    private void Awake()
    {
        if (_petManager == null)
        {
            _petManager = FindObjectOfType<PetManager>();
        }
        if(_backgroundController == null )
        {
            _backgroundController = FindObjectOfType<BackgroundRoomController>();
        }
        _roomChangeButton.onClick.AddListener(OnRoomChangeButtonClicked);

        _rightButton.onClick.AddListener(OnRightClicked);
        _leftButton.onClick.AddListener(OnLeftClicked);

        _confirmButton.onClick.AddListener(OnConfirmClicked);
        _cancelButton.onClick.AddListener(OnCancelClicked);
    }
    private void OnRoomChangeButtonClicked() //판넬 오픈
    {
        GetRoomListAndSetIndex();
        DisplayBackground(_curIndex);
        TurnOnOtherUIs(false); //배경 가리는 창들 비활성화
        _roomChangePanel.SetActive(true);
    }
    private void OnRightClicked()
    {
        if (_curIndex + 1 >= _roomList.Count)
        {
            _curIndex = 0;
        }
        else
        {
            _curIndex++;
        }
        DisplayBackground(_curIndex);
    }
    private void OnLeftClicked()
    {
        if (_curIndex - 1 < 0)
        {
            _curIndex = _roomList.Count - 1;
        }
        else
        {
            _curIndex--;
        }

        DisplayBackground(_curIndex);
    }
    private void OnConfirmClicked()
    {
        PetSaveData petData = _petManager.ZoomedPet;

        petData.RoomType = _roomList[_curIndex]; //룸 정보 저장

        TurnOnOtherUIs(true); //비활성화 한 창들 다시 활성화
        _roomChangePanel.SetActive(false);
    }
    private void OnCancelClicked()
    {
        DisplayBackground(_ogIndex);

        TurnOnOtherUIs(true);//비활성화 한 창들 다시 활성화
        _roomChangePanel.SetActive(false);
    }

    private void GetRoomListAndSetIndex()
    {
        _roomList.Clear(); //리스트 클리어

        var user = Manager.Save.CurrentData.UserData;

        var userRoomList = user.Items.Rooms; //유저가 소유한 룸 리스트
        var curRoom = _petManager.ZoomedPet.RoomType; // 줌인된 펫의 룸정보

        for (int i = 0; i < userRoomList.Count; i++)
        {
            _roomList.Add(userRoomList[i]);

            if (curRoom == userRoomList[i])
            {
                _ogIndex = i;
                _curIndex = i;
            }
        }
        
    }

    private void DisplayBackground(int index)
    {
        _backgroundController.SetRoom(_roomList[index]);
        //Debug.Log($"현재 Index: {index}, 현재 룸 : {_roomList[index]}");
    }

    private void TurnOnOtherUIs(bool on)
    {
        foreach (var ob in _otherUIs)
        {
            ob.SetActive(on);
        }
    }
} 

            

