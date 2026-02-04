using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_SpawnEgg : TutorialBase
{
    [Header("버튼 리스트")]
    [SerializeField] private List<Button> _buttonCantTouch;
    [Header("소환 옵션버튼")]
    [SerializeField] private Button _spawnOptionButton;
    [Header("랜던 소환 버튼")]
    [SerializeField] private Button _randomSpawnButton;


    [Header("포인터")]
    [SerializeField] private TutorialPointer _pointer;
    [Header("포인터")]
    [SerializeField] private Transform _pointerStartAt;
    [Header("스크린 버튼")]
    [SerializeField] private Button _screenButton;
    [Header("대사 패널")]
    [SerializeField] private GameObject _dialogueArea;
    [Header("대사")]
    [SerializeField] private List<LineInfo> _lines;
    [Header("텍스트")]
    [SerializeField] private TMP_Text _text;
    [Header("이름")]
    [SerializeField] private TMP_Text _nameText;
    //[Header("소환 옵션 판넬")]

    //private bool _isWaitingRandomButton = false;
    private int _curLine = -1;

    private void Awake()
    {
        ButtonOn(false);
        _spawnOptionButton.onClick.AddListener(OnSpawnOptionOpened);
        _randomSpawnButton.onClick.AddListener(OnRandomSpawned);
        _screenButton.onClick.AddListener(Next);
    }
    public override void Enter()
    {
        gameObject.SetActive(true);
        _pointer.SetPointer(PointerDir.Up, _pointerStartAt, true);
        _spawnOptionButton.interactable = true; //소환창 버튼 활성화
    }
    public override void Exit()
    {
        ButtonOn(true);
        gameObject.SetActive(false);
    }
    //=================버튼 클릭시==================
    private void OnSpawnOptionOpened() //옵션버튼 클릭시
    {
        _screenButton.gameObject.SetActive(true); //스크린 버튼 활성화
        _dialogueArea.gameObject.SetActive(true); //대사창 활성화
        Next();
    }
    private void OnRandomSpawned()
    {
        //_isWaitingRandomButton = false;
        _screenButton.gameObject.SetActive(true); //스크린 버튼 활성화
        Next();
    }
    private void Next() //스크린 버튼 클릭시
    {
        //if (_isWaitingRandomButton) return;

        if (_curLine >= _lines.Count - 1)
        {
            Complete();
            return;
        }

        _curLine++;

        LineInfo lineInfo = _lines[_curLine];

        if (lineInfo == null) //할당 안됐으면 다음으로
        {
            Next();
            return;
        }

        if(_curLine == 3)
        {
            //_isWaitingRandomButton = true;
            _randomSpawnButton.interactable = true; // 랜덤소환버튼 켜줌
            _screenButton.gameObject.SetActive(false); //스크린 버튼 비활성화
        }

        //Sprite sprite = lineInfo._sprite != null ? lineInfo._sprite : null; //있으면 쓰고 없으면 null
        //_image.sprite = sprite;
        //_image.gameObject.SetActive(sprite != null); //이미지 있으면 띄워줌

        Transform parent = lineInfo.PointerPos != null? lineInfo.PointerPos : null;
        if (parent != null)
            _pointer.SetPointer(lineInfo.PointerDir, parent, lineInfo.PointerAnim); //포인터 설정

        string nextLineID = string.IsNullOrEmpty(lineInfo.TextID) ? "" : lineInfo.TextID;
        string nextLine = Manager.Lang.GetText(nextLineID); //번역
        _text.text = nextLine;

        string nameID = string.IsNullOrEmpty(lineInfo.NameID) ? "" : lineInfo.NameID;
        string nextName = Manager.Lang.GetText(nameID); //번역
        _nameText.text = nextName;
    }
    //==============버튼 활성화/비활성화===============
    private void ButtonOn(bool on)
    {
        for (int i = 0; i < _buttonCantTouch.Count; i++)
        {
            _buttonCantTouch[i].interactable = on;
        }
    }
}
