using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour, IPointerDownHandler
{
    [Header("판넬")]
    [SerializeField] private GameObject _foodListPanel;
    [SerializeField] private GameObject _cleanListPanel;

    [Header("먹이 클릭이미지")]
    [SerializeField] private GameObject _foodButton;
    [SerializeField] private GameObject _snackButton;
    [SerializeField] private GameObject _medicineButton;

    [Header("먹이 오브젝트")]
    [SerializeField] private GameObject _food;
    [SerializeField] private GameObject _snack;
    [SerializeField] private GameObject _medicine;

    [Header("씻기기 클릭이미지")]
    [SerializeField] private GameObject _showerBallButton;

    [Header("샤워도구 오브젝트")]
    [SerializeField] private GameObject _showerBall;

    [Header("버튼")]
    [SerializeField] private Button _feedButton;
    [SerializeField] private Button _cleanButton;

    private bool _isDragging = false;
    private GameObject _currentObj;

    private void Awake()
    {
        _feedButton.onClick.AddListener(OpenFoodList);
        _cleanButton.onClick.AddListener(OpenCleanList);
    }
    private void OnEnable()
    {
        _foodListPanel.SetActive(false);
        _cleanListPanel.SetActive(false);
    }

    private void OpenFoodList()
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
    private void OpenCleanList()
    {
        if (_foodListPanel.activeSelf)
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
    public void OnPointerDown(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (target == _foodButton.gameObject)
        {
            Debug.Log("음식 클릭");
            Spawn(_food);
        }
        else if (target == _medicineButton.gameObject)
        {
            Debug.Log("간식 클릭");
            Spawn(_medicine);
        }
        else if (target == _snackButton.gameObject)
        {
            Debug.Log("약 클릭");
            Spawn(_snack);
        }
        else if (target == _showerBallButton.gameObject)
        {
            Debug.Log("샤워볼 클릭");
            Spawn(_showerBall);
        }
    }
    private void Spawn(GameObject go)
    {
        _currentObj = go;
        _currentObj.SetActive(true);
        _isDragging = true;
    }

    private void Update()
    {
        if (_isDragging && _currentObj != null)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            _currentObj.transform.position = worldPos;
        }

        if (Input.GetMouseButtonUp(0) && _currentObj != null)
        {
            _currentObj.SetActive(false);
            _isDragging = false;
            _currentObj = null;
        }
    }
}
