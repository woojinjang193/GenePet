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

    private IAdRequester _adRequester;
    // 광고 ID (테스트 용)
#if UNITY_ANDROID
    private string _bannerAdID = "ca-app-pub-3940256099942544/6300978111";
    private string _rewardAdID = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string _bannerAdID = "ca-app-pub-3940256099942544/2934735716";
    private string _rewardAdID = "ca-app-pub-3940256099942544/1712485313";
#endif

    private BannerView _bannerView;
    private RewardedAd _rewardedAd;

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
                Debug.LogError("[AdManager] Google Mobile Ads initialization failed.");
                return;
            }
            Debug.Log("[AdManager] Google Mobile Ads initialization complete.");

            _isMobileAdsInitialized = true;
            RequestRewarded(); //보상형 광고 미리 로드
            CheckManagerReady();
        });
    }
    //===================배너 광고 띄우기(씬에서 호출)===================
    public void TryRequestBanner() 
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
    //===================리워드 광고 띄우기(씬에서 호출)================
    public void ShowRewardedAd(IAdRequester requester) //보상형 광고 띄우기 (외부 호출용)
    {
        _adRequester = requester;

        if (_rewardedAd != null && _rewardedAd.CanShowAd()) // 광고 객체가 있고 현재 표시 가능 상태라면
        {
            _rewardedAd.Show((Reward reward) => // 광고를 표시하고, 보상 조건 충족 시 콜백 실행
            {
                requester.AdWatched();
                Debug.Log("[AdManager] 리워드 지급");
            });
        }
    }
    private void RequestRewarded() //리워드 광고 미리 준비
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            DestroyRewarded(); //이미 있으면 삭제
        }

        Debug.Log("[AdManager] Loading the rewarded ad.");

        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_rewardAdID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    return;
                }
                _rewardedAd = ad; //광고 등록

                _rewardedAd.OnAdClicked += HandleOnRewardedAdClicked;
                _rewardedAd.OnAdFullScreenContentClosed += HandleOnRewardedAdClosed;
                _rewardedAd.OnAdPaid += HandleOnRewardedAdPaid;
                _rewardedAd.OnAdImpressionRecorded += HandelOnRewardedAdImpression;
            });
    }

    public void DestroyRewarded()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
    }
    public void HandleOnRewardedAdClicked() // 리워드 광고 클릭 시 호출되는 핸들러
    {
        Debug.Log("[AdManager] 광고 클릭");
    }
    public void HandleOnRewardedAdClosed() // 리워드 광고가 닫혔을 때 호출되는 핸들러
    {
        _adRequester.AdClosed();
        _adRequester = null;
        Debug.Log("[AdManager] 리워드 광고 닫힘");

        //광고를 보고 클로즈하면 광고를 다시 요청해서 로드
        RequestRewarded();
    }
    public void HandelOnRewardedAdImpression() // 리워드 광고 노출 기록 시 호출되는 핸들러
    {
        Debug.Log("[AdManager] Rewarded Ad Impression Recorded");
    }
    public void HandleOnRewardedAdPaid(AdValue adValue) // 광고 수익이벤트 발생 시 호출되는 핸들러
    {
        Debug.Log("[AdManager] 수익이벤트 발생");
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
