using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AntiSpamClick : MonoBehaviour
{
    private Button button;
    public float timer = 0.5f;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            if(gameObject.activeInHierarchy)
            {
                button.interactable = false;
                StartCoroutine(ReActiveButton());
            }
        });
    }

    private IEnumerator ReActiveButton()
    {
        if (!gameObject.activeInHierarchy)
            yield break;
        yield return WaitForSecondCache.GetWFSCache(timer);
        button.interactable = true;
    }
}
