using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _text;

    private int _curEnergy;
    private void Awake()
    {
        _slider.maxValue = Manager.Game.Config.MaxEnergy;

        if ( _slider == null )
        {
            _slider = GetComponent<Slider>();
        }
        _curEnergy = Manager.Save.CurrentData.UserData.Energy;
        _slider.value = _curEnergy;
        _text.text = $"{_curEnergy} / {_slider.maxValue}";
    }
    public void SetEnergy(int value)
    {
        _curEnergy = value;
        _slider.value = value;
        _text.text = $"{value} / {_slider.maxValue}";
    }

}
