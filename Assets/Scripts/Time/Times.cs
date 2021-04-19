using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System;
using System.Globalization;
using UnityEngine.Events;

[System.Serializable]
public struct Times
{
    [OnValueChanged("CheckTimeSeconds")]
    [SuffixLabel("d", true)]
    [Min(0)]
    public int days;
    [OnValueChanged("CheckTimeSeconds")]
    [SuffixLabel("h", true)]
    [Range(0, 24)]
    public int hours;
    [OnValueChanged("CheckTimeSeconds")]
    [SuffixLabel("m", true)]
    [Range(0, 60)]
    public int minutes;
    [OnValueChanged("CheckTimeSeconds")]
    [SuffixLabel("s", true)]
    [Range(0, 60)]
    public int seconds;

    public TimesState State
    {
        get
        {
            if (hours >= 6 && hours <= 12)
            {
                return TimesState.Morning;
            }
            else if (hours > 12 && hours <= 17)
            {
                return TimesState.Afternoon;
            }
            else if (hours > 17 && hours <= 20)
            {
                return TimesState.Evening;
            }
            return TimesState.Night;
        }
    }

    [ReadOnly]
    [SerializeField] private int totalSeconds;
    public int TotalSeconds
    {
        get => totalSeconds;
        set
        {
            totalSeconds = value;
            CheckTime();
        }
    }

    public void Reset()
    {
        TotalSeconds = 0;
        CheckTime();
    }

    /// <summary>
    /// TimeSpan to totalSeconds
    /// </summary>
    public void CheckTimeSeconds()
    {
        TimeSpan span = new TimeSpan(days, hours, minutes, seconds);
        totalSeconds = (int) span.TotalSeconds;
    }
    /// <summary>
    /// totalSeconds to TimeSpan
    /// </summary>
    public void CheckTime()
    {
        TimeSpan span = TimeSpan.FromSeconds(TotalSeconds);

        days = span.Days;
        hours = span.Hours;
        minutes = span.Minutes;
        seconds = span.Seconds;
    }

    public float GetSkyPercent() => TotalSeconds / 86400f;

    public float GetDayPercent() => GetSkyPercent() % 1;


    public int GetAllMinutes() => TotalSeconds / 60;

    public void UpdateTimeFromSky(float skyTime)
    {
        TotalSeconds = (int)(skyTime * 86400f);
    }


    public void RandomTime()
    {
        hours = UnityEngine.Random.Range(0, 24);
        minutes = UnityEngine.Random.Range(0, 60);
        seconds = UnityEngine.Random.Range(0, 60);

        CheckTimeSeconds();
    }
    public void RandomDay()
    {
        //why
    }

    public void RandomHours()
    {
        hours = UnityEngine.Random.Range(0, 24);

        CheckTimeSeconds();
    }
    public void RandomMinutes()
    {
        minutes = UnityEngine.Random.Range(0, 60);

        CheckTimeSeconds();
    }
    public void RandomSeconds()
    {
        seconds = UnityEngine.Random.Range(0, 60);

        CheckTimeSeconds();
    }

    #region Overrides
    public static Times operator +(Times currTime, Times addTime)
    {
        currTime.TotalSeconds += addTime.TotalSeconds;

        return currTime;
    }
    public static Times operator -(Times currTime, Times addTime)
    {
        int result = currTime.TotalSeconds;
        result -= addTime.TotalSeconds;
        result = Mathf.Abs(result);
        currTime.TotalSeconds = result;
        return currTime;
    }

    public static bool operator !=(Times time0, Times time1) => time0.TotalSeconds != time1.TotalSeconds;
    public static bool operator ==(Times time0, Times time1) => time0.TotalSeconds == time1.TotalSeconds;

    public static bool operator <=(Times time0, Times time1) => time0.TotalSeconds <= time1.TotalSeconds;
    public static bool operator >=(Times time0, Times time1) => time0.TotalSeconds >= time1.TotalSeconds;

    public static bool operator >(Times time0, Times time1) => time0.TotalSeconds > time1.TotalSeconds;
    public static bool operator <(Times time0, Times time1) => time0.TotalSeconds < time1.TotalSeconds;

    private const char Infinity = '\u221E';
    private const char Dash = '-';


    public override string ToString()
    {
        return days + ":" + hours + ":" + minutes + ":" + seconds;
    }
    public string ToStringSimplification(bool showSecs = false, bool isInfinity = false)
    {
        string result = "";
        if (isInfinity)
        {
            result = Infinity.ToString();
            return result;
        }


        if (days != 0) result += days + "D ";
        if (hours != 0) result += hours + "H ";
        if (minutes != 0) result += minutes + "M ";
        if (showSecs && seconds != 0) result += seconds + "S";

        result = result == "" ? Dash.ToString() : result;

        return result;
    }
    #endregion


    public enum TimesState
    {
        Morning,
        Noon,
        Afternoon,
        Evening,
        Night,
    }
}