using UnityEngine;
using UnityEngine.UI;

public class OpenMoreOptions : MonoBehaviour
{
    [Header("더보기 버튼")]
    [SerializeField] private Button _button;

    [SerializeField] private GameObject _optionPanel;
    //[SerializeField] private GameObject _foodSelectBar;
    //[SerializeField] private GameObject _cleanSeletBar;

    private void Awake()
    {
        _button.onClick.AddListener(OnClicked);
    }
    private void OnEnable()
    {
        if (_optionPanel.activeSelf)
        {
            _optionPanel.SetActive(false);
        }
    }
    private void OnClicked()
    {
        //if(_foodSelectBar.activeSelf) { _foodSelectBar.SetActive(false); }
        //if(_cleanSeletBar.activeSelf) { _cleanSeletBar.SetActive(false); }

        if (_optionPanel.activeSelf)
        {
            _optionPanel.SetActive(false);
        }
        else
        {
            _optionPanel.SetActive(true);
        }
    }

}
