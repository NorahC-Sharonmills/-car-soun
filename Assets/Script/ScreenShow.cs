using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarInfo
{
    public int Stt;
    public string Car;
    public string Name;
    public string TopSpeed;
    public string ThreadSpeed;
    public string Drive;
    public string Power;
}

public class ScreenShow : UICanvas
{
    public GameObject BannerAdBackground;

    private int _indexCar;
    private string _nameCar;

    public TextAsset textAsset;
    public CarInfo[] carInfos;

    public override void Show()
    {
        base.Show();
        AdmobManager.Instance.SetBannerId("BannerCarId");
        AdmobManager.Instance.ConitinuteShowBanner();
    }

    public void Initialized(int _index, string _name)
    {
        _indexCar = _index;
        _nameCar = _name;
    }

    public void OnPlay()
    {
        AdmobManager.Instance.ShowInter("InterPlayId", () =>
        {
            Hide();
            ScreenHelper.FindScript<ScreenGame>().Initialized(_indexCar, _nameCar);
            ScreenHelper.FindScript<ScreenGame>().Show();
            GameManager.Instance.Hide();
        });
    }

    public void Setting()
    {
        PopupHelper.FindScript<PopupSetting>().Show();
    }

    public void OnBack()
    {
        Hide();
        GameManager.Instance.Hide();
        ScreenHelper.FindScript<ScreenSelect>().Show();
    }

    public void OnInfo()
    {
        carInfos = StaticJsonHelper.getJsonArray<CarInfo>(textAsset.text);
        var _data = carInfos[_indexCar];
        var _popupInfo = PopupHelper.FindScript<PopupInfo>();
        _popupInfo.Initialized(_data);
        _popupInfo.Show();
    }
}
