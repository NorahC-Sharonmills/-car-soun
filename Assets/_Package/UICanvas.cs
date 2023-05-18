using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public enum OpenType
    {
        None,
        Scale
    }

    public OpenType openType = OpenType.None;
    public Transform popup;
    [HideInInspector] public float time;
    private Vector3 _originalScale;
    private Vector3 _toScale;

    public virtual void Show()
    {
        switch(openType)
        {
            case OpenType.None:
                this.gameObject.SetActive(true);
                break;
            case OpenType.Scale:
                this.gameObject.SetActive(true);
                _originalScale = transform.localScale;
                _toScale = transform.localScale * 1.2f;
                this.popup.localScale = Vector3.zero;

                this.popup.DOKill();
                this.popup.DOScale(_toScale, 0.3f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        popup.DOScale(_originalScale, 0.1f)
                            .SetEase(Ease.OutBounce);
                    });
                break;
        }
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
