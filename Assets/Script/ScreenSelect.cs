using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenSelect : UICanvas
{
    public GameObject BannerAdBackground;

    public Transform _content;
    public Sprite[] _cars;
    public Image _img_car;
    public bool IsTransfer = false;

    public int _index_car_selected = -1;
    private bool _awake = false;

    public override void Show()
    {
        base.Show();
        //AdmobManager.Instance.ShowBanner("BannerHomeId");
        AdmobManager.Instance.SetBannerId("BannerHomeId");

        if (!_awake)
        {
            _awake = true;
            _content.ForChild((_index, _child) =>
            {
                var _carItem = _child.GetComponent<CarItem>();
                if (_carItem != null)
                    _carItem.Initialized(_index);
                //else
                //    Debug.Log(_index);
            });
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnChangeCar(int _index)
    {
        IsTransfer = true;
        _index_car_selected = _index;
        var _rect = _img_car.rectTransform;
        _rect.DOKill();
        _rect.DOLocalMove(new Vector3(-1170.0f, -185.0f, 0.0f), 0.5f).OnComplete(() =>
        {
            _img_car.sprite = _cars[_index];
            _rect.localPosition = new Vector3(1170.0f, 555.0f, 0.0f);
            _rect.DOLocalMove(new Vector3(0.0f, 185.0f, 0.0f), 0.5f).OnComplete(() =>
            {
                IsTransfer = false;
            });
        });
    }

    public void OnSetCar(int _index)
    {
        _index_car_selected = _index;
        _img_car.sprite = _cars[_index];
    }

    public void OnDrive()
    {
        var _child = _content.GetChild(_index_car_selected);
        var _name = _child.FindChildByParent("Name").GetComponent<Text>().text;
        GameManager.Instance.Show(_name);
        GameData.Instance.SetIndexCar(_index_car_selected);

        Hide();
        ScreenHelper.FindScript<ScreenShow>().Initialized(_index_car_selected, _name);
        ScreenHelper.FindScript<ScreenShow>().Show();

        //-120

        //51.5
        //270
    }

    public void Setting()
    {
        PopupHelper.FindScript<PopupSetting>().Show();
    }
}
