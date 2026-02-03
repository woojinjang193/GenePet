using System.Collections;
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
    [Header("텍스트")]
    [SerializeField] private TMP_Text _Text;
    [Header("포인터")]
    [SerializeField] private RectTransform _pointer;
    //[Header("소환 옵션 판넬")]
    

    private void Awake()
    {
        ButtonOn(false);
        _spawnOptionButton.onClick.AddListener(OnSpawnOptionOpened);
        _randomSpawnButton.onClick.AddListener(OnRandomSpawned);
    }
    public override void Enter()
    {
        gameObject.SetActive(true);
        _spawnOptionButton.interactable = true; //소환창 버튼 활성화
    }

    public override void Exit()
    {
        ButtonOn(true);
        gameObject.SetActive(false);
    }

    //=================버튼 클릭시==================
    private void OnSpawnOptionOpened()
    {
        _Text.text = "Click Here!";
    }
    private void OnRandomSpawned()
    {
        _Text.text = "goodgood!";
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
