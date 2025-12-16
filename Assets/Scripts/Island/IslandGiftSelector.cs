using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IslandGiftSelector : MonoBehaviour
{
    [Header("선물 리스트 판넬")]
    [SerializeField] private GameObject _giftListPanel;

    [Header("선물 이미지버튼 이미지")]
    [SerializeField] private SpriteRenderer _gift1Button;
    [SerializeField] private SpriteRenderer _gift2Button;
    [SerializeField] private SpriteRenderer _gift3Button;
    [SerializeField] private SpriteRenderer _gift4Button;

    [Header("소환될 선물 오브젝트")]
    [SerializeField] private GameObject _gift1;
    [SerializeField] private GameObject _gift2;
    [SerializeField] private GameObject _gift3;
    [SerializeField] private GameObject _gift4;

    [Header("옵션 오픈 버튼")]
    [SerializeField] private Button _giftButton;

    private bool _isDragging = false;
    private GameObject _currentObj;

    private void Awake()
    {
        SetGiftSprite();
        _giftButton.onClick.AddListener(OpenFoodList);
    }
    private void OnEnable()
    {
        _giftListPanel.SetActive(false);
    }

    private void OpenFoodList()
    {
        if (_giftListPanel.activeSelf)
        {
            _giftListPanel.SetActive(false);
        }
        else
        {
            _giftListPanel.SetActive(true);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (target == _gift1Button.gameObject)
        {
            Debug.Log("Gift 1");
            Spawn(_gift1);
        }
        else if (target == _gift2Button.gameObject)
        {
            Debug.Log("Gift 2");
            Spawn(_gift2);
        }
        else if (target == _gift3Button.gameObject)
        {
            Debug.Log("Gift 3");
            Spawn(_gift3);
        }
        else if (target == _gift4Button.gameObject)
        {
            Debug.Log("Gift 4");
            Spawn(_gift4);
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

    private void SetGiftSprite()
    {
        if(Manager.Item == null) return;

        var icon = Manager.Item.ItemImages;

        _gift1Button.sprite = icon.Gift1;
        _gift2Button.sprite = icon.Gift2;
        _gift3Button.sprite = icon.Gift3;
        _gift4Button.sprite = icon.Gift4;
    }
}
