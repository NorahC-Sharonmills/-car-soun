using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLanguage : UICanvas
{
    public Sprite languageSelect;
    public Sprite languageUnSelect;
    public Transform lstLanguage;
    public GameObject transformNativeAd;
    public AdmobNativeAd admobNativeAd;

    public override void Show()
    {
        base.Show();
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                OnClickLanguage(0);
                break;
            case SystemLanguage.French:
                OnClickLanguage(1);
                break;
            case SystemLanguage.Spanish:
                OnClickLanguage(2);
                break;
            case SystemLanguage.Portuguese:
                OnClickLanguage(3);
                break;
            case SystemLanguage.Arabic:
                OnClickLanguage(6);
                break;
            default:
                OnClickLanguage(0);
                break;
        }

        Debug.Log("------------------------------------------- Hide All Banner From ScreenLanguage 47");
        AdmobManager.Instance.HideBanner();
        AdmobManager.Instance.IsLanguage = true;
    }

    public void OkLanguage()
    {
        AdmobManager.Instance.IsLanguage = false;
        PrefManager.SetBool(Manager.IsLanguage, true);
        GameEvent.OnChangeLanguageMethod();
        bool IsTutorial = PrefManager.GetBool(Manager.IsTutorial);
        if(!IsTutorial)
        {
            AdmobManager.Instance.IsTutorial = true;
            ScreenHelper.FindScriptAndHide<ScreenTutorial>().Show();
        }
        else
        {
            Manager.Instance.CompleteOpenAd(0.01f);
            ScreenHelper.FindScriptAndHide<ScreenSelect>().Show();
        }
    }

    public void OnClickLanguage(int _index)
    {
        OnChangeLanguage(_index);
        OnChangeColorButton(_index);
        GameEvent.OnChangeLanguageMethod();
    }

    private void OnChangeColorButton(int __index)
    {
        lstLanguage.ForChild((_index, _child) =>
        {
            var _image = _child.GetComponent<Image>();
            if (_index == __index)
            {
                _image.sprite = languageSelect;
            }
            else
            {
                _image.sprite = languageUnSelect;
            }
        });
    }

    private void OnChangeLanguage(int __index)
    {
        switch(__index)
        {
            case 0:
                RuntimeStorageData.Player.Language = "English";
                break;
            case 1:
                RuntimeStorageData.Player.Language = "French";
                break;
            case 2:
                RuntimeStorageData.Player.Language = "Spanish";
                break;
            case 3:
                RuntimeStorageData.Player.Language = "Portuguese";
                break;
            case 4:
                RuntimeStorageData.Player.Language = "Hindi";
                break;
            case 5:
                RuntimeStorageData.Player.Language = "Urdu";
                break;
            case 6:
                RuntimeStorageData.Player.Language = "Arabic";
                break;
        }
    }
}
