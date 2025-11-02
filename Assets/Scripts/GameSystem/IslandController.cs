using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandController : MonoBehaviour
{
    [SerializeField] private GameObject _pet;
    [SerializeField] private GameObject _islandPet;
    [SerializeField] private GameObject _letter;
    [SerializeField] private GameObject _egg;
    
    [SerializeField] private Button _GoBackHomeButton;

    [SerializeField] private int _visitingPoint;

    private bool _isMarried;
    private bool _isLeft;

    private void Awake()
    {
        _isMarried = Manager.Save.CurrentData.UserData.Island.IsMarried;
        _isLeft = Manager.Save.CurrentData.UserData.Island.IsLeft;

        _GoBackHomeButton.onClick.AddListener(OnClickedGoHome);

        if (_isLeft)
        {
            LeaveIslandPet();
        }
        else if (_isMarried)
        {
            LayEggAndLeave();
        }
    }
    private void OnEnable()
    {
        //시간 제한 둬야함
        if (!_isLeft || !_isMarried)
        {
            Manager.Save.CurrentData.UserData.Island.Affinity += _visitingPoint;
            if (Manager.Save.CurrentData.UserData.Island.Affinity >= 100)
            {
                Manager.Save.CurrentData.UserData.Island.Affinity = 100f;
            }
            Debug.Log($"방문 포인트 +{_visitingPoint}. 현재 호감도 {Manager.Save.CurrentData.UserData.Island.Affinity}");
        }

        if(!_isMarried && !_isLeft)
        {
            if (Manager.Save.CurrentData.UserData.Island.Affinity >= 90)
            {
                Debug.Log($"결혼:{_isMarried}, 떠남:{_isLeft}. 알낳기 시도");
                TryToLayEgg();
            }
        }
    }

    private void OnClickedGoHome()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void LeaveIslandPet()
    {
        _islandPet.SetActive(false);
        _letter.SetActive(true);
    }
    public void LayEggAndLeave()
    {
        _islandPet.SetActive(false);
        _letter.SetActive(false);
        _egg.SetActive(true);
    }
    private void TryToLayEgg()
    {
        float affinity = Manager.Save.CurrentData.UserData.Island.Affinity;
        float value = Random.value;
        Debug.Log($"호감도 {affinity}, 랜덤 벨류 {value}");

        if (value < affinity / 50)
        {
            LayEggAndLeave();
            Manager.Save.CurrentData.UserData.Island.IsMarried = true;
            Debug.Log("알낳기 성공");
        }
        else
        {
            Debug.Log("알낳기 실패");
        }
    }
}
