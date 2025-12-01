using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _cleanListPanel;
    [SerializeField] private GameObject _foodListPanel;
    private void Awake()
    {
        if (_button == null)
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if(_foodListPanel.activeSelf)
        {
            _foodListPanel.SetActive(false);
        }
        if (_cleanListPanel.activeSelf)
        {
            _cleanListPanel.SetActive(false);
        }
        else
        {
            _cleanListPanel.SetActive(true);
        }
    }
}
