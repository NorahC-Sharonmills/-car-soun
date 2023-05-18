using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

[System.Serializable]
public class CustomRewardedAd
{
    public int MatchLoadAd = 0;
    public int MatchRequest = 0;
    public int MatchImpression = 0;

    public string positionAd;
    public string idAd = "";
    public RewardedAd rewardedAd;

    public Action RewardedAd_AsynsOnUserEarnedReward;
    public Action RewardAd_AsynsOnAdClosed;
    public Action RewardAd_AsynsOnAdFailedToShow;
    public Action RewardAd_AsynsOnAdOpening;

    public AdmobManager.StatusAd statusAd = AdmobManager.StatusAd.Mising;

    public CustomRewardedAd(string adUnitId, string adPosition)
    {
        idAd = adUnitId;
        positionAd = adPosition;

        rewardedAd = new RewardedAd(adUnitId);
        rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;
        rewardedAd.OnAdFailedToLoad += RewardedAd_OnAdFailedToLoad;
        rewardedAd.OnAdFailedToShow += RewardedAd_OnAdFailedToShow;
        rewardedAd.OnAdClosed += RewardedAd_OnAdClosed;
        rewardedAd.OnAdOpening += RewardedAd_OnAdOpening;

        rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;

        LoadAd();
    }

    private bool _RewardedAd_OnUserEarnedReward = false;
    private void RewardedAd_OnUserEarnedReward(object sender, Reward e)
    {
        _RewardedAd_OnUserEarnedReward = true;
        Debug.Log("Reward RewardedAd_OnUserEarnedReward.");
    }

    private void RewardedAd_OnAdOpening(object sender, EventArgs e)
    {
        Debug.Log("Reward RewardedAd_OnAdOpening.");
        RewardAd_AsynsOnAdOpening?.Invoke();
        MatchImpression += 1;
    }

    private bool _RewardedAd_OnAdClosed = false;
    private void RewardedAd_OnAdClosed(object sender, EventArgs e)
    {
        Debug.Log("Reward RewardedAd_OnAdClosed");
        _RewardedAd_OnAdClosed = true;
        if (!rewardedAd.IsLoaded())
            statusAd = AdmobManager.StatusAd.Mising;
    }

    private bool _RewardedAd_OnAdFailedToShow = false;
    private void RewardedAd_OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        _RewardedAd_OnAdFailedToShow = true;
        statusAd = AdmobManager.StatusAd.Mising;
    }

    private void RewardedAd_OnAdLoaded(object sender, EventArgs e)
    {
        statusAd = AdmobManager.StatusAd.Ready;
        MatchRequest += 1;
    }

    private void RewardedAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        statusAd = AdmobManager.StatusAd.Mising;
        MatchRequest += 1;
        LoadAd();
    }

    public void LoadAd()
    {
        if (rewardedAd != null && statusAd == AdmobManager.StatusAd.Mising)
        {
            MatchLoadAd += 1;
            statusAd = AdmobManager.StatusAd.Loading;
            AdRequest request = new AdRequest.Builder().Build();
            rewardedAd.LoadAd(request);
        }
    }

    public void Show()
    {
        if (IsLoaded())
        {
            if (rewardedAd != null)
                rewardedAd.Show();
        }
        else
            statusAd = AdmobManager.StatusAd.Mising;
    }

    public bool IsLoaded()
    {
        if (rewardedAd != null)
            return rewardedAd.IsLoaded();
        return false;
    }

    public void Destroy()
    {
        rewardedAd.Destroy();
    }

    public void Update()
    {
        if(_RewardedAd_OnAdClosed != false)
        {
            _RewardedAd_OnAdClosed = false;
            RewardAd_AsynsOnAdClosed?.Invoke();
        }

        if(_RewardedAd_OnAdFailedToShow != false)
        {
            _RewardedAd_OnAdFailedToShow = false;
            RewardAd_AsynsOnAdFailedToShow?.Invoke();
        }

        if(_RewardedAd_OnUserEarnedReward != false)
        {
            _RewardedAd_OnUserEarnedReward = false;
            RewardedAd_AsynsOnUserEarnedReward?.Invoke();
        }
    }
}

public partial class AdmobManager : MonoSingletonGlobal<AdmobManager>
{
    [Header("--- Reward ---")]
    public PositionAd[] positionAds;
    public List<CustomRewardedAd> customRewardedAds;

    int RewardTotalMatchLoadAd = 0;
    int RewardTotalMatchedRequest = 0;
    int RewardTotalImpresstion = 0;

    public void InitializedReward()
    {
        customRewardedAds = new List<CustomRewardedAd>();
    }

