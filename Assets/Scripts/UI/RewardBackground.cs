using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardBackground : MonoBehaviour
{
    [SerializeField] private ItemsSO _ItemsSO;
    [SerializeField] private Image _renderer;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _amount;

    private ItemManager _itemManager;
    private void Awake()
    {
        _itemManager = FindObjectOfType<ItemManager>();
        if(_button == null)
        {
            _button = GetComponentInChildren<Button>();
        }
        _itemManager.OnRewardGranted += OnRewardGranted;
        _button.onClick.AddListener(Close);
    }
    private void OnDestroy()
    {
        _itemManager.OnRewardGranted -= OnRewardGranted;
    }
    public void SetImage(Sprite sprite) //알용 (나중에 여유되면 로직 하나로 합치기)
    {
        _renderer.sprite = sprite;
        _amount.text = "";
    }
    public void OnRewardGranted(RewardType type, int amount) //보상 지급시 발생하는 이벤트
    {
        gameObject.SetActive(true);
        _renderer.sprite = GetSprite(type);
        _amount.text = $"X {amount}";
    }
    public Sprite GetSprite(RewardType type)
    {
        switch(type)
        {
            case RewardType.Coin: return _ItemsSO.CoinSprite;
            case RewardType.Energy: return _ItemsSO.Energy;
            case RewardType.RemovedAD: return _ItemsSO.RemoveAdSprite;
            case RewardType.IslandTicket: return _ItemsSO.IslandTicketSprite;
            case RewardType.MissingPoster: return _ItemsSO.MissingPosterSprite;
            case RewardType.GeneticScissors: return _ItemsSO.GeneticScissorsSprite;
            case RewardType.GeneticTester: return _ItemsSO.geneticTesterSprite;
            case RewardType.Snack: return _ItemsSO.SnackSprite;
        }
        return null;
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
