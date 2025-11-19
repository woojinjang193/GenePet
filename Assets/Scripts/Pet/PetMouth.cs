using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMouth : MonoBehaviour
{
    [SerializeField] private PetController _petController;
    //private Animator _animator;
    private void Awake()
    {
        //_animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            collision.gameObject.SetActive(false);
            _petController.Feed();
            Debug.Log("밥 먹음");
            //_animator.SetTrigger("Eat");
            //먹는 사운드 출력
        }
        else if (collision.CompareTag("Snack"))
        {
            collision.gameObject.SetActive(false);
            _petController.Feed();
            Debug.Log("간식 먹음");
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
}
