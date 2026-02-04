using System.Reflection;
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
    private PointerDir _prevDir;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _image = GetComponent<Image>();
    }
    public void SetPointer(PointerDir dir,Transform parent, bool isAnimated)
    {
        if (parent == null) return;

        gameObject.SetActive(true);

        if (_prevParent == parent && _prevDir == dir) return;//전이랑 같은 부모, 같은 방향이면

        _prevParent = parent; //전 포지션 저장
        _prevDir = dir;

        transform.SetParent(parent, false);

        // None이면 포인터 숨김 처리
        if (dir == PointerDir.None)
        {
            if (_image != null) _image.enabled = false;
            _animator.SetBool("Animated", false);
            return;
        }

        if (_image != null) _image.enabled = true; // None이 아니면 이미지 활성

        switch (dir)
        {
            case PointerDir.Up: _image.sprite = _upSprite; _animator.SetInteger("Dir", 0); break;
            case PointerDir.Down: _image.sprite = _downSprite; _animator.SetInteger("Dir", 1); break;
            case PointerDir.Left: _image.sprite = _leftSprite; _animator.SetInteger("Dir", 2); break;
            case PointerDir.Right: _image.sprite = _rightSprite; _animator.SetInteger("Dir", 3); break;
        }

        _animator.SetBool("Animated", isAnimated);
    }
}
