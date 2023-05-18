using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLoading : MonoSingletonGlobal<ScreenLoading>
{
    public Image _loading_fill;
    private Coroutine coroutine;

    private IEnumerator CorotineLoading()
    {
        yield return SimpleCoroutine.MoveTowardsEnumerator(start: 1, end: 0, onCallOnFrame: (_frame) =>
        {
            _loading_fill.fillAmount = _frame;
        }, speed: 0.5f);
        yield return null;
        StaticVariable.isLoaded = true;
    }

    public void Hide()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        this.transform.SetActive(false);
    }

    public void Show()
    {
        this.transform.SetActive(true);
        coroutine = StartCoroutine(CorotineLoading());
    }
}
