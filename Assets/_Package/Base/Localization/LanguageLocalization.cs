using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageLocalization : MonoBehaviour
{
    public string Id;
    public bool IsInitialized = false;
    public bool IsAwake = false;

    private void Start()
    {
        IsAwake = true;
        if (!IsInitialized)
            OnChangeLanguage();
    }

    private void OnEnable()
    {
        GameEvent.OnChangeLanguage += OnChangeLanguage;
        if (!IsInitialized && IsAwake)
            OnChangeLanguage();
    }

    private void OnDisable()
    {
        IsInitialized = false;
        GameEvent.OnChangeLanguage -= OnChangeLanguage;
    }

    private void OnChangeLanguage()
    {
        IsInitialized = true;
        var _text = LanguageLocalizationData.Instance.GetText(Id);
        this.GetComponent<Text>().text = _text;
    }
}
