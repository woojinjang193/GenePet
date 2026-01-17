using System.Collections;
using TMPro;
using UnityEngine;

public class RythmUiManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RythmScoring _scoring;

    [Header("리워드 아이콘")]
    [SerializeField] private SpriteRenderer _rewardIcon;
    [SerializeField] private TMP_Text _rewardAmount;

    [Header("플레이어 목숨")]
    [SerializeField] private GameObject[] _playerHearts;

    [Header("패턴 성공/실패 연출")]
    [SerializeField] private GameObject _successParticle;
    [SerializeField] private GameObject _failParticle;
    [SerializeField] private float _particleLifeTime;

    [Header("판정 연출")]
    [SerializeField] private SpriteRenderer _judgeSpriterenderer;
    [SerializeField] private Sprite _perfectSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _missSprite;

    private void Awake()
    {
        _scoring.OnJudged += HandleJudgeResult;
    }
    public void SetHeart(int amount) //하트 켜줌
    {
        if (amount <= 0) return;
        if (_playerHearts.Length < amount) return;

        for (int i = 0; i < _playerHearts.Length; i++)
        {
            _playerHearts[i].gameObject.SetActive(false);
        }

        for (int i = 0;  i < amount; i++)
        {
            _playerHearts[i].gameObject.SetActive(true);
        }
    }
    public void RemoveHeart() //하트 지움
    {
        foreach (var heart in _playerHearts)
        {
            if(heart.gameObject.activeSelf)
            {
                heart.gameObject.SetActive(false);
                break;
            }
        }
    }
    public void ShowItem(RewardType reward, int amount) //획득 아이템 보여줌
    {
        if (Manager.Item != null)
        {
            _rewardIcon.sprite = Manager.Item.ItemImages.GetItemSprite(reward);
        }
        else
        {
            _rewardIcon.sprite = null;
        }
       
        _rewardAmount.text = amount.ToString();

        _rewardIcon.gameObject.SetActive(true);
    }
    public void HandleJudgeResult(JudgeResult result)
    {
        _judgeSpriterenderer.gameObject.SetActive(false);

        switch (result)
        {
            case JudgeResult.Perfect: _judgeSpriterenderer.sprite = _perfectSprite; break;
            case JudgeResult.Good: _judgeSpriterenderer.sprite = _goodSprite; break;
            case JudgeResult.Miss: _judgeSpriterenderer.sprite = _missSprite; break;
        }

        _judgeSpriterenderer.gameObject.SetActive(true);
    }
    public void PatternSuccess(bool success)
    {
        GameObject particle = success ? _successParticle : _failParticle;

        particle.SetActive(true);
        StartCoroutine(ParticleOffDelay(particle));
    }
    IEnumerator ParticleOffDelay(GameObject particle)
    {
        yield return new WaitForSeconds(_particleLifeTime);
        particle.SetActive(false);
    }
}
