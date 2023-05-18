using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSerializable
{
    public string Id = "hihidochoo";
    public bool IsAds = true;
    public int Gold;

    public int Car;
    public int[] Cars;

    public string Language = "";

    public PlayerSerializable()
    {
        Id = SystemInfo.deviceUniqueIdentifier;
        IsAds = true;
        Gold = 0;
        Car = 0;
        Cars = new int[14] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Language = "";
    }
}

