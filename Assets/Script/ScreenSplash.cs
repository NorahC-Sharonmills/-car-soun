using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ScreenSplash : MonoBehaviour
{
    public GameObject _loading;
    public GameObject _loadingAnimation;

    private float timer = 0;
    private bool isAnimation = false;

    //private float timerCdCallback = 0;
    //private Action loadingCallback;
    //private bool isAutoCallback = false;
    //private Coroutine _showInterAwait;

    public Image _loadingFill;
    private Coroutine coroutine;

    public void Show()
    {
        this.gameObject.SetActive(true);
        coroutine = StartCoroutine(CorotineLoading());
    }

    private IEnumerator CorotineLoading()
    {
        yield return SimpleCoroutine.MoveTowardsEnumerator(start: 1, end: 0, onCallOnFrame: (_frame) =>
        {
            _loadingFill.fillAmount = _frame;
        }, speed: 0.5f);
        yield return null;
        StaticVariable.isLoaded = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 5 && !isAnimation)
        {
            isAnimation = true;
            _loading.SetActive(false);
            _loadingAnimation.SetActive(true);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
