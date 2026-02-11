using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandPetMouthController : MonoBehaviour
{
    [SerializeField] private Sprite _chewMouth;
    [SerializeField] private Sprite _happyEyes;
    [SerializeField] private SpriteRenderer _mouthRenderer;
    [SerializeField] private SpriteRenderer _eyeRenderer;
    private Animator _anim;

    private Sprite _ogMouth;
    private Sprite _ogEyes;

    public Action<Gift> OnGiveTaken;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _mouthRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _ogMouth = _mouthRenderer.sprite;
        _ogEyes = _eyeRenderer.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"충돌: {collision.gameObject.name}");
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
        else if (collision.CompareTag("MasterGift"))
        {
            collision.gameObject.SetActive(false);
            OnGiveTaken?.Invoke(Gift.MasterGift);
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
    //=====애니메이션 에서 호출==========
    public void ChangeToChewMouth()
    {
        //Debug.Log("애니메이션 시작");
        _mouthRenderer.sprite = _chewMouth;
        _eyeRenderer.sprite = _happyEyes;
    }
    public void ChangeToOgMouth()
    {
        //Debug.Log("애니메이션 종료");
        _mouthRenderer.sprite = _ogMouth;
        _eyeRenderer.sprite = _ogEyes;
    }
}
