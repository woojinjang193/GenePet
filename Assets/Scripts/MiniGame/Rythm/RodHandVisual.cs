using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodHandVisual : MonoBehaviour
{
    [Header("손 트랜스폼")]
    [SerializeField] private Transform _handsTransform;
    [Header("손 올리/내리 는 거리")]
    [SerializeField] private float _handsDistanceUp = 0.2f; 
    [SerializeField] private float _handsDistanceDown = 0.2f; 
    [Header("낚싯대 랜더러")]
    [SerializeField] private SpriteRenderer _renderer;
    [Header("낚싯대 스프라이트")]
    [SerializeField] private Sprite _perfectSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _missSprite;

    private Sprite _ogSprite;
    private Vector3 _ogPos;
    private void Awake()
    {
        _ogSprite = _renderer.sprite;
        _ogPos = _handsTransform.position;
    }
    public void PullUp(JudgeResult result) 
    {
        switch(result)
        {
            case JudgeResult.Perfect: Perfect(); break;
            case JudgeResult.Good: Good(); break;
            case JudgeResult.Miss: Miss(); break;
        }
        
        StartCoroutine(RodSpriteRoutine());
    }
    private IEnumerator RodSpriteRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        _renderer.sprite = _ogSprite;
        _handsTransform.position = _ogPos;
    }
    private void Perfect()
    {
        _renderer.sprite = _perfectSprite;

        Vector3 pos = _handsTransform.position;
        pos.y += _handsDistanceUp;
        _handsTransform.position = pos;
    }
    private void Good()
    {
        _renderer.sprite = _goodSprite;

        Vector3 pos = _handsTransform.position;
        pos.y += _handsDistanceUp;
        _handsTransform.position = pos;
    }
    private void Miss()
    {
        _renderer.sprite = _missSprite;

        Vector3 pos = _handsTransform.position;
        pos.y -= _handsDistanceDown;
        _handsTransform.position = pos;
    }
}
