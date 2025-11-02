using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectPanel;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (_selectPanel.activeSelf)
        {
            _selectPanel.SetActive(false);
        }
        else
        {
            _selectPanel.SetActive(true);
        }
    }
}
