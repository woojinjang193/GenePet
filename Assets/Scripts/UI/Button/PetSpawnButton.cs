using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetSpawnButton : MonoBehaviour
{
    private Button _button;
    [SerializeField] PetVisualController _visual;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    private void Start()
    {
        //if (!string.IsNullOrWhiteSpace(Manager.Save.CurrentData.UserData.HavePet.ID))
        //{
        //    _visual.VisualizePet();
        //    _button.interactable = false;
        //}
    }

    private void OnEnable()
    {
        Manager.Game.OnPetSpawned += OnPetSpawned;
        Manager.Game.OnPetLeft += OnPetLeft;
    }

    private void OnDisable()
    {
        Manager.Game.OnPetSpawned -= OnPetSpawned;
        Manager.Game.OnPetLeft -= OnPetLeft;
    }

    private void OnPetSpawned()
    {
        _button.interactable = false;
    }

    private void OnPetLeft()
    {
        _button.interactable = true;
    }

    private void OnClicked()
    {
        //if(string.IsNullOrWhiteSpace(Manager.Save.CurrentData.UserData.HavePet.ID))
        //{
        //    Manager.Game.CreateRandomPet(true);
        //    _visual.VisualizePet();
        //}
        //else
        //{
        //    Debug.Log("펫이 이미 있음");
        //}
    }
}

