using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinballUiManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PinballVisualManager _visual;
    [SerializeField] private PinballRouletteZone _rouletteZone;
    [SerializeField] private PinballGameManager _pinballManager;

    [Header("게임끝 판넬")]
    [SerializeField] private PinballTitlePanel _titlePanel;
    [SerializeField] private float _EndPanelOpenDelay = 2;

    [Header("슬롯")]
    [SerializeField] private GameObject _slotPanel;
    [SerializeField] private Image _slot1Image;
    [SerializeField] private Image _slot2Image;
    [SerializeField] private Image _slot3Image;

    [Header("슬롯별 아이템 개수")]
    [SerializeField] private TMP_Text _slot1Text;
    [SerializeField] private TMP_Text _slot2Text;
    [SerializeField] private TMP_Text _slot3Text;

    private void Awake()
    {
        _visual.OnItemFlown += SlotUpdate;
        _rouletteZone.OnRouletteStart += CloseSlots;
        _pinballManager.OnGameStart += ResetUis;
    }
    private void Start()
    {
        Manager.Audio.PlayBGM("BGM_Pinball");
    }
    private void OnDestroy()
    {
        _visual.OnItemFlown -= SlotUpdate;
        _rouletteZone.OnRouletteStart -= CloseSlots;
        _pinballManager.OnGameStart -= ResetUis;
    }
    private void SlotUpdate(BrickColor color, Sprite icon, int amount)
    {
        Image image = null;
        TMP_Text text = null;
        switch (color)
        {
            case BrickColor.one: image = _slot1Image; text = _slot1Text; break;
            case BrickColor.two: image = _slot2Image; text = _slot2Text; break;
            case BrickColor.three: image = _slot3Image; text = _slot3Text; break;
        }

        image.sprite = icon;

        if (amount > 1) // 개수가 1개보다 많을때만 텍스트 켜줌
        {
            text.text = $"x{amount}";
            text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }
    //게임종료 UI 오픈===========================
    public void GameEndUiOpen()
    {
        StartCoroutine(OpenEndPanel());
    }

    private IEnumerator OpenEndPanel()
    {
        yield return new WaitForSeconds(_EndPanelOpenDelay);
        
        //스코어 업데이트
        int curScore = _pinballManager.Score;
        int bestScore = Manager.Mini.GetBestScore(MiniGame.Pinball);
        _titlePanel.UpdateScore(curScore, bestScore);
        _titlePanel.gameObject.SetActive(true);
        Manager.Item.NotifyRewardsReady();
    }
    //===================플레이어가 룰렛존 진입시 호출===========================
    private void CloseSlots()
    {
        _slotPanel.SetActive(false);

        //개수 텍스트 초기화
        _slot1Text.text = "";
        _slot2Text.text = "";
        _slot3Text.text = "";
    }
    private void ResetUis() //게임 시작시 호출
    {
        _slot1Image.sprite = null;
        _slot2Image.sprite = null;
        _slot3Image.sprite = null;

        _slotPanel.SetActive(true);
    }
}
