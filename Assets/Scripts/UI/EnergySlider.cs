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
        if( _slider == null )
        {
            _slider = GetComponent<Slider>();
        }
        _curEnergy = Manager.Save.CurrentData.UserData.Energy;
        _slider.value = _curEnergy;
        _text.text = $"{_curEnergy} / {_slider.maxValue}";
    }

    public void UpdateEnergu(int amount)
    {
        _curEnergy += amount;
        _slider.value = Mathf.Clamp(_curEnergy, _slider.minValue, _slider.maxValue) ;
        _text.text = _curEnergy.ToString();
    }
}
