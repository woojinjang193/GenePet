using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameListPanel : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private InGameUIManager _uiManager;

    [Header("버튼")]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _PlayButton;

    [Header("이미지")]
    [SerializeField] private Image _spriteRenderer;
    [SerializeField] private Sprite[] _sprites;

    [Header("타이틀")]
    [SerializeField] private TMP_Text _text;

    [Header("최고점수")]
    [SerializeField] private TMP_Text _bestScoreText;

    private int _curIndex = 0;

    private void Awake()
    {
        _leftButton.onClick.AddListener(OnLeftButtonClicked);
        _rightButton.onClick.AddListener(OnRightButtonClicked);
        _PlayButton.onClick.AddListener(OnPlayButtonClicked);
    }
    private void OnEnable()
    {
        if (_sprites == null || _sprites.Length == 0)
        {
            Debug.LogWarning("스프라이트 비었음");
            return;
        }

        int bestScore = Manager.Mini.GetBestScore(MiniGame.Jump);

        _curIndex = 0;
        _text.text = "Jump Jump";
        _bestScoreText.text = bestScore.ToString();

        UpdateUI();
    }
    private void OnLeftButtonClicked()
    {
        if (_sprites == null || _sprites.Length == 0) return;

        _curIndex--;
        if (_curIndex < 0)
        {
            _curIndex = _sprites.Length - 1;
        }

        UpdateUI();
    }
    private void OnRightButtonClicked()
    {
        if (_sprites == null || _sprites.Length == 0) return;

        _curIndex++;
        if (_curIndex >= _sprites.Length)
        {
            _curIndex = 0;
        }

        UpdateUI();
    }
    private void UpdateUI()
    {
        if (_spriteRenderer == null) return;

        if (_sprites[_curIndex] == null)
        {
            Debug.LogWarning($"스프라이트가 null임: index={_curIndex}");
            _spriteRenderer.sprite = null;
            return;
        }

        _spriteRenderer.sprite = _sprites[_curIndex];

        switch(_curIndex)
        {
            case 0: _text.text = "Jump Jump"; _bestScoreText.text = Manager.Mini.GetBestScore(MiniGame.Jump).ToString();  break;
            case 1: _text.text = "Fishing"; _bestScoreText.text = Manager.Mini.GetBestScore(MiniGame.Rythm).ToString(); break;
            case 2: _text.text = "Pinball"; _bestScoreText.text = Manager.Mini.GetBestScore(MiniGame.Pinball).ToString(); break;
        }
    }
    private void OnPlayButtonClicked()
    {
        if (_sprites == null || _sprites.Length == 0) return;

        _uiManager.MiniGameStartButtonClicked(_curIndex);
    }
}
