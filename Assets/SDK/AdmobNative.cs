using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

[System.Serializable]
public class CustomNativeAd
{
    public int MatchLoadAd = 0;
    public int MatchRequest = 0;
    public int MatchImpression = 0;

    public string positionAd;
    public string idAd = "";
    public NativeAd adNative;
    public Action NativeAd_OnNativeAdLoaded;
    public Action NativeAd_OnAdFailedToLoad;
    public AdmobManager.StatusAd statusAd = AdmobManager.StatusAd.Mising;

    public CustomNativeAd(string adUnitId, string adPosition)
    {
        idAd = adUnitId;
        positionAd = adPosition;
        LoadAd();
    }

    public void LoadAd()
    {
        if(statusAd == AdmobManager.StatusAd.Mising)
        {
            MatchRequest += 1;
            Debug.Log("Native Ad - Loading " + positionAd);
            statusAd = AdmobManager.StatusAd.Loading;
            AdLoader adLoader = new AdLoader.Builder(idAd).ForNativeAd().Build();
            adLoader.OnNativeAdLoaded += (sender, args) =>
            {
                Debug.Log("Native Ad - Loaded Ad Complete " + positionAd);
                MatchLoadAd += 1;
                statusAd = AdmobManager.StatusAd.Ready;
                adNative = args.nativeAd;
            };
            adLoader.OnAdFailedToLoad += (sender, args) =>
            {
                statusAd = AdmobManager.StatusAd.Mising;
                LoadAd();
            };
            adLoader.LoadAd(new AdRequest.Builder().Build());
        }
    }

    public bool IsLoaded()
    {
        return statusAd == AdmobManager.StatusAd.Ready ? true : false;
    }
}

public partial class AdmobManager : MonoSingletonGlobal<AdmobManager>
{
    [Header("--- Native ---")]
    public PositionAd[] positionNativeAd;
    public List<CustomNativeAd> customNativeAds;
    public void InitializedNativeAd()
    {
        Debug.Log("Loading Ad Native");
        customNativeAds = new List<CustomNativeAd>();
        bool IsLanguage = PrefManager.GetBool(Manager.IsLanguage);
        if (!IsLanguage)
            InitializedNativeAd("NativeLanguage");

        bool IsTutorial = PrefManager.GetBool(Manager.IsTutorial);
        if (!IsTutorial)
            InitializedNativeAd("NativeTutorial");
    }

    private void InitializedNativeAd(string positionAd)
    {
        var nativeId = NativeGetId(positionAd);
        if (nativeId.IsPreventive)
        {
            CustomNativeAd nativeAd1 = new CustomNativeAd(nativeId.Id, nativeId.Position);
            customNativeAds.Add(nativeAd1);
            CustomNativeAd nativeAd2 = new CustomNativeAd(nativeId.IdPreventive, nativeId.Position);
            customNativeAds.Add(nativeAd2);

            AsynsGetNativeAd(positionAd, (_nativeAd) => GameEvent.OnNativeLoadedMethod(_nativeAd.adNative));
        }
        else
        {
            CustomNativeAd nativeAd1 = new CustomNativeAd(nativeId.Id, nativeId.Position);
            customNativeAds.Add(nativeAd1);
        }
    }

    private PositionAd NativeGetId(string positionAd)
    {
        for(int i = 0; i < positionNativeAd.Length; i++)
        {
            if (positionNativeAd[i].Position == positionAd)
                return positionNativeAd[i];
        }
        return null;
    }

    private List<CustomNativeAd> FindNativeAd(string positionAd)
    {
        var lstNativeAd = customNativeAds.FindAll(x => x.positionAd == positionAd);
        return lstNativeAd;
    }

    public bool NativeIsReady(string Position)
    {
        var lstNativeAd = FindNativeAd(Position);
        if (lstNativeAd.Exists(x => x.IsLoaded() == true))
            return true;
        return false;
    }

    public CustomNativeAd GetNativeAd(string positionAd)
    {
        var lstNativeAd = FindNativeAd(positionAd);
        var lstNativeAdReady = lstNativeAd.Find(x => x.IsLoaded() == true);
        return lstNativeAdReady;
    }

    public void AsynsGetNativeAd(string positionAd, Action<CustomNativeAd> nativeAdReady)
    {
        StartCoroutine(CorotineAsynsGetNativeAd(positionAd, nativeAdReady));
    }

    private IEnumerator CorotineAsynsGetNativeAd(string positionAd, Action<CustomNativeAd> nativeAdReady)
    {
        yield return new WaitUntil(() => NativeIsReady(positionAd));
        var nativeAd = GetNativeAd(positionAd);
        nativeAdReady?.Invoke(nativeAd);
        Debug.Log("Native Ad - Ok");
    }
}
