using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinballUiManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PinballVisualManager _visual;

    [Header("슬롯")]
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
}
