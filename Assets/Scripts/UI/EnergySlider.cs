using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private PetManager _petManager;

    private int _curEnergy;
    private void Awake()
    {
        if (_petManager == null)
        {
            _petManager = FindObjectOfType<PetManager>();
        }

        _slider.maxValue = Manager.Game.Config.MaxEnergy;

        if ( _slider == null )
        {
            _slider = GetComponent<Slider>();
        }
        _curEnergy = Manager.Save.CurrentData.UserData.Energy;
        _slider.value = _curEnergy;
        _text.text = $"{_curEnergy} / {_slider.maxValue}";

        Manager.Item.OnRewardGranted += UpdateUI;
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
        {
            Manager.Item.OnRewardGranted += UpdateUI;
        }
    }
    public void SetEnergy(int value)
    {
        _curEnergy = value;
        _slider.value = value;
        _text.text = $"{value} / {_slider.maxValue}";
    }

    private void UpdateUI(RewardType type, int newValue)
    {
        if (type != RewardType.Energy) return;

        SetEnergy(newValue);
    }
}
