using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Growing : MonoBehaviour
{
    [SerializeField] private PetController _petController;

    private int _timer;
    private int _timeToGrow;

    private void Awake()
    {
        if (_petController == null)
        {
            _petController = GetComponent<PetController>();
        }
    }


}
