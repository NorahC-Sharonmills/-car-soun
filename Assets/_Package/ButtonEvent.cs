using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvent : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed;
    public Action PointerDown;
    public Action PointerUp;

    public void OnUpdateSelected(BaseEventData data)
    {

    }
    public void OnPointerDown(PointerEventData data)
    {
        PointerDown?.Invoke();
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        PointerUp?.Invoke();
        isPressed = false;
    }
}
