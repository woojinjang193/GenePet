using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PointerDir
{
    Up, Down, Left, Right, None
}
public class TutorialPointer : MonoBehaviour
{
    [SerializeField] private Sprite _upSprite;
    [SerializeField] private Sprite _downSprite;
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;

    private Image _image;
    private Animator _animator;
    private Transform _prevParent;
    private PointerDir _prevDic;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _image = GetComponent<Image>();
    }
    public void SetPointer(PointerDir dir,Transform parent, bool isAnimated)
    {
        if (parent == null) return;

        if (_prevParent == parent && _prevDic == dir) return;//전이랑 같은 부모, 같은 방향이면

        _prevParent = parent; //전 포지션 저장
        transform.SetParent(parent, false);

        _animator.SetBool("Up", false);
        _animator.SetBool("Down", false);
        _animator.SetBool("Left", false);
        _animator.SetBool("Right", false);

        switch (dir)
        {
            case PointerDir.Up: _image.sprite = _upSprite; _animator.SetBool("Up",isAnimated); break;
            case PointerDir.Down: _image.sprite = _downSprite; _animator.SetBool("Down", isAnimated); break;
            case PointerDir.Left: _image.sprite = _leftSprite; _animator.SetBool("Left", isAnimated); break;
            case PointerDir.Right: _image.sprite = _rightSprite; _animator.SetBool("Right", isAnimated); break;
        }
    }
}
