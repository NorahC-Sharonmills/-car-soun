using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoSingletonGlobal<GameData>
{
    public enum CarType
    {
        Lamborghini_Sian,
        Audi_R8,
        Bugati_Veyron,
        Bugati_Bolide,
        Lamborghini_Ferzor,
        Ferrari_Enzo,
        Ferrari_La,
        Lamborghini_Centenario,
        Lamborghini_Aventador,
        Lykan_Hypersport,
        Tesla_Model_S,
        Bugati_Veneno
    }

    public enum PositionType
    {
        Key,
        Pedan,
        Start,
        Idle,
        Stop,
        Sign,
        Warning,
        Airbag
    }

    public AudioClip _sound_gas;
    public AudioClip _sound_key;
    public AudioClip[] _sound_start;
    public AudioClip[] _sound_idle;
    public AudioClip _sound_stop;
    public AudioClip _sound_sign;
    public AudioClip _sound_warning;
    public AudioClip _sound_air_bag;

    public int _indexCar = 0;
    public void SetIndexCar(int _index)
    {
        _indexCar = _index;
    }

    private AudioClip GetStartAudioClip(CarType carType)
    {
        AudioClip audioClip = null;

        var _index = (int)carType;
        if (_index >= _sound_start.Length)
            _index = _sound_start.Length - 1;
        audioClip = _sound_start[_index];

        return audioClip;
    }

    private AudioClip GetIdleAudioClip(CarType carType)
    {
        AudioClip audioClip = null;

        var _index = (int)carType;
        if (_index >= _sound_idle.Length)
            _index = _sound_idle.Length - 1;
        audioClip = _sound_idle[_index];

        return audioClip;
    }

    private AudioClip GetAudioClip(PositionType positionType)
    {
        AudioClip audioClip = null;
        switch(positionType)
        {
            case PositionType.Key:
                audioClip = _sound_key;
                break;
            case PositionType.Pedan:
                audioClip = _sound_gas;
                break;
            case PositionType.Stop:
                audioClip = _sound_stop;
                break;
            case PositionType.Start:
                audioClip = GetStartAudioClip((CarType)_indexCar);
                break;
            case PositionType.Idle:
                audioClip = GetIdleAudioClip((CarType)_indexCar);
                break;
            case PositionType.Sign:
                audioClip = _sound_sign;
                break;
            case PositionType.Warning:
                audioClip = _sound_warning;
                break;
            case PositionType.Airbag:
                audioClip = _sound_air_bag;
                break;

        }
        return audioClip;
    }

    public static AudioClip GetAudio(PositionType positionType)
    {
        return Instance.GetAudioClip(positionType);
    }
}
