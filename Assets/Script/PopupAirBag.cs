using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAirBag : UICanvas
{
    public Transform _btnClose;

    public override void Show()
    {
        base.Show();
        _btnClose.SetActive(false);
        CoroutineUtils.PlayCoroutine(() => _btnClose.SetActive(true), 1f);
        SoundManager.Instance.PlaySpecial(GameData.GetAudio(GameData.PositionType.Airbag), true, false);
        AdmobManager.Instance.HideBanner();
        AdmobManager.Instance.IsBannerAd = false;
    }

    public override void Hide()
    {
        base.Hide();
        AdmobManager.Instance.IsBannerAd = true;
        AdmobManager.Instance.SetBannerId("BannerPlayId");
        AdmobManager.Instance.ConitinuteShowBanner();
    }
}
