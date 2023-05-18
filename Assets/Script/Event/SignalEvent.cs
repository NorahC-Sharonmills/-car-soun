using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SignalEvent : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed;
    public enum Status
    {
        Left, Off, Right
    }

    public Status status = Status.Off;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    public AudioClip _signalControl;

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
        if (isPressed)
        {
            _endPosition = data.position;
            isPressed = false;
            if(_startPosition.y > _endPosition.y)
            {
                Debug.Log("1");
                var _value = (int)status;
                Debug.Log(_value);
                _value -= 1;
                if (_value < 0)
                    return;
                status = (Status)_value;
                switch (status)
                {
                    case Status.Off:
                        _events.ForEach((_event) =>
                        {
                            _event.SetActive(false);
                        });
                        this.transform.localEulerAngles = this.transform.localEulerAngles.WithZ(0);
                        SoundManager.Instance.StopSpecial();
                        break;
                    case Status.Left:
                        _events[0].SetActive(true);
                        _events[1].SetActive(false);
                        this.transform.localEulerAngles = this.transform.localEulerAngles.WithZ(20);
                        SoundManager.Instance.PlayLoopSpecial(_signalControl);
                        break;
                    case Status.Right:
                        _events[0].SetActive(false);
                        _events[1].SetActive(true);
                        this.transform.localEulerAngles = this.transform.localEulerAngles.WithZ(-20);
                        SoundManager.Instance.PlayLoopSpecial(_signalControl);
                        break;
                }
            }
            else if(_startPosition.y < _endPosition.y)
            {
                Debug.Log("2");
                var _value = (int)status;
                Debug.Log(_value);
                _value += 1;
                if (_value > 2)
                    return;
                status = (Status)_value;
                switch (status)
                {
                    case Status.Off:
                        _events.ForEach((_event) =>
                        {
                            _event.SetActive(false);
                        });
                        this.transform.localEulerAngles = this.transform.localEulerAngles.WithZ(0);
                        SoundManager.Instance.StopSpecial();
                        break;
                    case Status.Left:
                        _events[0].SetActive(true);
                        _events[1].SetActive(false);
                        this.transform.localEulerAngles = this.transform.localEulerAngles.WithZ(20);
                        SoundManager.Instance.PlayLoopSpecial(_signalControl);
                        break;
                    case Status.Right:
                        _events[0].SetActive(false);
                        _events[1].SetActive(true);
                        this.transform.localEulerAngles = this.transform.localEulerAngles.WithZ(-20);
                        SoundManager.Instance.PlayLoopSpecial(_signalControl);
                        break;
                }
            }
            else
            {
                status = Status.Off;
                _events.ForEach((_event) =>
                {
                    _event.SetActive(false);
                });
                SoundManager.Instance.StopSpecial();
            }
        }
    }
}
