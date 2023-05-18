using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHorn : UICanvas
{
    public Color[] colors;
    public Image color;
    private int index = 0;

    public override void Show()
    {
        base.Show();
        color.color = colors[index];
        SoundManager.Instance.PlaySpecial(GameData.GetAudio(GameData.PositionType.Warning), true, true);
        AdmobManager.Instance.HideBanner();
        AdmobManager.Instance.IsBannerAd = false;
    }

    public override void Hide()
    {
        base.Hide();
        AdmobManager.Instance.IsBannerAd = true;
        SoundManager.Instance.PlaySpecial(GameData.GetAudio(GameData.PositionType.Warning), false, true);
        AdmobManager.Instance.SetBannerId("BannerPlayId");
        AdmobManager.Instance.ConitinuteShowBanner();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 0.15f)
        {
            time = 0;
            index += 1;
            if (index >= colors.Length)
                index = 0;
            color.color = colors[index];
        }
    }
}
