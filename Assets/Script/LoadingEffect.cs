using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingEffect : MonoBehaviour
{
    private float timer = 0;
    public float speed = 1;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * speed;
        if(timer > 1f)
        {
            timer = 0;
            var child = this.transform.GetChild(this.transform.childCount - 1);
            child.SetAsFirstSibling();
        }
    }
}
