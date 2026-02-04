using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerRequester : MonoBehaviour
{
    private void Awake()
    {
        Manager.AD.OnAdRemoved += CloseAdPanel;

        if(Manager.AD.IsAdRemoved) //광고 제거 구매상태이면
        {
            CloseAdPanel(); //판넬 닫음
        }
        else //광고 제거 구매 안했으면
        {
            Manager.AD.TryRequestBanner(); //광고 요청
        }
    }
    private void OnDestroy()
    {
        if(Manager.Shop != null)
        Manager.AD.OnAdRemoved -= CloseAdPanel;
    }
    private void CloseAdPanel()
    {
        gameObject.SetActive(false);
    }
}
