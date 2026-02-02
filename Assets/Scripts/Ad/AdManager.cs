using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;

public class AdManager : Singleton<AdManager>
{
    private bool _isMobileAdsInitialized = false;
    private bool _isAdRemoveFlagSet = false;
    public bool IsReady { get; private set; }
    public bool IsAdRemoved { get; private set; }

    // 광고 ID (테스트 용)
#if UNITY_ANDROID
    private string _bannerAdID = "ca-app-pub-3940256099942544/6300978111";
    private string _rewardAdID = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string _bannerAdID = "ca-app-pub-3940256099942544/2934735716";
    private string _rewardAdID = "ca-app-pub-3940256099942544/1712485313";
#endif

    BannerView _bannerView;
    public event Action OnAdRemoved;
    protected override void Awake()
    {
        base.Awake();
        CheckRemoveAD();

        if (Manager.Shop != null)
            Manager.Shop.OnRemoveAdPurchased += PurchasedAdRemove;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Manager.Shop != null)
            Manager.Shop.OnRemoveAdPurchased -= PurchasedAdRemove;
    }
    private void Start()
    {
        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                return;
            }
            Debug.Log("Google Mobile Ads initialization complete.");

            _isMobileAdsInitialized = true;
            CheckManagerReady();
        });
    }

    public void TryRequestBanner() //광고 띄우기 (씬에서 호출)
    {
        if (!_isMobileAdsInitialized) return; //광고 초기화 아직이면 리턴
        if (IsAdRemoved) return; // //광고제거상태면 리턴

        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(_bannerAdID, adaptiveSize, AdPosition.Bottom);

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        _bannerView.LoadAd(adRequest);
        Debug.Log("[AdManager] 배너 로드");
    }
  
    //===================광고 제거 관련=====================
    private void CheckRemoveAD()
    {
        if (Manager.Save != null)
        {
            IsAdRemoved = Manager.Save.CurrentData.UserData.Items.IsAdRemoved;
            _isAdRemoveFlagSet = true;
            Debug.Log($"[AdManager] 광고제거 여부 : {IsAdRemoved}");
        }
        else
        {
            StartCoroutine(WaitForSave());
            Debug.Log($"[AdManager] 세이브 데이터 기다림");
        }
    }
  private IEnumerator WaitForSave() //세이브 데이터 기다리기
    {
        while (Manager.Save.CurrentData.UserData.Items == null)
        {
            yield return null;
        }

        IsAdRemoved = Manager.Save.CurrentData.UserData.Items.IsAdRemoved;
        _isAdRemoveFlagSet = true;
        Debug.Log($"[AdManager] 광고제거 여부 : {_isAdRemoveFlagSet}");
    }
    //--------------광고제거 구매시------------------
    private void PurchasedAdRemove() 
    {
        IsAdRemoved = true;
        Manager.Save.CurrentData.UserData.Items.IsAdRemoved = true;
        RemoveBanner();
    }
    private void RemoveBanner() //배너 지우기
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
        OnAdRemoved?.Invoke();
    }
 
    //===============배너 숨김/표시==================
    public void ShowBanner()
    {
        if (_bannerView == null) return;

        _bannerView.Show();
        Debug.Log("[AdManager] 배너 Show");
    }
    public void HideBanner()
    {
        if (_bannerView == null) return;

        _bannerView.Show();
        Debug.Log("[AdManager] 배너 Hide");
    }
    //==============매니저 준비 체크===================
    private void CheckManagerReady()
    {
        if (!_isMobileAdsInitialized|| !_isAdRemoveFlagSet) return;
        IsReady = true;
    }
}
