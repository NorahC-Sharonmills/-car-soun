﻿[System.Serializable]
public class SoundSerializable
{
    public bool isSound = true;
    public bool isMusic = true;
    public bool isVibrate = true;

    public SoundSerializable()
    {
        isMusic = true;
        isSound = true;
        isVibrate = true;
    }
}
