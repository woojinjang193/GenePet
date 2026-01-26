using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Slider _hungerGauge;
    [SerializeField] private Slider _cleanlinessGauge;
    [SerializeField] private Slider _happinessGauge;
    [SerializeField] private Slider _healthGauge;
    [SerializeField] private Slider _expBar;

    public void UpdateGauges(PetStatusCore pet, float maxEXP)
    {
        //100 기준 게이지들
        _hungerGauge.value = pet.Hunger;
        _cleanlinessGauge.value = pet.Cleanliness;
        _happinessGauge.value = pet.Happiness;
        _healthGauge.value = pet.Health;

        //경험치 바
        GrowthStatus growth = pet.Growth;
        _expBar.maxValue = maxEXP;
        _expBar.value = pet.GrowthExp;
    }
}
