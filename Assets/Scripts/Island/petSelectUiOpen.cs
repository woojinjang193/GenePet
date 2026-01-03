//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//
//public class petSelectUiOpen : MonoBehaviour, IConfirmRequester
//{
//    [SerializeField] private IslandManager _islandManager;
//    [SerializeField] private Button _petSelectUiButton;
//    [SerializeField] private GameObject _petSelectUI;
//    [SerializeField] private ConfirmMessage _confirmMessage;
//
//    private void Awake()
//    {
//        _petSelectUiButton.onClick.AddListener(OnPetSlectUiButtonClicked);
//    }
//
//    private void OnPetSlectUiButtonClicked()
//    {
//        if(_islandManager.IslandMypetData != null)
//        {
//            if (_islandManager.IslandMypetData.IsLeft)
//            {
//                _confirmMessage.OpenConfirmUI(Confirm.ChangingIslandMyPet, this);
//                return;
//            }
//        }
//
//        _petSelectUI.SetActive(true);
//    }
//    public void Confirmed()
//    {
//        _petSelectUI.SetActive(true);
//    }
//    public void Canceled()
//    {
//
//    }
//}
