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
        if (!string.IsNullOrWhiteSpace(Manager.Save.CurrentData.UserData.HavePet.ID))
        {
            _visual.VisualizePet();
            gameObject.SetActive(false);
        }
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
         gameObject.SetActive(false);
    }

    private void OnPetLeft()
    {
        gameObject.SetActive(true);
    }

    private void OnClicked()
    {
        if (Manager.Gene.IsReady == true)
        {
            Manager.Game.CreateRandomPet(true);
            _visual.VisualizePet();
        }
        else
        {
            StartCoroutine(WaitGeneAndCreatePetRoutine());
        }
    }

    private IEnumerator WaitGeneAndCreatePetRoutine()
    {
        while (Manager.Gene.IsReady == false)
        {
            yield return null;
        }
        Manager.Game.CreateRandomPet(true);
        _visual.VisualizePet();
    }

}

