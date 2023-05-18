using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : UICanvas
{
    public Transform _sound;
    public Transform _vibrate;
    public Transform _flash;

    public Text _languageText;
    public int _languageInt;

    public void BackLanguage()
    {
        var lstLanguage = LanguageLocalizationData.Instance.Languages;
        _languageInt -= 1;
        if (_languageInt < 0)
            _languageInt = lstLanguage.Length - 1;

        RuntimeStorageData.Player.Language = lstLanguage[_languageInt];
        _languageText.text = RuntimeStorageData.Player.Language;
        GameEvent.OnChangeLanguageMethod();
    }

    public void NextLanguage()
    {
        var lstLanguage = LanguageLocalizationData.Instance.Languages;
        _languageInt += 1;
        if (_languageInt >= lstLanguage.Length)
            _languageInt = 0;

        RuntimeStorageData.Player.Language = lstLanguage[_languageInt];
        _languageText.text = RuntimeStorageData.Player.Language;
        GameEvent.OnChangeLanguageMethod();
    }

    public override void Show()
    {
        base.Show();

        _languageText.text = RuntimeStorageData.Player.Language;
        var lstLanguage = LanguageLocalizationData.Instance.Languages;
        lstLanguage.ForEach((_language, _int) =>
        {
            if (_language == RuntimeStorageData.Player.Language)
            {
                _languageInt = _int;
            }
        });

        _sound.ForChild((_index, _child) =>
        {
            var _value = RuntimeStorageData.Sound.isSound == true ? 1 : 0;
            if (_index == _value)
                _child.SetActive(true);
            else
                _child.SetActive(false);
        });

        _vibrate.ForChild((_index, _child) =>
        {
            var _value = RuntimeStorageData.Sound.isVibrate == true ? 1 : 0;
            if (_index == _value)
                _child.SetActive(true);
            else
                _child.SetActive(false);
        });

        _flash.ForChild((_index, _child) =>
        {
            var _value = RuntimeStorageData.Sound.isMusic == true ? 1 : 0;
            if (_index == _value)
                _child.SetActive(true);
            else
                _child.SetActive(false);
        });
    }

    public void Sound()
    {
        RuntimeStorageData.Sound.isSound = !RuntimeStorageData.Sound.isSound;
        _sound.ForChild((_index, _child) =>
        {
            var _value = RuntimeStorageData.Sound.isSound == true ? 1 : 0;
            if (_index == _value)
                _child.SetActive(true);
            else
                _child.SetActive(false);
        });
    }

    public void Vibrate()
    {
        RuntimeStorageData.Sound.isVibrate = !RuntimeStorageData.Sound.isVibrate;
        _vibrate.ForChild((_index, _child) =>
        {
            var _value = RuntimeStorageData.Sound.isVibrate == true ? 1 : 0;
            if (_index == _value)
                _child.SetActive(true);
            else
                _child.SetActive(false);
        });
    }

    public void Flash()
    {
        RuntimeStorageData.Sound.isMusic = !RuntimeStorageData.Sound.isMusic;
        _flash.ForChild((_index, _child) =>
        {
            var _value = RuntimeStorageData.Sound.isMusic == true ? 1 : 0;
            if (_index == _value)
                _child.SetActive(true);
            else
                _child.SetActive(false);
        });
    }    
}
