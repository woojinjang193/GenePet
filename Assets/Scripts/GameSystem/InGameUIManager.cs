using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("줌아웃 버튼")]
    [SerializeField] private GameObject _zoomOutButton;

    private CameraController _camera;

    private void Awake()
    {
        _camera = FindObjectOfType<CameraController>();
    }
}
