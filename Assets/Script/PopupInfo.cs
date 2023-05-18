using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfo : UICanvas
{
    public Text _name;
    public Text _maxSpeed;
    public Text _threadSpeed;
    public Text _drive;
    public Text _power;

    public void Initialized(CarInfo _data)
    {
        base.Show();
        _name.text = _data.Name;
        _maxSpeed.text = _data.TopSpeed;
        _threadSpeed.text = _data.ThreadSpeed;
        _drive.text = _data.Drive;
        _power.text = _data.Power;
    }
}
