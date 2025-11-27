using UnityEngine;
using UnityEngine.UI;

public class RewardBackground : MonoBehaviour
{
    [SerializeField] private Image _renderer;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(Close);
    }
    public void SetImage(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }
    private void Close()
    {
        gameObject.SetActive(false);
    }
}
