using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageLocalizationData : MonoSingletonGlobal<LanguageLocalizationData>
{
    public string[] Languages = new string[] { "English", "French" , "Spanish" , "Portuguese" , "Hindi" , "Urdu" , "Arabic" };

    public class LanguageData
    {
        public string English;
        public string French;
        public string Spanish;
        public string Portuguese;
        public string Hindi;
        public string Urdu;
        public string Arabic;

        public string GetText()
        {
            string response = English;
            switch (RuntimeStorageData.Player.Language)
            {
                case "English":
                    response = English;
                    break;
                case "French":
                    response = French;
                    break;
                case "Spanish":
                    response = Spanish;
                    break;
                case "Portuguese":
                    response = Portuguese;
                    break;
                case "Hindi":
                    response = Hindi;
                    break;
                case "Urdu":
                    response = Urdu;
                    break;
                case "Arabic":
                    response = Arabic;
                    break;
            }

            return response;
        }
    }

    [System.Serializable]
    public class Language
    {
        public RelisticCarEngineSimulator RelisticCarEngineSimulator;
        public Play Play;
        public Setting Setting;
        public Sound Sound;
        public Vibrate Vibrate;
        public Flash Flash;
        public SwipeToView SwipeToView;
        public Unlock Unlock;
        public YourFelIsOut YourFelIsOut;
        public NoThanks NoThanks;
        public Refill Refill;
        public Languagee Languagee;
        public ChooseYourLanguage ChooseYourLanguage;
        public Ok Ok;
        public ThisActionCanContainAds ThisActionCanContainAds;
        public Skip Skip;
        public Next Next;
        public Home Home;
        public TutorialHomeTab TutorialHomeTab;
        public CarSpecs CarSpecs;
        public TutorialSelectTab TutorialSelectTab;
        public CarInterior CarInterior;
        public TutorialGameTab TutorialGameTab;
    }

    [System.Serializable]
    public class Flash : LanguageData { }
    [System.Serializable]
    public class Play : LanguageData { }
    [System.Serializable]
    public class RelisticCarEngineSimulator : LanguageData { }
    [System.Serializable]
    public class Setting : LanguageData { }
    [System.Serializable]
    public class Sound : LanguageData { }
    [System.Serializable]
    public class SwipeToView : LanguageData { }
    [System.Serializable]
    public class Vibrate : LanguageData { }
    [System.Serializable]
    public class Unlock : LanguageData { }
    [System.Serializable]
    public class YourFelIsOut : LanguageData { }
    [System.Serializable]
    public class NoThanks : LanguageData { }
    [System.Serializable]
    public class Refill : LanguageData { }
    [System.Serializable]
    public class Languagee : LanguageData { }
    [System.Serializable]
    public class ChooseYourLanguage : LanguageData { }
    [System.Serializable]
    public class Ok : LanguageData { }
    [System.Serializable]
    public class ThisActionCanContainAds : LanguageData { }
    [System.Serializable]
    public class Skip : LanguageData { }
    [System.Serializable]
    public class Next : LanguageData { }
    [System.Serializable]
    public class Home : LanguageData { }
    [System.Serializable]
    public class TutorialHomeTab : LanguageData { }
    [System.Serializable]
    public class CarSpecs : LanguageData { }
    [System.Serializable]
    public class TutorialSelectTab : LanguageData { }
    [System.Serializable]
    public class CarInterior : LanguageData { }
    [System.Serializable]
    public class TutorialGameTab : LanguageData { }

    public TextAsset _languageText;
    public Language _language;

    protected override void Awake()
    {
        base.Awake();
        _language = JsonUtility.FromJson<Language>(_languageText.text);
    }

    private void Start()
    {
        GameEvent.OnChangeLanguageMethod();
    }

    public string GetText(string Id)
    {
        string response = "";
        switch (Id)
        {
            case "Flash": response = _language.Flash.GetText(); break;
            case "Play": response = _language.Play.GetText(); break;
            case "RelisticCarEngineSimulator": response = _language.RelisticCarEngineSimulator.GetText(); break;
            case "Setting": response = _language.Setting.GetText(); break;
            case "Sound": response = _language.Sound.GetText(); break;
            case "SwipeToView": response = _language.SwipeToView.GetText(); break;
            case "Vibrate": response = _language.Vibrate.GetText(); break;
            case "Unlock": response = _language.Unlock.GetText(); break;
            case "YourFelIsOut": response = _language.YourFelIsOut.GetText(); break;
            case "NoThanks": response = _language.NoThanks.GetText(); break;
            case "Refill": response = _language.Refill.GetText(); break;
            case "Languagee": response = _language.Languagee.GetText(); break;
            case "ChooseYourLanguage": response = _language.ChooseYourLanguage.GetText(); break;
            case "Ok": response = _language.Ok.GetText(); break;
            case "ThisActionCanContainAds": response = _language.ThisActionCanContainAds.GetText(); break;
            case "Skip": response = _language.Skip.GetText(); break;
            case "Next": response = _language.Next.GetText(); break;
            case "Home": response = _language.Home.GetText(); break;
            case "TutorialHomeTabDescription": response = _language.TutorialHomeTab.GetText(); break;
            case "CarSpecs": response = _language.CarSpecs.GetText(); break;
            case "TutorialSelectTabDesciption": response = _language.TutorialSelectTab.GetText(); break;
            case "CarInterior": response = _language.CarInterior.GetText(); break;
            case "TutorialGameTabDescription": response = _language.TutorialGameTab.GetText(); break;
        }
        return response;
    }
}
