using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxManager : MonoSingletonGlobal<MaxManager>
{
    public string MaxSDK;
    public string MaxReward;
    public string MaxInter;
    public string MaxBanner;

    public bool _initialized = false;

    private Action Interstitial_Callback;
    private Action Reward_Success_Callback;
    private Action Reward_Fail_Callback;

#if APPLOVIN_ENABLE
    public IEnumerator MaxInitialized()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            Debug.Log("Applovin initialized");
        };

        MaxSdk.SetSdkKey(MaxSDK);
        MaxSdk.InitializeSdk();
        yield return null;
        InitializeBannerAds();
        InitializeInterstitialAds();
        InitializeRewardedAds();
        yield return WaitForSecondCache.WAIT_TIME_ONE;

        _initialized = true;
    }

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320?50 on phones and 728?90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(MaxBanner, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(MaxBanner, Color.white);

        //Start auto-refresh for a banner ad with the following code:
        MaxSdk.StartBannerAutoRefresh(MaxBanner);

        //There may be cases when you would like to stop auto-refresh, for instance, if you want to manually refresh banner ads. To stop auto-refresh for a banner ad, use the following code:
        //MaxSdk.StopBannerAutoRefresh(MaxBanner);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

        //Load banner
        LoadBanner();
        ShowBanner();
    }

    public void LoadBanner()
    {
        MaxSdk.LoadBanner(MaxBanner);
    }

    public void ShowBanner()
    {
        MaxSdk.ShowBanner(MaxBanner);
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(MaxBanner);
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnBannerAdLoadedEvent"); }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { Debug.Log("OnBannerAdLoadFailedEvent"); }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnBannerAdClickedEvent"); }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnBannerAdRevenuePaidEvent"); }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnBannerAdExpandedEvent"); }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnBannerAdCollapsedEvent"); }


    private int retryAttemptInter;
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(MaxInter);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttemptInter = 0;
        Debug.Log("OnInterstitialLoadedEvent");
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttemptInter++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptInter));

        Invoke("LoadInterstitial", (float)retryDelay);
        Debug.Log("OnInterstitialLoadFailedEvent");
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnInterstitialDisplayedEvent"); }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
        Debug.Log("OnInterstitialAdFailedToDisplayEvent");
        Interstitial_Callback?.Invoke();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnInterstitialClickedEvent"); }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
        Debug.Log("OnInterstitialHiddenEvent");
        Interstitial_Callback?.Invoke();
    }

    public void ShowInter(Action Callback)
    {
        if (MaxSdk.IsInterstitialReady(MaxInter))
        {
            Interstitial_Callback = Callback;
            MaxSdk.ShowInterstitial(MaxInter);
        }
        else
        {
            Callback?.Invoke();
        }    
    }


    private int retryAttemptReward;
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(MaxReward);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttemptReward = 0;
        Debug.Log("OnRewardedAdLoadedEvent");
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttemptReward++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptReward));

        Invoke("LoadRewardedAd", (float)retryDelay);
        Debug.Log("OnRewardedAdLoadFailedEvent");
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnRewardedAdDisplayedEvent"); }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
        Debug.Log("OnRewardedAdFailedToDisplayEvent");
        Reward_Fail_Callback?.Invoke();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("OnRewardedAdClickedEvent"); }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        Debug.Log("OnRewardedAdHiddenEvent");
        Reward_Success_Callback?.Invoke();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        Debug.Log("OnRewardedAdReceivedRewardEvent");
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        Debug.Log("OnRewardedAdRevenuePaidEvent");
    }

    public void ShowReward(Action Success, Action Fail)
    {
        if (MaxSdk.IsRewardedAdReady(MaxReward))
        {
            Reward_Success_Callback = Success;
            Reward_Fail_Callback = Fail;
            MaxSdk.ShowRewardedAd(MaxReward);
        }
        else
        {
            Fail?.Invoke();
        }
    }
#endif
}
