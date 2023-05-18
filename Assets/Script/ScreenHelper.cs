using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHelper : MonoBehaviour
{
    private static ScreenHelper _instance;
    private bool _isReady = false;

    private void Awake()
    {
        _isReady = true;
        _instance = this;
    }
    private void Start()
    {
        AdmobManager.Instance.SetBannerId("BannerHomeId");
        bool IsLanguage = PrefManager.GetBool(Manager.IsLanguage);
        bool IsTutorial = PrefManager.GetBool(Manager.IsTutorial);
        if (!IsLanguage)
        {
            FindScriptAndHide<ScreenLanguage>().Show();
        }
        else if(!IsTutorial)
        {
            FindScriptAndHide<ScreenTutorial>().Show();
        }
        else
        {
            AdmobManager.Instance.ConitinuteShowBanner();
            FindScriptAndHide<ScreenSelect>().Show();
        }
    }

    private static Type _typeCache;
    public static void AppPause()
    {

    }

    public static void AppResume()
    {

    }

    public static void NativeBack()
    {
        Debug.Log("type " + _typeCache);
        if(_typeCache == typeof(ScreenLanguage))
        {
            RuntimeStorageData.SaveAllData();
            Application.Quit();
        }
        else if (_typeCache == typeof(ScreenTutorial))
        {
            RuntimeStorageData.SaveAllData();
            Application.Quit();
        }
        else if(_typeCache == typeof(ScreenSelect))
        {
            RuntimeStorageData.SaveAllData();
            Application.Quit();
        }else if(_typeCache == typeof(ScreenShow))
        {
            FindScriptAndHide<ScreenSelect>().Show();
        }else if(_typeCache == typeof(ScreenGame))
        {
            var popupType = PopupHelper.GetPopupShow();
            if (popupType == null)
                FindScriptAndHide<ScreenSelect>().Show();
            else if(popupType == typeof(PopupAirBag))
            {
                PopupHelper.FindScript<PopupAirBag>().Hide();
            }
            else if(popupType == typeof(PopupHorn))
            {
                PopupHelper.FindScript<PopupHorn>().Hide();
            }
            else if (popupType == typeof(PopupInfo))
            {
                PopupHelper.FindScript<PopupInfo>().Hide();
            }
            else if (popupType == typeof(PopupNotification))
            {
                PopupHelper.FindScript<PopupNotification>().Hide();
            }
            else if (popupType == typeof(PopupRefill))
            {
                PopupHelper.FindScript<PopupRefill>().Hide();
            }
            else if (popupType == typeof(PopupSetting))
            {
                PopupHelper.FindScript<PopupSetting>().Hide();
            }
        }
    }

    public static T FindScript<T>()
    {
        for (int i = 0; i < _instance.transform.childCount; i++)
        {
            var _child = _instance.transform.GetChild(i);
            var _childScript = _child.GetComponent<T>();
            if (_childScript != null)
            {
                _typeCache = _childScript.GetType();
                return _childScript;
            }
        }

        return default(T);
    }

    public static T FindScriptAndHide<T>()
    {
        T response = default(T);
        for (int i = 0; i < _instance.transform.childCount; i++)
        {
            var _child = _instance.transform.GetChild(i);
            var _childScript = _child.GetComponent<T>();
            if (_childScript != null)
            {
                _typeCache = _childScript.GetType();
                response = _childScript;
            }
            else
            {
                _child.gameObject.SetActive(false);
            }
        }

        return response;
    }
}
