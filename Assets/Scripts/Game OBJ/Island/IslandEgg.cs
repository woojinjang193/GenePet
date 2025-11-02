using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandEgg : MonoBehaviour
{
    [SerializeField] private GameObject _egg;

    private void OnMouseDown()
    {
        _egg.SetActive(false);
    }
}
