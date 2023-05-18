using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomInterAd
{
    public int MatchLoadAd = 0;
    public int MatchRequest = 0;
    public int MatchImpression = 0;

    public string positionAd;
    public string idAd = "";
    public InterstitialAd interstitial;

    public Action InterAd_AsynsOnAdOpened;
    public Action InterAd_AsynsOnAdClosed;

    public void SetAction(Action AsynsOnAdOpened, Action AsynsOnAdClosed)
    {
        InterAd_AsynsOnAdOpened = () => AsynsOnAdOpened?.Invoke();
        InterAd_AsynsOnAdClosed = () => AsynsOnAdClosed?.Invoke();
    }

    public AdmobManager.StatusAd statusAd = AdmobManager.StatusAd.Mising;

    public CustomInterAd(string adUnitId, string adPosition)
    {
        idAd = adUnitId;
        positionAd = adPosition;

        interstitial = new InterstitialAd(adUnitId);
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        interstitial.OnAdOpening += HandleOnAdOpening;
        interstitial.OnAdClosed += HandleOnAdClosed;
        interstitial.OnAdFailedToShow += HandleOnAdFailedToShow;

        LoadAd();
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        statusAd = AdmobManager.StatusAd.Ready;
        MatchRequest += 1;
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        statusAd = AdmobManager.StatusAd.Mising;
        MatchRequest += 1;
        LoadAd();
    }

    private bool _HandleOnAdOpening = false, __HandleOnAdOpening = false;
    public void HandleOnAdOpening(object sender, EventArgs args)
    {
        _HandleOnAdOpening = true;
        MatchImpression += 1;
    }

    private bool _HandleOnAdClosed = false, __HandleOnAdClosed = false;
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        _HandleOnAdClosed = true;
        if (!interstitial.IsLoaded())
            statusAd = AdmobManager.StatusAd.Mising;

    }

    private bool _HandleOnAdFailedToShow = false, __HandleOnAdFailedToShow = false;
    public void HandleOnAdFailedToShow(object sender, EventArgs args)
    {
        _HandleOnAdFailedToShow = true;
        statusAd = AdmobManager.StatusAd.Mising;
    }

    public void LoadAd()
    {
        if(interstitial != null && statusAd == AdmobManager.StatusAd.Mising)
        {
            MatchLoadAd += 1;
            statusAd = AdmobManager.StatusAd.Loading;
            AdRequest request = new AdRequest.Builder().Build();
            interstitial.LoadAd(request);
        }
    }

    public void Show()
    {
        if (interstitial != null)
            interstitial.Show();
    }

    public bool IsLoaded()
    {
        if (interstitial != null)
            return interstitial.IsLoaded();
        return false;
    }

    public void Destroy()
    {
        if (interstitial != null)
            interstitial.Destroy();
    }

    public void Update()
    {
        if (__HandleOnAdOpening != _HandleOnAdOpening)
        {
            _HandleOnAdOpening = false;
            InterAd_AsynsOnAdOpened?.Invoke();
        }

        if (__HandleOnAdClosed != _HandleOnAdClosed)
        {
            _HandleOnAdClosed = false;
            InterAd_AsynsOnAdClosed?.Invoke();
        }

        if (__HandleOnAdFailedToShow != _HandleOnAdFailedToShow)
        {
            _HandleOnAdFailedToShow = false;
            InterAd_AsynsOnAdClosed?.Invoke();
        }
    }
}

public partial class AdmobManager : MonoSingletonGlobal<AdmobManager>
{
    [Header("--- Inter ---")]
    public PositionAd[] positionInterAds;
    public List<CustomInterAd> customInterAds;
    public float MaxInterTimerWait = 30;

    int InterTotalMatchLoadAd = 0;
    int InterTotalMatchedRequest = 0;
    int InterTotalImpresstion = 0;

    public float TimeDelayInter = 30;

