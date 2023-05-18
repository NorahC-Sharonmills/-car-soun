using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public delegate void ChangeCar(int index);
    public static event ChangeCar OnChangeCar;
    public static void OnChangeCarMethod(int index)
    {
        if (OnChangeCar != null)
            OnChangeCar(index);
    }

    public delegate void InitializedCar();
    public static event InitializedCar OnInitializedCar;
    public static void OnInitializedCarMethod()
    {
        if (OnInitializedCar != null)
            OnInitializedCar();
    }

    public delegate void Refill();
    public static event Refill OnRefill;
    public static void OnRefillMethod()
    {
        if (OnRefill != null)
            OnRefill();
    }

    public delegate void ChangeLanguage();
    public static event ChangeLanguage OnChangeLanguage;
    public static void OnChangeLanguageMethod()
    {
        if (OnChangeLanguage != null)
            OnChangeLanguage();
    }

    public delegate void NativeLoaded(GoogleMobileAds.Api.NativeAd nativeAd);
    public static event NativeLoaded OnNativeLoaded;
    public static void OnNativeLoadedMethod(GoogleMobileAds.Api.NativeAd nativeAd)
    {
        if (OnNativeLoaded != null)
            OnNativeLoaded(nativeAd);
    }
}
