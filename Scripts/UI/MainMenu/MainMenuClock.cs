using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuClock : MonoBehaviour
{
    const int _secondsPerHour = 3600;
    const int _secondsPerTwelveHours = 43200;

    const int _hourHandOffset = 0;
    const int _minuteHandOffset = 0;

    [SerializeField] private GameObject _minuteHand;
    [SerializeField] private GameObject _hourHand;

    void Start()
    {
        InvokeRepeating(nameof(UpdateClock), 0, 1);
    }

    public void UpdateClock()
    {

        DateTime currentTime = DateTime.Now;

        float minuteFraction = ((float)(currentTime.Minute * 60) + (float)currentTime.Second) / (float)_secondsPerHour;
        float minuteRotation = 360 * minuteFraction;

        _minuteHand.transform.localRotation = Quaternion.Euler(minuteRotation + _minuteHandOffset, 0, 0);

        float hourFraction = ((float)(currentTime.Hour * _secondsPerHour) + (float)(currentTime.Minute * 60)) / (float)_secondsPerTwelveHours;
        float hourRotation = 360 * hourFraction;

        _hourHand.transform.localRotation = Quaternion.Euler(hourRotation + _hourHandOffset, 0, 0);
    }
}
