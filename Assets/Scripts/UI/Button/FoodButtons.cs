using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodButtons : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject _foodButton;
    [SerializeField] private GameObject _snackButton;
    [SerializeField] private GameObject _medicineButton;

    [SerializeField] private GameObject _food;
    [SerializeField] private GameObject _snack;
    [SerializeField] private GameObject _medicine;

    private bool _isDragging = false;
    private GameObject _currentFood;

    public void OnPointerDown(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (target == _foodButton.gameObject)
        {
            Debug.Log("음식 클릭");
            SpawnFood();
        }
        else if (target == _medicineButton.gameObject)
        {
            Debug.Log("간식 클릭");
            SpawnMedicine();
        }
        else if (target == _snackButton.gameObject)
        {
            Debug.Log("약 클릭");
            SpawnSnack();
        }
    }

    private void SpawnFood()
    {
        _currentFood = _food;
        _currentFood.SetActive(true);
        _isDragging = true;
    }
    private void SpawnSnack()
    {
        _currentFood = _snack;
        _currentFood.SetActive(true);
        _isDragging = true;
    }
    private void SpawnMedicine()
    {
        _currentFood = _medicine;
        _currentFood.SetActive(true);
        _isDragging = true;
    }

    private void Update()
    {
        if (_isDragging && _currentFood != null)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            _currentFood.transform.position = worldPos;
        }

        if (Input.GetMouseButtonUp(0) && _currentFood != null)
        {
            _currentFood.SetActive(false);
            _isDragging = false;
            _currentFood = null;
        }
    }
}
