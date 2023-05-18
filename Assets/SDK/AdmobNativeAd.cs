using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdmobNativeAd : MonoBehaviour
{
    private NativeAd adNative = null;

    [SerializeField] GameObject adNativeLoading;
    [SerializeField] GameObject adNativePanel;
    [SerializeField] RawImage adIcon;
    [SerializeField] RawImage BodyImage;
    [SerializeField] RawImage adChoices;
    [SerializeField] Text adHeadline;
    [SerializeField] Text adCallToAction;
    [SerializeField] Text adAdvertiser;
    public string AdPostion;

    //private void OnEnable()
    //{
    //    GameEvent.OnNativeLoaded += OnNativeLoaded;
    //}

    private void OnNativeLoaded(NativeAd _nativeAd)
    {
        IsShow = false;
        this.adNative = _nativeAd;
    }

    private void Awake()
    {
        IsShow = false;
    }

    private bool IsShow = false;
    private void Update()
    {
        if (this.adNative == null && AdmobManager.Instance.NativeIsReady(AdPostion))
            this.adNative = AdmobManager.Instance.GetNativeAd(AdPostion).adNative;

        if (this.adNative != null && !IsShow)
        {
            IsShow = true;
            DisplayAd(this.adNative);
        }
    }

    void DisplayAd(NativeAd ad)
    {
        Debug.Log("native DisplayAd 1");
        Texture2D iconTexture = this.adNative.GetIconTexture();
        Texture2D iconAdChoices = this.adNative.GetAdChoicesLogoTexture();
        Debug.Log("native DisplayAd 2");
        if (this.adNative.GetImageTextures().Count > 0)
        {
            List<Texture2D> goList = this.adNative.GetImageTextures();
            BodyImage.texture = goList[0];
            List<GameObject> list = new List<GameObject>();
            list.Add(BodyImage.gameObject);
            this.adNative.RegisterImageGameObjects(list);
        }

        Debug.Log("native DisplayAd 3");
        string body = this.adNative.GetBodyText();
        string headline = this.adNative.GetHeadlineText();
        string cta = this.adNative.GetCallToActionText();
        Debug.Log(this.adNative.GetAdvertiserText());
        Debug.Log(this.adNative.GetResponseInfo().ToString());

        adIcon.texture = iconTexture;
        adChoices.texture = iconAdChoices;
        adHeadline.text = headline;
        adAdvertiser.text = body;
        adCallToAction.text = cta;

        if (this.adNative.RegisterIconImageGameObject(adIcon.gameObject))
        {
            Debug.Log("--------------------------------- RegisterIconImageGameObject");
        };
        if (this.adNative.RegisterAdChoicesLogoGameObject(adChoices.gameObject))
        {
            Debug.Log("--------------------------------- RegisterAdChoicesLogoGameObject");
        };
        if (this.adNative.RegisterHeadlineTextGameObject(adHeadline.gameObject))
        {
            Debug.Log("--------------------------------- RegisterHeadlineTextGameObject");
        };
        if (this.adNative.RegisterCallToActionGameObject(adCallToAction.gameObject))
        {
            Debug.Log("--------------------------------- RegisterCallToActionGameObject");
        };
        if (this.adNative.RegisterAdvertiserTextGameObject(adAdvertiser.gameObject))
        {
            Debug.Log("--------------------------------- RegisterAdvertiserTextGameObject");
        };

        adNativePanel.SetActive(true);
    }

    public void Click()
    {

    }
}
