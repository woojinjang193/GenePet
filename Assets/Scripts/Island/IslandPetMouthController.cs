using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandPetMouthController : MonoBehaviour
{
    [SerializeField] private Sprite _chewMouth;

    private Animator _anim;
    private SpriteRenderer _spriteRenderer;
    private Sprite _ogMouth;

    public Action<Gift> OnGiveTaken;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _ogMouth = _spriteRenderer.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item1"))
        {
            collision.gameObject.SetActive(false);
            OnGiveTaken?.Invoke(Gift.Gift1);
            //먹는 사운드 출력
        }
        else if (collision.CompareTag("Item2"))
        {
            collision.gameObject.SetActive(false);
            OnGiveTaken?.Invoke(Gift.Gift2);
            //먹는 사운드 출력
        }
        else if (collision.CompareTag("Item3"))
        {
            collision.gameObject.SetActive(false);
            OnGiveTaken?.Invoke(Gift.Gift3);
            //먹는 사운드 출력
        }
        else if (collision.CompareTag("Item4"))
        {
            collision.gameObject.SetActive(false);
            OnGiveTaken?.Invoke(Gift.Gift4);
            //먹는 사운드 출력
        }
    }
    public void StartAnimation(bool isWanted)
    {
        if (isWanted)
        {
            _anim.SetTrigger("Eat");
        }
        else
        {
            _anim.SetTrigger("Full");
        }
    }
    //애니메이션 전용
    public void ChangeToChewMouth()
    {
        //Debug.Log("애니메이션 시작");
        _spriteRenderer.sprite = _chewMouth;
    }
    public void ChangeToOgMouth()
    {
        //Debug.Log("애니메이션 종료");
        _spriteRenderer.sprite = _ogMouth;
    }
}
