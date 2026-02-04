using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetSpawnButton : MonoBehaviour
{
    private Button _button;
    [SerializeField] private SpawnOptionList _spawnOption;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);

        if(_spawnOption == null )
        {
            _spawnOption = FindObjectOfType<SpawnOptionList>();
        }
    }
    private void OnClicked()
    {
        var eggs = Manager.Save.CurrentData.UserData.EggList;

        _spawnOption.Open();
    }
}

