using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEvent : MonoBehaviour
{
    public float speed = 5;
    private bool IsRotate = true;
    public Transform target;
    public float distanceToTarget = 10;

    public bool IsActive = false;

    private Vector3 previousPosition;
    private bool isMouse = false;
    private float time = 0;
    void Update()
    {
        if (!IsActive)
            return;

        if (IsRotate)
            transform.Rotate(0, speed * Time.deltaTime, 0);

        CaculateRotationCar();
    }

    private void CaculateRotationCar()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouse = true;
            time = 0;
            previousPosition = GameManager.Instance._cam.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            isMouse = true;
            time = 0;
            Vector3 newPosition = GameManager.Instance._cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            GameManager.Instance._cam.transform.position = target.position;
            GameManager.Instance._cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            GameManager.Instance._cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <? This is what makes it work!

            if (GameManager.Instance._cam.transform.localEulerAngles.x < 15)
                GameManager.Instance._cam.transform.localEulerAngles = GameManager.Instance._cam.transform.localEulerAngles.WithX(15);
            if (GameManager.Instance._cam.transform.localEulerAngles.x > 45)
                GameManager.Instance._cam.transform.localEulerAngles = GameManager.Instance._cam.transform.localEulerAngles.WithX(45);
            GameManager.Instance._cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;
        }
        else { isMouse = false; }

        if (isMouse)
            IsRotate = false;
        else
        {
            time += Time.deltaTime;
            if (time > 1f && IsRotate == false)
                IsRotate = true;
        }
    }
}
