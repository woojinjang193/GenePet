using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    [SerializeField] private GameObject _letterPanel;

    private void Awake()
    {
        
    }
    public void OnMouseDown()
    {
        _letterPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
