using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

[System.Serializable]
public class PositionAd
{
    public bool IsPreventive;
    public string Position;
    public string Id;
    public string IdPreventive;
}

[System.Serializable]
public class TestIdAd
{
    public string Title;
    public string Id;
}

public partial class AdmobManager : MonoSingletonGlobal<AdmobManager>
{
    public enum StatusAd
    {
        Ready,
        Mising,
        Loading
    }

    [Header("--- Manager ---")]
    public bool appTest = false;
    public bool DisableAd = false;
    public TestIdAd[] testIdAds;

    public string AppId = "";
    public bool IsLanguage = false;
    public bool IsTutorial = false;

    public GameObject AppOpenAdBackground;
    public GameObject AdBeforOpen;
    public GameObject BannerLoadingAd;

    public static float minTimeRequestAd = 10f;
    public static float maxTimeRequestAd = 30f;

    public bool IsNetwork()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;
        else
            return true;
    }

    private void ShowLoadingAds()
    {
        AdBeforOpen.SetActive(true);
#if UNITY_EDITOR
        CoroutineUtils.PlayCoroutine(() =>
        {
            AdBeforOpen.SetActive(false);
        }, 1.5f);
#endif
    }

    private void HideLoadingAds()
    {
        AdBeforOpen.SetActive(false);
    }

    public string GetDemoId(string _title)
    {
        for(int i = 0; i < testIdAds.Length; i++)
        {
            if (testIdAds[i].Title == _title)
                return testIdAds[i].Id;
        }
        return "";
    }

    public IEnumerator AdmobInitialized()
    {
        List<string> deviceIds = new List<string>();
        deviceIds.Add("2FA3A00E90CC61A6CB020EF9F799CFA5");
        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => {
            Debug.Log("Admob initialized status" + initStatus);
        });

        if(appTest)
        {
            for(int i = 0; i < positionBannerAds.Length; i++)
            {
                positionBannerAds[i].Id = GetDemoId("Banner");
            }

            for(int i = 0; i < positionInterAds.Length; i++)
            {
                positionInterAds[i].Id = GetDemoId("Interstitial");
                if (positionInterAds[i].IsPreventive)
                    positionInterAds[i].IdPreventive = GetDemoId("Interstitial");
            }

            for(int i = 0; i < positionAds.Length; i++)
            {
                positionAds[i].Id = GetDemoId("Rewarded");
            }

            for(int i = 0; i < positionNativeAd.Length; i++)
            {
                positionNativeAd[i].Id = GetDemoId("Native");
                if (positionNativeAd[i].IsPreventive)
                    positionNativeAd[i].IdPreventive = GetDemoId("Native");
            }

            OpenAdsId = GetDemoId("AppOpen");
        }

        yield return null;
        InitializedReward();
        InitializedInter();
        InitializedBanner();
        InitializedOpenAd();
        InitializedNativeAd();

        if (appTest)
            yield return WaitForSecondCache.WAIT_TIME_THREE;
        else
            yield return WaitForSecondCache.WAIT_TIME_ONE;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        //Debug.Log("OnApplicationPause " + pauseStatus);
        ApplicationPauseResumeAd(pauseStatus);
    }

    private Coroutine adResumeAwait;
    private Coroutine adInterAwait;
    private Coroutine adRewardAwait;

    int IntAdMobTime = 0;
    float FloatAdMobTime = 0;

    private void Update()
    {
        OnUpdateAdBackground();
        OnUpdateInterAd();
        OnUpdateRewardAd();
        OnUpdateBannerAd();
        OnUpdateRAdesume();

#if !UNITY_EDITOR
        FloatAdMobTime += Time.deltaTime;
        if(FloatAdMobTime > IntAdMobTime)
        {
            IntAdMobTime += 1;
            Debug.Log($"Inter {InterTotalMatchedRequest}/{InterTotalImpresstion} Banner {BannerTotalMatchedRequest}/{BannerTotalImpresstion} Reward {RewardTotalMatchedRequest}/{RewardTotalImpresstion} Resume {ResumeAdMatchRequest}/{ResumeAdImpression}");
        }
#endif
    }

    private void OnUpdateAdBackground()
    {
        if (AppOpenAdBackground.active != IsOpenAdShow)
            AppOpenAdBackground.SetActive(IsOpenAdShow);
    }
}