    private void CheckingDelayInter()
    {
        if (TimeDelayInter <= 35)
            TimeDelayInter += Time.deltaTime;
    }

    public void InitializedInter()
    {
        customInterAds = new List<CustomInterAd>();
        if (IsNetwork() == false)
            MaxInterTimerWait = minTimeRequestAd;

        InterIsReady("InterSplashId");
    }

    public void ShowInter(string positionAd, Action ActionBeforeCorotine, Action ActionOnAdClose = null)
    {
        if (!IsNetwork() || DisableAd || TimeDelayInter <= MaxInterTimerWait)
        {
            ActionBeforeCorotine?.Invoke();
            if (ActionOnAdClose != null)
                ActionOnAdClose?.Invoke();
            return;
        }

        TimeDelayInter = 0;

        HideBanner();
        IsOpenAdShow = true;
        IsOpenAd = false;
        switch (positionAd)
        {
            case "InterPlayId":
                SetShowInter(FirebaseManager.Instance.IsInterPlay, positionAd, ActionBeforeCorotine, ActionOnAdClose);
                break;
            case "InterBackId":
                SetShowInter(FirebaseManager.Instance.IsInterBack, positionAd, ActionBeforeCorotine, ActionOnAdClose);
                break;
            case "InterTutorialId":
                SetShowInter(FirebaseManager.Instance.IsInterTutorial, positionAd, ActionBeforeCorotine, ActionOnAdClose);
                break;
        }
    }

    private void SetShowInter(bool IsShowFromRemoteConfig, string positionAd, Action ActionBeforeCorotine, Action ActionOnAdClose)
    {
        if (IsShowFromRemoteConfig)
        {
            ShowLoadingAds();
            AwaitShowInter(positionAd, () =>
            {
                ActionOnAdClose?.Invoke();
                IsOpenAd = true;
                IsOpenAdShow = false;
                ConitinuteShowBanner();
            }, ActionBeforeCorotine);

            if (IsNetwork() == true)
                MaxInterTimerWait = maxTimeRequestAd;
        }
        else
        {
            IsOpenAd = true;
            IsOpenAdShow = false;
            ConitinuteShowBanner();
            ActionBeforeCorotine?.Invoke();
        }
    }

    private void OnUpdateInterAd()
    {
        CheckRequestTimeOutInterAd();
        CheckingDelayInter();

        if (!IsNetwork())
            return;

        InterTotalMatchedRequest = 0;
        InterTotalImpresstion = 0;
        InterTotalMatchLoadAd = 0;

        if (customInterAds != null)
        {
            for(int i = 0; i < customInterAds.Count; i++)
            {
                InterTotalMatchedRequest += customInterAds[i].MatchRequest;
                InterTotalImpresstion += customInterAds[i].MatchImpression;
                InterTotalMatchLoadAd += customInterAds[i].MatchLoadAd;
                customInterAds[i].Update();
            }
        }

        //Debug.Log($"inter match request {InterTotalMatchedRequest} impression {InterTotalImpresstion}");
    }

    public bool InterIsReady(string positionAd)
    {
        var inter = InterGet(positionAd);
        var isLoaded = inter.Exists(x => x.IsLoaded() == true);
        return isLoaded;
    }

    public CustomInterAd InterReady(string positionAd)
    {
        var inter = InterGet(positionAd);
        var interReady = inter.Find(x => x.IsLoaded() == true);
        return interReady;
    }

