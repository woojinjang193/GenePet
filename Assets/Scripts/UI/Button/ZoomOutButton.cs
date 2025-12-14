using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomOutButton : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private PetManager _petManager;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _cameraController.CameraZoomOut();
        _petManager.ZoomOutPet();
    }

}
