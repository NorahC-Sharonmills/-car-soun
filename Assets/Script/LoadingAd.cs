using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAd : MonoBehaviour
{
    public GameObject loadingAd;
    private float timer = 3f;
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 5f)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
                if (loadingAd.activeInHierarchy)
                    loadingAd.SetActive(false);
            }
            else
            {
                if (!loadingAd.activeInHierarchy)
                    loadingAd.SetActive(true);
            }
        }
    }
}
