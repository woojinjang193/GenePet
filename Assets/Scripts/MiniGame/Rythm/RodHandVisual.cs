using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodHandVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _perfectSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _missSprite;

    private Sprite _ogSprite;
    private void Awake()
    {
        _ogSprite = _renderer.sprite;
    }
    public void PullUp(JudgeResult result)
    {
        switch(result)
        {
            case JudgeResult.Perfect: _renderer.sprite = _perfectSprite; break;
            case JudgeResult.Good: _renderer.sprite = _goodSprite; break;
            case JudgeResult.Miss: _renderer.sprite = _missSprite; break;
        }
        
        StartCoroutine(RodSpriteRoutine());
    }
    private IEnumerator RodSpriteRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        _renderer.sprite = _ogSprite;
    }
}
