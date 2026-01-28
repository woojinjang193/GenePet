using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomCameraSetter : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GraphicRaycaster _raycaster;

    private void Awake()
    {
        if(_canvas == null )
        {
            _canvas = GetComponent<Canvas>();

        }
        if (_raycaster == null)
        {
            _raycaster = GetComponent<GraphicRaycaster>();
        }
       
        ApplyEventCamera();
    }
    private void ApplyEventCamera()
    {
        if (_canvas == null) return;
        _canvas.worldCamera = GetActiveUICamera();
    }

    private Camera GetActiveUICamera()
    {
        var cams = Resources.FindObjectsOfTypeAll<Camera>();
        for (int i = 0; i < cams.Length; i++)
        {
            if (cams[i].CompareTag("ZoomCamera"))
                return cams[i];
        }

        return Camera.main;
    }
}
