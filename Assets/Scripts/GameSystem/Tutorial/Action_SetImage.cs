using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ImageSize
{
    Small,
    Medium,
    Large,
}
public class Action_SetImage : TutorialActionBase
{
    [Header("타겟")]
    [SerializeField] private Image _target;
    [Header("스프라이트")]
    [SerializeField] private Sprite _sprite;
    [Header("사이즈")]
    [SerializeField] private ImageSize _size;

    private RectTransform _rectT;
    private void Awake()
    {
        if (_target == null) return;

        _rectT = _target.GetComponent<RectTransform>();
        SetSize();
    }
    public override void Execute()
    {
        if (_target == null) return;
        if (_sprite == null) return;

        _target.gameObject.SetActive(true);
        _target.sprite = _sprite;
    }
    private void SetSize()
    {
        switch (_size)
        {
            case ImageSize.Small: _rectT.sizeDelta = new Vector2(300,300); break;
            case ImageSize.Medium: _rectT.sizeDelta = new Vector2(500, 500); break;
            case ImageSize.Large: _rectT.sizeDelta = new Vector2(800, 800); break;
        }
    }
}
