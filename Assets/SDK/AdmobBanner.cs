using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomBannerAd
{
    public int MatchLoadAd = 0;
    public int MatchRequest = 0;
    public int MatchImpression = 0;

    public string positionAd;
    public string idAd = "";
    public BannerView bannerView;

    public AdmobManager.StatusAd statusAd = AdmobManager.StatusAd.Mising;

    public CustomBannerAd(string adUnitId, string adPosition)
    {
        idAd = adUnitId;
        positionAd = adPosition;

#if UNITY_EDITOR
        AdSize adSize = AdSize.Banner;
#else
        AdSize adSize = new AdSize(AdSize.FullWidth, 50);
#endif
        bannerView = new BannerView(adUnitId, adSize, AdPosition.Bottom);
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        bannerView.OnAdClosed += HandleOnAdClosed;
        bannerView.OnAdOpening += BannerView_OnAdOpening;

        LoadAd();
    }

    private void BannerView_OnAdOpening(object sender, System.EventArgs e)
    {

    }

    private void HandleOnAdClosed(object sender, System.EventArgs e)
    {
        statusAd = AdmobManager.StatusAd.Mising;
    }

    private void HandleOnAdLoaded(object sender, System.EventArgs e)
    {
        statusAd = AdmobManager.StatusAd.Ready;
        MatchRequest += 1;
        MatchImpression += 1;
    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs error)
    {
        statusAd = AdmobManager.StatusAd.Mising;
        // Increase match request count if error is "No Fill"
        MatchRequest += 1;
        LoadAd();
    }

    public void Show()
    {
        if (bannerView != null)
            bannerView.Show();
    }

    public void Hide()
    {
        if (bannerView != null)
            bannerView.Hide();
    }

    public bool IsLoaded()
    {
        return statusAd == AdmobManager.StatusAd.Ready ? true : false;
    }

    public void LoadAd()
    {
        if (bannerView != null && statusAd == AdmobManager.StatusAd.Mising)
        {
            MatchLoadAd += 1;
            statusAd = AdmobManager.StatusAd.Loading;
            AdRequest request = new AdRequest.Builder().Build();
            this.bannerView.LoadAd(request);
        }
    }
}

public partial class AdmobManager : MonoSingletonGlobal<AdmobManager>
{
    [Header("--- Banner ---")]
    public PositionAd[] positionBannerAds;
    public List<CustomBannerAd> customBannerAds;
    public bool IsBannerAd = false;

    int BannerTotalMatchedRequest = 0;
    int BannerTotalImpresstion = 0;
    int BannerTotalLoadAd = 0;

    public void InitializedBanner()
    {
        customBannerAds = new List<CustomBannerAd>();
    }

    public void OnUpdateBannerAd()
    {
        if (!IsNetwork())
            return;

        BannerTotalMatchedRequest = 0;
        BannerTotalImpresstion = 0;
        BannerTotalLoadAd = 0;

        if (customBannerAds != null)
        {
            for (int i = 0; i < customBannerAds.Count; i++)
            {
                BannerTotalMatchedRequest += customBannerAds[i].MatchRequest;
                BannerTotalImpresstion += customBannerAds[i].MatchImpression;
                BannerTotalLoadAd += customBannerAds[i].MatchLoadAd;
            }
        }
    }

    public void HideBanner()
    {
        if (_corotineShowBannerAfter != null)
            StopCoroutine(_corotineShowBannerAfter);
        if (customBannerAds != null)
        {
            for (int i = 0; i < customBannerAds.Count; i++)
            {
                customBannerAds[i].Hide();
            }
        }
    }

    private string _continuteShowBannerAdId = "";
    public void ConitinuteShowBanner()
    {
        if (!string.IsNullOrEmpty(_continuteShowBannerAdId))
        {
            ShowBanner(_continuteShowBannerAdId);
            Debug.Log("Conitinute Show Banner Id " + _continuteShowBannerAdId);
        }
    }

    Coroutine _corotineShowBannerAfter;
    public void ContinuteShowBannerAfter(float _after)
    {
        if (_corotineShowBannerAfter != null)
            StopCoroutine(_corotineShowBannerAfter);

        _corotineShowBannerAfter = CoroutineUtils.PlayCoroutine(() =>
        {
            ConitinuteShowBanner();
        }, _after);
    }

    public void SetBannerId(string positionAd)
    {
        _continuteShowBannerAdId = positionAd;
    }

    public void ShowBanner(string positionAd)
    {
        if (!IsBannerAd)
            return;

        if (IsLanguage)
            return;

        if (IsTutorial)
            return;

        Debug.Log("Show Banner Id " + _continuteShowBannerAdId);
        var banner = BannerGet(positionAd);
        banner.Show();
    }

    private CustomBannerAd BannerGet(string positionAd)
    {
        if (customBannerAds.Exists(x => x.positionAd == positionAd))
        {
            //ad ready in list
            var customBannerAd = customBannerAds.Find(x => x.positionAd == positionAd);
            if (!customBannerAd.IsLoaded())
            {
                customBannerAd.LoadAd();
            }
            return customBannerAd;
        }
        else
        {
            //ad missing in list
            var bannerID = BannerGetId(positionAd);
            var customBannerAd = new CustomBannerAd(bannerID.Id, bannerID.Position);
            //customBannerAd.positionAd = bannerID.Position;
            //customBannerAd.idAd = bannerID.Id;
            customBannerAds.Add(customBannerAd);
            return customBannerAd;
        }
    }

    public PositionAd BannerGetId(string positionAd)
    {
        for(int i = 0; i < positionBannerAds.Length; i++)
        {
            if (positionBannerAds[i].Position == positionAd)
                return positionBannerAds[i];
        }

        return null;
    }
}
