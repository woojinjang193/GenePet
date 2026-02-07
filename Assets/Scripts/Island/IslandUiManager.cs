using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandUiManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _panelsToClose;
    [SerializeField] private RewardPopUp _rewardPopUp;
    [SerializeField] private TutorialController _tutorial;
    [SerializeField] private EggObj _egg;

    private bool _isEggClicked = false;
    private void Awake()
    {
        if(Manager.Item != null)
        {
            Manager.Item.OnRewardsGiven += OffUisAfterGetEgg; //보상 팝업시 처리
        }
        _rewardPopUp.OnEndQeueu += MoveToMainScene; //보상 팝업 끝난후 처리
        _egg.OnClicked += OnGetEgg;
    }
    private void Start()
    {
        Manager.Audio.PlayBGM("BGM_Island");
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
        {
            Manager.Item.OnRewardsGiven -= OffUisAfterGetEgg;
        }
        _rewardPopUp.OnEndQeueu -= MoveToMainScene;
        _egg.OnClicked -= OnGetEgg;
    }
    private void OffUisAfterGetEgg() //UI 꺼주기
    {
        if (_tutorial.IsRunning || !_isEggClicked) return;
        for (int i = 0; i < _panelsToClose.Length; i++)
        {
            _panelsToClose[i].gameObject.SetActive(false);
        }
    }
    private void MoveToMainScene()
    {
        if (_tutorial.IsRunning || !_isEggClicked) return;

        SceneManager.LoadScene("InGameScene");
    }
    private void OnGetEgg()// 알 주우면 이벤트 호출
    {
        _isEggClicked = true;
    }
}
