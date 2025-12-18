using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IslandGiftSelector : MonoBehaviour, IPointerDownHandler
{
    [Header("선물 리스트 판넬")]
    [SerializeField] private GameObject _giftListPanel;

    [Header("선물 이미지버튼 이미지")]
    [SerializeField] private Image _gift1Button;
    [SerializeField] private Image _gift2Button;
    [SerializeField] private Image _gift3Button;
    [SerializeField] private Image _gift4Button;

    [Header("선물 소지 개수 텍스트")]
    [SerializeField] private TMP_Text _gift1Amount;
    [SerializeField] private TMP_Text _gift2Amount;
    [SerializeField] private TMP_Text _gift3Amount;
    [SerializeField] private TMP_Text _gift4Amount;

    [Header("소환될 선물 오브젝트")]
    [SerializeField] private GameObject _gift1;
    [SerializeField] private GameObject _gift2;
    [SerializeField] private GameObject _gift3;
    [SerializeField] private GameObject _gift4;

    [Header("옵션 오픈 버튼")]
    [SerializeField] private Button _giftButton;

    [Header("섬 매니저")]
    [SerializeField] private IslandManager _islandManager;

    private bool _isDragging = false;
    private GameObject _currentObj;

    private void Awake()
    {
        if(_islandManager ==  null)
        {
            _islandManager = FindObjectOfType<IslandManager>();
        }
        UpdateAmount();
        SetGiftSprite();
        _giftButton.onClick.AddListener(OpenFoodList);

        Manager.Item.OnGiftAmountChanged += UpdateAmount;
    }
    private void OnDestroy()
    {
        if(Manager.Item)
        {
            Manager.Item.OnGiftAmountChanged -= UpdateAmount;
        }
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
        if (_islandManager.IslandMypetData.IsLeft) return;

        var target = eventData.pointerCurrentRaycast.gameObject;
        var item = Manager.Save.CurrentData.UserData.Items;

        if (target == _gift1Button.gameObject)
        {
            if (item.Gift1 <= 0) return;
            Spawn(_gift1);
        }
        else if (target == _gift2Button.gameObject)
        {
            if (item.Gift2 <= 0) return;
            Spawn(_gift2);
        }
        else if (target == _gift3Button.gameObject)
        {
            if (item.Gift3 <= 0) return;
            Spawn(_gift3);
        }
        else if (target == _gift4Button.gameObject)
        {
            if (item.Gift4 <= 0) return;
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

    private void UpdateAmount()
    {
        var item = Manager.Save.CurrentData.UserData.Items;

        _gift1Amount.text = $"X {item.Gift1.ToString()}";
        _gift2Amount.text = $"X {item.Gift2.ToString()}";
        _gift3Amount.text = $"X {item.Gift3.ToString()}";
        _gift4Amount.text = $"X {item.Gift4.ToString()}";
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
