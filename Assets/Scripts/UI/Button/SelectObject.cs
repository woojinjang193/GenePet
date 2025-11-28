using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour, IPointerDownHandler
{
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

    private bool _isDragging = false;
    private GameObject _currentObj;
    
    public void OpenFoodMenu()
    {
        SetButton(true);
    }
    public void OpenShowerMenu()
    {
        SetButton(false);
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
        else if (target == _showerBall.gameObject)
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

    private void SetButton(bool isFood)
    {
        _foodButton.SetActive(isFood);
        _snackButton.SetActive(isFood);
        _medicineButton.SetActive(isFood);

        _showerBallButton.SetActive(!isFood);
    }
}
