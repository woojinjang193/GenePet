using System;
using System.Collections;
using UnityEngine;

public class RythmJudgeLetter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    [SerializeField] private Sprite _perfectSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _missSprite;

    [SerializeField] private float _distance;
    [SerializeField] private float _moveSpeed;

    public event Action<RythmJudgeLetter> OnFinishedMoving;

    public void Init(JudgeResult result)
    {
        StopAllCoroutines();

        switch (result)
        {
            case JudgeResult.Perfect: _renderer.sprite = _perfectSprite; break;
            case JudgeResult.Good: _renderer.sprite = _goodSprite; break;
            case JudgeResult.Miss: _renderer.sprite = _missSprite; break;
        }

        Color col = _renderer.color;
        col.a = 1f;
        _renderer.color = col;

        transform.localPosition = Vector3.zero;

        _renderer.gameObject.SetActive(true);

        StartCoroutine(MoveUpRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator MoveUpRoutine()
    {
        float startY = transform.position.y;
        float targetY = transform.position.y + _distance;

        float startAlpha = _renderer.color.a;

        while (transform.position.y < targetY)
        {
            // 위치 이동
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, targetY, _moveSpeed * Time.deltaTime);
            transform.position = pos;

            // 진행 비율 (0 ~ 1)
            float t = (pos.y - startY) / _distance;

            // 알파 감소
            Color col = _renderer.color;
            col.a = Mathf.Lerp(startAlpha, 0f, t);
            _renderer.color = col;

            yield return null;
        }

        OnFinishedMoving?.Invoke(this);
    }
}
