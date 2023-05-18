using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupNotification : UICanvas
{
    public Text titleText;
    public Text descriptionText;

    public void Initialized(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;

        base.Show();
    }
}
