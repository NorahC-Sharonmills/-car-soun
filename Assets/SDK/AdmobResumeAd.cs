using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public partial class AdmobManager : MonoSingletonGlobal<AdmobManager>
{
    [Header("--- Resume ---")]
    public string OpenAdsId = "";
    private AppOpenAd ad;
    private bool IsShowingAd = false;
    public bool IsOpenAd = false;
    public bool IsOpenAdShow = false;
    public StatusAd ResumeAd = StatusAd.Mising;

    private int ResumeAdMatchRequest = 0;
    private int ResumeAdImpression = 0;

    private bool IsAdAvailable
    {
        get
        {
            //if (ad == null) InitializedOpenAd();
            return ad != null;
        }
    }

    float TimeResumeAd = 0;
    public void OnUpdateRAdesume()
    {
        TimeResumeAd += Time.deltaTime;
    }

    public void ApplicationPauseResumeAd(bool pauseStatus)
    {
        Debug.Log("Application status : " + (pauseStatus ? "Pause" : "Resume"));
        if (!pauseStatus)
        {
            // Show the app open ad.
            FocusApp();
        }
    }

    public void InitializedOpenAd()
    {
        //if (appTest)
        //    OpenAdsId = GetId("AppOpen");
    }

    public void InitializedOpenAd(Action<AppOpenAd> ReadyAd, Action MissAd)
    {
        //if (appTest)
        //    OpenAdsId = GetId("AppOpen");

        if (ad == null && ResumeAd == StatusAd.Mising || ad != null && !ad.IsLoaded() && ResumeAd == StatusAd.Mising)
        {
            ResumeAd = StatusAd.Loading;
            AdRequest request = new AdRequest.Builder().Build();
            // Load an app open ad for portrait orientation
            AppOpenAd.LoadAd(OpenAdsId, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
            {
                if (error != null)
                {
                    MissAd?.Invoke();
                    // Handle the error.
                    Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                    return;
                }

                ad = appOpenAd;
                ReadyAd?.Invoke(appOpenAd);
                ResumeAdMatchRequest += 1;
                // App open ad is loaded.
                ResumeAd = StatusAd.Ready;
            }));
        }
    }

    private void FocusApp()
    {
        HideLoadingAds();
        if (adResumeAwait != null)
            StopCoroutine(adResumeAwait);

        //ShowAdIfAvailable();
        if (TimeResumeAd > 1f)
            AsynsShowResumeAd();

        if (adInterAwait != null)
            StopCoroutine(adInterAwait);

        if (adRewardAwait != null)
            StopCoroutine(adRewardAwait);
    }

    private void AsynsShowResumeAd()
    {
        if(IsShowingAd || !IsOpenAd) return;
        IsOpenAdShow = true;
        ShowLoadingAds();
        HideBanner();
        InitializedOpenAd((reponse) => {
            TimeResumeAd = 0;
            ResumeAdImpression += 1;
            HideLoadingAds();
            reponse.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
            reponse.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
            reponse.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
            reponse.OnAdDidRecordImpression += HandleAdDidRecordImpression;
            reponse.OnPaidEvent += HandlePaidEvent;
            reponse.Show();
        }, () => {
            Debug.Log("Closed app open ad");
            ad = null;
            ResumeAd = StatusAd.Mising;
            IsShowingAd = false;
            IsOpenAdShow = false;
            ConitinuteShowBanner();
        });
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        ResumeAd = StatusAd.Mising;
        IsShowingAd = false;
        IsOpenAdShow = false;
        //InitializedOpenAd();
        //ScreenHelper.AppResume();
        ConitinuteShowBanner();
        //AppOpenAdBackground.SetActive(false);
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        ResumeAd = StatusAd.Mising;
        //InitializedOpenAd();
        //AppOpenAdBackground.SetActive(true);
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Displayed app open ad");
        IsShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.AdValue.CurrencyCode, args.AdValue.Value);
    }
}
