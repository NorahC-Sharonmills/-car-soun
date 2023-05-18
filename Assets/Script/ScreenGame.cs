using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenGame : UICanvas
{
    public GameObject BannerAdBackground;

    [System.Serializable]
    public struct AnimationJob
    {
        public Transform tranform;
        public Transform target;
    }

    [System.Serializable]
    public struct LstAnimationJob
    {
        public int turn;
        public List<AnimationJob> animationJobs;
    }

    public List<LstAnimationJob> lstAnimationJobs;
    private float animationSpeed = 3;

    [System.Serializable]
    public struct ActionJob
    {
        public Transform job;
        public Transform action;
    }

    public ActionJob[] actionJobs;

    private bool isKey = false;
    private bool _isKey = false;
    private bool isStart = false;
    private bool _isStart = false;
    private bool isWaring = false;
    private bool _isWaring = false;
    private bool isLight = false;
    private bool _isLight = false;

    public Transform _signal_Left;
    public Transform _signal_Right;

    private bool isComplete = false;

    public ButtonEvent _gasEvent;
    public ButtonEvent _brakeEvent;

    private float _gasMax = 10.0f;
    private float _gasLimit = 10.0f;
    private float _gasSpendDefault = 0.25f;
    private float _gasSpendPedan = 0.5f;

    public Image _imgGas;

    public Transform _handKey;

    public void OpenToturialKey()
    {
        _handKey.SetActive(true);
        CoroutineUtils.PlayCoroutine(() =>
        {
            _handKey.SetActive(false);
        }, 2.5f);
    }

    public Transform _handEngine;
    public void OpenToturialEngine()
    {
        _handEngine.SetActive(true);
        CoroutineUtils.PlayCoroutine(() =>
        {
            _handEngine.SetActive(false);
        }, 2.5f);
    }

    public Transform _engine_rpm_wise;
    public Transform _engine_speed_wise;

    private string KeyToturial = "_key_toturial";
    private string KeyStart = "_key_start";

    public Sprite[] _steering_wheels;
    public Image _img_steering;
    public Text _name_car;

    public void Initialized(int _index, string _name)
    {
        Debug.Log(_index);
        _img_steering.sprite = _steering_wheels[_index];
        _name_car.text = _name;
    }    

    public override void Show()
    {
        base.Show();
        StartAnimation();

        _gasLimit = _gasMax;
        _imgGas.fillAmount = _gasLimit / _gasMax;
        GameEvent.OnRefill += GameEvent_OnRefill;

        if (PrefManager.GetBool(KeyToturial, false) == false)
        {
            OpenToturialKey();
            PrefManager.SetBool(KeyToturial, true);
        }

        _gasEvent.PointerDown = () =>
        {
            if(!_isKey)
            {
                OpenToturialKey();
            }
            else if(!_isStart)
            {
                OpenToturialEngine();
            }
        };

        _brakeEvent.PointerDown = () =>
        {
            if (!_isKey)
            {
                OpenToturialKey();
            }
            else if (!_isStart)
            {
                OpenToturialEngine();
            }
        };
        AdmobManager.Instance.SetBannerId("BannerPlayId");
    }

    public override void Hide()
    {
        AdmobManager.Instance.ShowInter("InterBackId", () =>
        {
            base.Hide();
            _isKey = false; _isLight = false; _isWaring = false; _isStart = false;
            DisableAllStatus();
            GameEvent.OnRefill -= GameEvent_OnRefill;
            ScreenHelper.FindScript<ScreenSelect>().Show();
            SoundManager.Instance.Stop();
            initialized = true;
        });
    }

    private void GameEvent_OnRefill()
    {
        isComplete = false;
        _gasLimit = 0;
        _imgGas.fillAmount = _gasLimit / _gasMax;
        DOTween.To(() => _gasLimit, x => _gasLimit = x, _gasMax, 1f)
            .OnUpdate(() => {
                _imgGas.fillAmount = _gasLimit / _gasMax;
            }).OnComplete(() =>
            {
                isComplete = true;
            });

    }

    public void Setting()
    {
        PopupHelper.FindScript<PopupSetting>().Show();
    }

    private void StartAnimation()
    {
        for (int i = 0; i < lstAnimationJobs.Count; i++)
        {
            int jobIndex = i;
            SetJob(jobIndex);
            CoroutineUtils.PlayCoroutine(() =>
            {
                isComplete = false;
                StartJob(jobIndex);
            }, lstAnimationJobs[i].turn / animationSpeed);
        }
    }

    private Dictionary<int, Vector3[]> _cacheStartJob = new Dictionary<int, Vector3[]>();
    private bool initialized = false;

    private void SetJob(int jobIndex)
    {
        if(initialized)
        {
            for (int i = 0; i < _cacheStartJob[jobIndex].Length; i++)
            {
                lstAnimationJobs[jobIndex].animationJobs[i].tranform.position = _cacheStartJob[jobIndex][i];
            }
        }
    }

    private void StartJob(int jobIndex)
    {
        if(!initialized)
        {
            if (!_cacheStartJob.ContainsKey(jobIndex))
                _cacheStartJob.Add(jobIndex, new Vector3[lstAnimationJobs[jobIndex].animationJobs.Count]);
            for (int i = 0; i < lstAnimationJobs[jobIndex].animationJobs.Count; i++)
            {
                _cacheStartJob[jobIndex][i] = lstAnimationJobs[jobIndex].animationJobs[i].tranform.position;

                lstAnimationJobs[jobIndex].animationJobs[i].tranform.DOKill();
                lstAnimationJobs[jobIndex].animationJobs[i].tranform.DOMove(lstAnimationJobs[jobIndex].animationJobs[i].target.position, 0.3f);
            }
        }
        else
        {
            for (int i = 0; i < _cacheStartJob[jobIndex].Length; i++)
            {
                lstAnimationJobs[jobIndex].animationJobs[i].tranform.DOKill();
                lstAnimationJobs[jobIndex].animationJobs[i].tranform.DOMove(lstAnimationJobs[jobIndex].animationJobs[i].target.position, 0.3f);
            }
        }


        isComplete = true;
    }

    private string[] _datas;
    public void OnCar(string _data)
    {
        if (!isComplete)
            return;

        _datas = _data.Split('_');
        switch(_datas[0])
        {
            case "key":
                if(_datas[1] == "on")
                {
                    SoundManager.Instance.Play(GameData.GetAudio(GameData.PositionType.Key));
                    CoroutineUtils.PlayCoroutine(() =>
                    {
                        _isKey = _datas[1] == "on" ? true : false;
                        if (!_isKey)
                        {
                            _isLight = false; _isWaring = false; _isStart = false;
                            DisableAllStatus();
                        }

                        if (PrefManager.GetBool(KeyStart, false) == false)
                        {
                            OpenToturialEngine();
                            PrefManager.SetBool(KeyStart, true);
                        }
                    }, 0.3f);
                }
                else
                {
                    _isKey = _datas[1] == "on" ? true : false;
                    if (!_isKey)
                    {
                        _isLight = false; _isWaring = false; _isStart = false;
                        DisableAllStatus();
                        SoundManager.Instance.Stop();
                    }
                }
                break;
            case "light":
                if(_isKey)
                {
                    _isLight = _datas[1] == "on" ? true : false;
                }
                else
                {
                    OpenToturialKey();
                }    
                break;
            case "warning":
                if (_isKey)
                {
                    _isWaring = _datas[1] == "on" ? true : false;
                    SoundManager.Instance.PlaySpecial(GameData.GetAudio(GameData.PositionType.Sign), _isWaring, true);
                }
                else
                {
                    OpenToturialKey();
                }    
                break;
            case "start":
                if (_isKey)
                {
                    var _audioClip = GameData.GetAudio(GameData.PositionType.Start);
                    SoundManager.Instance.Play(_audioClip);
                    CoroutineUtils.PlayCoroutine(() =>
                    {
                        SoundManager.Instance.Play(GameData.GetAudio(GameData.PositionType.Idle), true, false);
                        _isStart = _datas[1] == "on" ? true : false;
                    }, _audioClip.length);
                }
                else
                {
                    OpenToturialKey();
                }
                break;
            case "horn":
                if (_isKey)
                {
                    PopupHelper.FindScript<PopupHorn>().Show();
                }
                else
                {
                    OpenToturialKey();
                }
                break;
            case "airbag":
                if (_isKey)
                {
                    PopupHelper.FindScript<PopupAirBag>().Show();
                }
                else
                {
                    OpenToturialKey();
                }
                break;
        }
    }

    private void ButtonStatus()
    {
        if(isKey || _datas[0] == "key")
        {
            switch (_datas[1])
            {
                case "on":
                    if (IsJob(out Transform _action))
                    {
                        _action.SetActive(!_action.IsActive());
                    }
                    break;
                case "off":
                    EventSystem.current.currentSelectedGameObject.SetActive(false);
                    break;
            }
        }
    }

    private void DisableAllStatus()
    {
        for(int i = 0; i < actionJobs.Length; i++)
        {
            actionJobs[i].action.SetActive(false);
        }
    }

    public bool IsJob(out Transform _action)
    {
        for(int i = 0; i < actionJobs.Length; i++)
        {
            if(EventSystem.current.currentSelectedGameObject == actionJobs[i].job.gameObject)
            {
                _action = actionJobs[i].action;
                return true;
            }
        }
        _action = null;
        return false;
    }


    private bool isGasPressed = false;
    private bool isBrakePressed = false;

    private void Update()
    {
        if(isKey != _isKey)
        {
            isKey = _isKey;
            ButtonStatus();
        }

        if (isLight != _isLight)
        {
            isLight = _isLight;
            ButtonStatus();
        }

        if (isStart != _isStart)
        {
            isStart = _isStart;
            ButtonStatus();
        }

        if (isWaring != _isWaring)
        {
            isWaring = _isWaring;
            _signal_Left.SetActive(isWaring);
            _signal_Right.SetActive(isWaring);
            ButtonStatus();
        }

        if(isStart)
        {
            if(isBrakePressed != _brakeEvent.isPressed)
            {
                isBrakePressed = _brakeEvent.isPressed;
                if(isBrakePressed)
                    SoundManager.Instance.PlaySoundUpdateVolume(GameData.GetAudio(GameData.PositionType.Stop), 0.8f, 1f);
                else
                    SoundManager.Instance.PlaySoundLoopWithTime(GameData.GetAudio(GameData.PositionType.Idle));
            }

            if(!isBrakePressed && isGasPressed != _gasEvent.isPressed)
            {
                isGasPressed = _gasEvent.isPressed;
                if (isGasPressed)
                {
                    _engine_rpm_wise.DOLocalRotate(new Vector3(0, 0, -160f), 3f);
                    var _value = _engine_speed_wise.localEulerAngles.z;
                    if(_value < 120)
                    {
                        _engine_speed_wise.localEulerAngles = _engine_speed_wise.localEulerAngles.WithZ(359);
                        _value = 359;
                    }
                    DOTween.To(() => _value, x => _value = x, 120, 2.5f).OnUpdate(() =>
                    {
                        _engine_speed_wise.localEulerAngles = _engine_speed_wise.localEulerAngles.WithZ(_value);
                    });
                    SoundManager.Instance.PlaySoundUpdateVolume(GameData.GetAudio(GameData.PositionType.Pedan));
                }
                else
                {
                    _engine_rpm_wise.DOLocalRotate(new Vector3(0, 0, 0), 3f);
                    var _value = _engine_speed_wise.localEulerAngles.z;
                    DOTween.To(() => _value, x => _value = x, 360, 2.5f).OnUpdate(() =>
                    {
                        _engine_speed_wise.localEulerAngles = _engine_speed_wise.localEulerAngles.WithZ(_value);
                    });
                    SoundManager.Instance.PlaySoundLoopWithTime(GameData.GetAudio(GameData.PositionType.Idle));
                }
            }

            if(!isBrakePressed && _gasEvent.isPressed)
            {
                _gasLimit -= Time.deltaTime * _gasSpendPedan;
                _imgGas.fillAmount = _gasLimit / _gasMax;
            }
            else
            {
                _gasLimit -= Time.deltaTime * _gasSpendDefault;
                _imgGas.fillAmount = _gasLimit / _gasMax;
            }

            if(_gasLimit <= 0)
            {
                _isKey = false; _isLight = false; _isWaring = false; _isStart = false;
                DisableAllStatus();
                SoundManager.Instance.Stop();
                PopupHelper.FindScript<PopupRefill>().Show();
            }
        }
    }
}