    private List<CustomInterAd> InterGet(string positionAd)
    {
        if(customInterAds.Exists(x => x.positionAd == positionAd))
        {
            var response = customInterAds.FindAll(x => x.positionAd == positionAd);
            return response;
        }
        else
        {
            Debug.Log("Initialized Inter Ad");
            var interId = InterGetId(positionAd);
            if(interId.IsPreventive)
            {
                var customInterAd1 = new CustomInterAd(interId.Id, interId.Position);
                customInterAds.Add(customInterAd1);
                var customInterAd2 = new CustomInterAd(interId.Id, interId.Position);
                customInterAds.Add(customInterAd2);

                var response = customInterAds.FindAll(x => x.positionAd == positionAd);
                return response;
            }
            else
            {
                var customInterAd = new CustomInterAd(interId.Id, interId.Position);
                customInterAds.Add(customInterAd);

                var response = customInterAds.FindAll(x => x.positionAd == positionAd);
                return response;
            }
        }
    }

    private PositionAd InterGetId(string positionAd)
    {
        for(int i = 0; i < positionInterAds.Length; i++)
        {
            if (positionInterAds[i].Position == positionAd)
                return positionInterAds[i];
        }
        return null;
    }

    private Coroutine _ShowInterAsync;
    private Action ActionLoadedOverTime;
    private bool IsInterAwaitAd = false;
    private float TimerInterAwaitAd = 0;
    //public void AwaitShowInter(string positionAd, Action AsynsOnAdClosed, Action actionLoadedOverTime, Action ActionBeforeCorotine)
    public void AwaitShowInter(string positionAd, Action AsynsOnAdClosed, Action ActionBeforeCorotine, float timeBeforeShow = 1f)
    {
        _ShowInterAsync = StartCoroutine(ShowInterAsync(positionAd, AsynsOnAdClosed, ActionBeforeCorotine, timeBeforeShow));
        //ActionLoadedOverTime = actionLoadedOverTime;
        ActionLoadedOverTime = AsynsOnAdClosed;
        IsInterAwaitAd = true;
        TimerInterAwaitAd = 0;
    }

    int CacheTimerInterAwaitAd = 0;
    private void CheckRequestTimeOutInterAd()
    {
        if (IsInterAwaitAd)
        {
            TimerInterAwaitAd += Time.deltaTime;
#if !UNITY_EDITOR
            if(TimerInterAwaitAd > CacheTimerInterAwaitAd)
            {
                CacheTimerInterAwaitAd += 1;
                Debug.Log("TIME AWAIT AD INTER " + TimerInterAwaitAd);
            }
#endif
            if (TimerInterAwaitAd > MaxInterTimerWait)
            {
                if (_ShowInterAsync != null)
                    StopCoroutine(_ShowInterAsync);
                IsInterAwaitAd = false;
                IsOpenAd = true;
                IsOpenAdShow = false;
                ActionLoadedOverTime?.Invoke();
            }
        }
    }

    private IEnumerator ShowInterAsync(string positionAd, Action AsynsOnAdClosed, Action ActionBeforeCorotine, float timeBeforeShow)
    {
        var customInterAds = InterGet(positionAd);
        if (customInterAds != null && InterIsReady(positionAd))
        {
            var customInterAd = InterReady(positionAd);
            customInterAd.SetAction(AsynsOnAdClosed, () => IsOpenAdShow = false);
            InterShow(ActionBeforeCorotine, customInterAd, timeBeforeShow);
        }
        else
        {
            var customInterAd = customInterAds[0];
            customInterAd.LoadAd();
            customInterAd.SetAction(AsynsOnAdClosed, () => IsOpenAdShow = false);
            yield return new WaitUntil(() => customInterAd.IsLoaded());
            InterShow(ActionBeforeCorotine, customInterAd, timeBeforeShow);
        }
    }

    private void InterShow(Action ActionBeforeCorotine, CustomInterAd inter, float timeBeforeShow)
    {
        TimerInterAwaitAd = 0;
        IsOpenAdShow = true;
        ShowLoadingAds();
        ActionBeforeCorotine?.Invoke();
        adInterAwait = CoroutineUtils.PlayCoroutine(() =>
        {
            HideLoadingAds();
            TimerInterAwaitAd = 0;
            IsInterAwaitAd = false;
            inter.Show();
        }, timeBeforeShow);
    }
}
