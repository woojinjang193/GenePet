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

    public void UpdateGauges(PetStatusCore pet)
    {
        _hungerGauge.value = pet.Hunger;
        _cleanlinessGauge.value = pet.Cleanliness;
        _happinessGauge.value = pet.Happiness;
        _healthGauge.value = pet.Health;
    }
}
