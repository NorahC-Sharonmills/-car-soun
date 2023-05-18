using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarItem : MonoBehaviour
{
    public int index = -1;
    private int GetIndex()
    {
        return index;
    }
    private void SetIndex(int _index)
    {
        index = _index;
    }

    private Transform _unselect;
    private Transform _lock;
    public string _id;

    public bool _openFromFirebase = false;

    public void Initialized(int _index)
    {
        SetIndex(_index);
        _unselect = this.transform.FindChildByParent("Unselect");
        var _btn = _unselect.GetComponent<Button>();
        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(() =>
        {
            GameEvent.OnChangeCarMethod(GetIndex());
        });

        _lock = this.transform.FindChildByParent("btnLock");
        var _btnLock = _lock.GetComponent<Button>();
        _btnLock.onClick.RemoveAllListeners();
        _btnLock.onClick.AddListener(() =>
        {
            OnBuyCar(GetIndex());
        });

        if(RuntimeStorageData.Player.Cars[GetIndex()] == 1)
        {
            _lock.SetActive(false);
            if(RuntimeStorageData.Player.Car == GetIndex())
            {
                _unselect.SetActive(false);
                ScreenHelper.FindScript<ScreenSelect>().OnSetCar(GetIndex());
            }
            else
            {
                _unselect.SetActive(true);
            }
        }
        else
        {
            var _data = FirebaseManager.Instance.carReward;
            _data.SimpleForEach((_car) =>
            {
                if (_car.id == _id)
                {
                    if (_car.open == 1)
                    {
                        _lock.SetActive(true);
                        _openFromFirebase = false;
                    }
                    else
                    {
                        _lock.SetActive(false);
                        _openFromFirebase = true;
                        if (RuntimeStorageData.Player.Car == GetIndex())
                        {
                            Debug.Log("initialized set car");
                            OnChangeCar(GetIndex());
                            ScreenHelper.FindScript<ScreenSelect>().OnSetCar(GetIndex());
                        }
                    }
                }
            });
            _unselect.SetActive(true);
        }
    }

    private void OnEnable()
    {
        GameEvent.OnChangeCar += OnChangeCar;
        GameEvent.OnInitializedCar += OnInitializedCar;
    }

    private void OnDisable()
    {
        GameEvent.OnChangeCar -= OnChangeCar;
        GameEvent.OnInitializedCar -= OnInitializedCar;
    }

    private void OnInitializedCar()
    {
        if (RuntimeStorageData.Player.Cars[GetIndex()] == 0)
        {
            var _data = FirebaseManager.Instance.carReward;
            _data.SimpleForEach((_car) =>
            {
                if(_car.id == _id)
                {
                    if (_car.open == 1)
                    {
                        _lock.SetActive(true);
                        _openFromFirebase = false;
                    }
                    else
                    {
                        _lock.SetActive(false);
                        _openFromFirebase = true;
                        if(RuntimeStorageData.Player.Car == GetIndex())
                        {
                            _unselect.SetActive(false);
                            ScreenHelper.FindScript<ScreenSelect>().OnSetCar(GetIndex());
                        }
                        else
                            _unselect.SetActive(true);
                    }
                }
            });
        }
    }

    private void OnChangeCar(int _index)
    {
        if(GetIndex() == _index)
        {
            _unselect.SetActive(false);
            RuntimeStorageData.Player.Car = _index;
            ScreenHelper.FindScript<ScreenSelect>().OnChangeCar(_index);
        }
        else
        {
            _unselect.SetActive(true);
        }
    }

    private void OnBuyCar(int _index)
    {
        if(!AdmobManager.Instance.IsNetwork())
        {
            var notification = PopupHelper.FindScript<PopupNotification>();
            notification.Initialized("warning", "No internet\nplease connect to open the car");
            return;
        }

        string positionAd = "";
        switch(_index)
        {
            case 2:
                positionAd = "RewardFerrariLaId";
                break;
            case 3:
                positionAd = "RewardLamboAvenId";
                break;
            case 4:
                positionAd = "RewardLykanHyperId";
                break;
            case 5:
                positionAd = "RewardMaclaren12Id";
                break;
            case 6:
                positionAd = "RewardTeslaModelsId";
                break;
            case 7:
                positionAd = "RewardAudiA8Id";
                break;
            case 8:
                positionAd = "RewardLanboSianId";
                break;
            case 9:
                positionAd = "RewardLamboRenenoId";
                break;
            case 10:
                positionAd = "RewardLamboVenenosId";
                break;
            case 11:
                positionAd = "RewardBugatiVeyronId";
                break;
            case 12:
                positionAd = "RewardFerrariEnzoId";
                break;
            case 13:
                positionAd = "RewardLamboCentemario";
                break;
        }

        AdmobManager.Instance.ShowReward(positionAd, () =>
        {
            if (RuntimeStorageData.Player.Cars[_index] == 0)
            {
                RuntimeStorageData.Player.Cars[_index] = 1;
                RuntimeStorageData.Player.Car = _index;
                GameEvent.OnChangeCarMethod(_index);
                _lock.gameObject.SetActive(false);
            }
        }, () =>
        {

        }, () =>
        {
            var notification = PopupHelper.FindScript<PopupNotification>();
            notification.Initialized("warning", "Request time out\nplease connect to use");
        });
    }
}
