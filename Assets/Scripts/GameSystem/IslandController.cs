using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandController : MonoBehaviour
{
    [SerializeField] private GameObject _pet;
    [SerializeField] private GameObject _islandPet;
    [SerializeField] private Button _egg;
    [SerializeField] private Button _letter;
    [SerializeField] private Button _GoBackHomeButton;

    [SerializeField] private int _visitingPoint;
    private void Awake()
    {
        _letter.enabled = false;
        _GoBackHomeButton.onClick.AddListener(OnClickedGoHome);
    }
    private void OnEnable()
    {
        //시간 제한 둬야함
        float affinity = Manager.Save.CurrentData.UserData.Island.Affinity;
        Manager.Save.CurrentData.UserData.Island.Affinity += _visitingPoint;
        Debug.Log($"방문 포인트 +{_visitingPoint}. 현재 호감도 {affinity}");
    }

    private void OnClickedGoHome()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void LeaveIslandPet()
    {
        _islandPet.SetActive(false);
        _letter.enabled = true;
    }

    public void LayEggAndLeave()
    {
        _islandPet.SetActive(false);
        _letter.enabled = true;
    }
}
