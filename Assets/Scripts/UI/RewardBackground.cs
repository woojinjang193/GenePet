using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardBackground : MonoBehaviour
{
    [SerializeField] private Image _renderer;
    [SerializeField] private Button _button;

    private void Awake()
    {
        if(_button == null)
        {
            _button = GetComponentInChildren<Button>();
        }
        _button.onClick.AddListener(Close);
    }
    public void SetImage(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }
    private void Close()
    {
        if (SceneManager.GetActiveScene().name == "IslandScene")
        {
            SceneManager.LoadScene("InGameScene");
        }
        gameObject.SetActive(false);
    }
}
