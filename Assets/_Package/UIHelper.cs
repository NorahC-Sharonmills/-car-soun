using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour
{
    private static UIHelper _instance;
    private void Awake() { _instance = this; }

    public static T FindScript<T>()
    {
        for (int i = 0; i < _instance.transform.childCount; i++)
        {
            var _child = _instance.transform.GetChild(i);
            var _childScript = _child.GetComponent<T>();
            if (_childScript != null)
                return _childScript;
        }

        return default(T);
    }

    public static T FindScriptAndHide<T>()
    {
        T response = default(T);
        for (int i = 0; i < _instance.transform.childCount; i++)
        {
            var _child = _instance.transform.GetChild(i);
            var _childScript = _child.GetComponent<T>();
            if (_childScript != null)
                response = _childScript;
            else
                _child.gameObject.SetActive(false);
        }

        return response;
    }
}
