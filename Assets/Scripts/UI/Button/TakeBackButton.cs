using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeBackButton : MonoBehaviour
{
    [SerializeField] private PetUnit _pet;
    [SerializeField] private GameObject _letterPanel;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        var userData = Manager.Save.CurrentData.UserData;

        if (userData.Items.MissingPoster > 0)
        {
            _pet.Status.SetStat(PetStat.Hunger, 80);
            _pet.Status.SetStat(PetStat.Happiness, 80);
            _pet.Status.SetStat(PetStat.Energy, 100);
            _pet.Status.SetStat(PetStat.Cleanliness, 50);
            _pet.Status.SetStat(PetStat.Health, 100);

            _pet.Status.SetFlag(PetFlag.IsLeft, false);
            _pet.gameObject.transform.position = new Vector3(0, -1, 0);
            _letterPanel.SetActive(false);
            Debug.Log("펫 복귀시킴");
        }
    }
}
