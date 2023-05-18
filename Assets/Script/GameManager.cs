using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Transform _car_container;
    public Camera _cam;
    public RotateEvent _rotate;

    private void Start()
    {
        AdmobManager.Instance.AwaitShowInter("InterSplashId",
            () => {
                AdmobManager.Instance.IsOpenAd = true;
                AdmobManager.Instance.IsOpenAdShow = false;
                Manager.Instance.CompleteOpenAd();
            },
            () => Manager.Instance.HideSplash(), 5f);
    }

    public void Show(string _car)
    {
        _car_container.ForChild((_child) =>
        {
            if (_child.name.ToLower() == _car.ToLower())
                _child.SetActive(true);
            else
                _child.SetActive(false);
        });

        _rotate.IsActive = true;
        _rotate.transform.localPosition = Vector3.zero;
        _rotate.transform.localEulerAngles = Vector3.zero;
    }

    public void Hide()
    {
        _rotate.IsActive = false;
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ScreenHelper.NativeBack();
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ScreenHelper.NativeBack();
        }
#endif
    }
}
