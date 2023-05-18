using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartRotation : MonoBehaviour
{
    public float speed = -45;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 2 * speed * Time.deltaTime)); // Xoay 45 độ theo trục y
    }
}
