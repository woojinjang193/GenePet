using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _foodListPanel;
    [SerializeField] private GameObject _cleanListPanel;

    private void Awake()
    {
        if(_button == null)
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        
    }

    private void OnButtonClicked()
    {
        if(_cleanListPanel.activeSelf)
        {
            _cleanListPanel.SetActive(false);
        }
        if (_foodListPanel.activeSelf)
        {
            _foodListPanel.SetActive(false);
        }
        else
        {
            _foodListPanel.SetActive(true);
        }
    }
}
