using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetSpawnButton : MonoBehaviour
{
    private Button _button;
    [SerializeField] private EggListUI _eggList;
    [SerializeField] private GameObject _popUp;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);

        if(_eggList == null )
        {
            _eggList = FindObjectOfType<EggListUI>();
        }
    }
    private void OnClicked()
    {
        var eggs = Manager.Save.CurrentData.UserData.EggList;

        _eggList.Open();

        //if (eggs.Count > 0)
        //{
        //    _eggList.Open();
        //}
        //else
        //{
        //    Manager.Game.CreateRandomPet(true);
        //}
    }
}

