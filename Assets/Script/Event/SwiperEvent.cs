using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwiperEvent : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed;
    public enum Status
    {
        On, Off
    }

    public Status status = Status.Off;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    public AudioClip _swipeMove;
    public GameObject[] _events;

    public void OnUpdateSelected(BaseEventData data)
    {

    }

    public void OnPointerDown(PointerEventData data)
    {
        isPressed = true;
        _startPosition = data.position;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if(isPressed)
        {
            _endPosition = data.position;
            isPressed = false;

            if (_endPosition.y < _startPosition.y)
            {
                status = Status.On;
                this.transform.localEulerAngles = Vector3.zero.WithZ(-20);
                SoundManager.Instance.PlayLoopSpecial(_swipeMove);
                _events.ForEach((_event) =>
                {
                    _event.SetActive(true);
                });
            }
            else
            {
                status = Status.Off;
                this.transform.localEulerAngles = Vector3.zero;
                SoundManager.Instance.StopSpecial();
                _events.ForEach((_event) =>
                {
                    _event.SetActive(false);
                });
            }
        }    
    }
}
