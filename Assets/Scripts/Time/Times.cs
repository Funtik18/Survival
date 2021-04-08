using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public struct Times
{
    [SuffixLabel("d", true)]
    [Min(0)]
    public int days;
    [SuffixLabel("h", true)]
    [Range(0, 24)]
    public int hours;
    [SuffixLabel("m", true)]
    [Range(0, 60)]
    public int minutes;
    [SuffixLabel("s", true)]
    [Range(0, 60)]
    public int seconds;

    public int allSeconds;

    public void RandomHours()
    {
        hours = Random.Range(0, 24);
    }
    public void RandomMinutes()
    {
        minutes = Random.Range(0, 60);
    }
    public void RandomSeconds()
    {
        seconds = Random.Range(0, 60);
    }

    public void Reset()
    {
        days = 0;
        hours = 0;
        minutes = 0;
        seconds = 0;
    }

    public void CheckTime()
    {
        while (seconds >= 60)
        {
            seconds -= 60;
            minutes += 1;
        }
        while (minutes >= 60)
        {
            minutes -= 60;
            hours += 1;
        }
        while (hours >= 24)
        {
            hours -= 24;
            days += 1;
        }
    }

    public void SetTimeBySeconds(int secs)
    {
        days = secs / (24 * 3600);

        secs = secs % (24 * 3600);
        hours = secs / 3600;

        secs %= 3600;
        minutes = secs / 60;

        secs %= 60;
        seconds = secs;
    }

    public float GetDayPercent()
    {
        return (float)GetAllMinutes() / 1440f;
    }
    public int GetAllMinutes()
    {
        int allMinutes = 0;

        for (int i = 0; i < hours; i++)
        {
            allMinutes += 60;
        }
        allMinutes += minutes;
        return allMinutes;
    }
    public int GetAllSeconds()
    {
        int allSeconds = 0;
        for (int i = 0; i < days; i++)
        {
            allSeconds += 86400;
        }
        for (int i = 0; i < hours; i++)
        {
            allSeconds += 3600;
        }
        for (int i = 0; i < minutes; i++)
        {
            allSeconds += 60;
        }
        allSeconds += seconds;

        return allSeconds;
    }

    public void UpdateTimeFromSky(float skyTime)
    {
        SetTimeBySeconds((int)(skyTime * 86400));
    }

    public static Times operator +(Times currTime, Times addTime)
    {
        currTime.days += addTime.days;
        currTime.hours += addTime.hours;
        currTime.minutes += addTime.minutes;
        currTime.seconds += addTime.seconds;

        currTime.CheckTime();

        return currTime;
    }

    public static bool operator !=(Times time0, Times time1)
    {
        if (time0.days != time1.days || time0.hours != time1.hours || time0.minutes != time1.minutes || time0.seconds != time1.seconds)
            return true;
        return false;
    }
    public static bool operator ==(Times time0, Times time1)
    {
        if (time0.days == time1.days && time0.hours == time1.hours && time0.minutes == time1.minutes && time0.seconds == time1.seconds)
            return true;
        return false;
    }

    public static bool operator <=(Times time0, Times time1)
    {
        if (time0 < time1)
            return true;

        return time0 == time1;
    }
    public static bool operator >=(Times time0, Times time1)
    {
        if (time0 > time1)
            return true;

        return time0 == time1;
    }

    public static bool operator >(Times time0, Times time1)
    {
        return time0.GetAllSeconds() > time1.GetAllSeconds();
    }
    public static bool operator <(Times time0, Times time1)
    {
        return time0.GetAllSeconds() < time1.GetAllSeconds();
    }

    public override string ToString()
    {
        return days + ":" + hours + ":" + minutes + ":" + seconds;
    }
    public string ToStringSimplification(bool showSecs = false)
    {
        string result = "";
        if (days != 0) result += days + "D ";
        if (hours != 0) result += hours + "H ";
        if (minutes != 0) result += minutes + "M ";
        if (showSecs && seconds != 0) result += seconds + "S";

        result = result == "" ? "-" : result;

        return result;
    }
}
