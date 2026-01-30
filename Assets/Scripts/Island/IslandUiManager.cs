using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandUiManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _panelsToClose;
    [SerializeField] private RewardPopUp _rewardPopUp;

    private void Awake()
    {
        if(Manager.Item != null)
        {
            Manager.Item.OnRewardsGiven += OffUisAfterGetEgg; //보상 팝업시 처리
        }
        _rewardPopUp.OnEndQeueu += MoveToMainScene; //보상 팝업 끝난후 처리
    }
    private void OnDestroy()
    {
        if (Manager.Item != null)
        {
            Manager.Item.OnRewardsGiven -= OffUisAfterGetEgg;
        }
        _rewardPopUp.OnEndQeueu -= MoveToMainScene;
    }
    private void OffUisAfterGetEgg() //UI 꺼주기
    {
        for (int i = 0; i < _panelsToClose.Length; i++)
        {
            _panelsToClose[i].gameObject.SetActive(false);
        }
    }
    private void MoveToMainScene()
    {
        SceneManager.LoadScene("InGameScene");
    }
}
