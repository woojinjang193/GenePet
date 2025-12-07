using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMouth : MonoBehaviour
{
    [SerializeField] private PetController _petController;
    
    [SerializeField] private Sprite _chewMouth;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _ogMouth;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _ogMouth = _spriteRenderer.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            collision.gameObject.SetActive(false);
            _petController.Feed();
            //_animator.SetTrigger("Eat");
            //먹는 사운드 출력
        }
        else if (collision.CompareTag("Snack"))
        {
            collision.gameObject.SetActive(false);
            _petController.Feed();
            //먹는 사운드 출력
        }
        else if(collision.CompareTag("Medicine"))
        {
            _petController.Heal();
            collision.gameObject.SetActive(false);
            Debug.Log("약 먹음");
            //먹는 사운드 출력
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