    private void OnUpdateRewardAd()
    {
        if (!IsNetwork())
            return;

        CheckRequestTimeOutRewardAd();

        RewardTotalMatchedRequest = 0;
        RewardTotalImpresstion = 0;
        RewardTotalMatchLoadAd = 0;

        if (customRewardedAds != null)
        {
            for(int i = 0; i < customRewardedAds.Count; i++)
            {
                RewardTotalMatchedRequest += customRewardedAds[i].MatchRequest;
                RewardTotalImpresstion += customRewardedAds[i].MatchImpression;
                RewardTotalMatchLoadAd += customRewardedAds[i].MatchLoadAd;
                customRewardedAds[i].Update();
            }
        }

        //Debug.Log($"reward match request {RewardTotalMatchedRequest} impression {RewardTotalImpresstion}");
    }

    public void ShowReward(string positionAd, Action Reward, Action Close, Action Error)
    {
        if(!IsNetwork())
        {
            Error?.Invoke();
            return;
        }

        if(DisableAd)
        {
            Reward?.Invoke();
            Close?.Invoke();
            return;
        }

        IsOpenAd = false;
        _ShowRewardAsync = StartCoroutine(AsynsShowReward(positionAd, Reward, Close, Error));
    }

    private Coroutine _ShowRewardAsync;
    private Action ActionRewardLoadedOverTime;
    private bool IsRewardAwaitAd = false;
    private float TimerRewardAwaitAd = 0;
    //private float MaxRewardTimerWait = 30;
    private void CheckRequestTimeOutRewardAd()
    {
        if(IsRewardAwaitAd)
        {
            TimerRewardAwaitAd += Time.deltaTime;
            if(TimerRewardAwaitAd > maxTimeRequestAd)
            {
                IsRewardAwaitAd = false;
                if (_ShowRewardAsync != null)
                    StopCoroutine(_ShowRewardAsync);

                IsOpenAd = true;
                HideLoadingAds();
                ConitinuteShowBanner();
                ActionRewardLoadedOverTime?.Invoke();
            }
        }
    }
        

    private IEnumerator AsynsShowReward(string positionAd, Action Reward, Action Close, Action Error)
    {
        IsOpenAdShow = true;
        var customRewardedAd = RewardGet(positionAd);
        ActionRewardLoadedOverTime = () =>
        {
            IsOpenAdShow = false;
            Error?.Invoke();
        };
        IsRewardAwaitAd = true;
        TimerRewardAwaitAd = 0;
        if (customRewardedAd != null && customRewardedAd.IsLoaded())
        {
            RewardInitialized(customRewardedAd, Reward, Close, Error);
            RewardShow(customRewardedAd);
        }
        else
        {
            customRewardedAd.LoadAd();
            RewardInitialized(customRewardedAd, Reward, Close, Error);
            yield return new WaitUntil(() => customRewardedAd.IsLoaded());
            RewardShow(customRewardedAd);
        }
    }

    private void RewardInitialized(CustomRewardedAd customRewardedAd, Action Reward, Action Close, Action Error)
    {
        ShowLoadingAds();
        HideBanner();
        customRewardedAd.RewardedAd_AsynsOnUserEarnedReward = () =>
        {
            Reward?.Invoke();
        };
        customRewardedAd.RewardAd_AsynsOnAdClosed = () =>
        {
            IsRewardAwaitAd = false;
            IsOpenAdShow = false;
            IsOpenAd = true;
            ConitinuteShowBanner();
            Close?.Invoke();
        };
        customRewardedAd.RewardAd_AsynsOnAdFailedToShow = () =>
        {
            IsRewardAwaitAd = false;
            IsOpenAdShow = false;
            IsOpenAd = true;
            ConitinuteShowBanner();
            Error?.Invoke();
        };
        customRewardedAd.RewardAd_AsynsOnAdOpening = () =>
        {
            IsOpenAdShow = false;
            IsRewardAwaitAd = false;
        };
    }

    private void RewardShow(CustomRewardedAd customRewardedAd)
    {
        adRewardAwait = CoroutineUtils.PlayCoroutine(() =>
        {
            HideLoadingAds();
            customRewardedAd.Show();
        }, 2f);
    }

    private CustomRewardedAd RewardGet(string positionAd)
    {
        if (customRewardedAds.Exists(x => x.positionAd == positionAd))
        {
            var customRewardedAd = customRewardedAds.Find(x => x.positionAd == positionAd);
            return customRewardedAd;
        }
        else
        {
            var rewardId = RewardGetId(positionAd);
            var customRewardedAd = new CustomRewardedAd(rewardId.Id, rewardId.Position);
            customRewardedAds.Add(customRewardedAd);
            return customRewardedAd;
        }
    }

    private PositionAd RewardGetId(string positionAd)
    {
        for(int i = 0; i < positionAd.Length; i++)
        {
            if (positionAds[i].Position == positionAd)
                return positionAds[i];
        }
        return null;
    }
}
