using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralTime : MonoBehaviour
{
    private static GeneralTime instance;
    public static GeneralTime Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GeneralTime>();
            }
            return instance;
        }
    }


    [HideLabel]
    public GlobalTime time;

    private void Awake()
    {
        time.StartTime(this);
    }

    /// <summary>
    /// Пропустить сколько, за столько
    /// </summary>
    /// <param name="s"></param>
    /// <param name="skipTime"></param>
    public void SkipTime(Times s, float skipTime)
    {
        time.isStopped = true;
        //Times times = time.globalTime + skip;
        Times a = time.globalTime;
        Times b = a + s;

        float v = s.GetAllSeconds() / skipTime;
        Debug.LogError(a.ToStringSimplification() + " - " + b.ToStringSimplification() + " s = " + s.ToStringSimplification() + "  v = " + v);
    }


    void OnGUI()
    {
        GUI.Label(new Rect(5, 80, 100, 25), time.ToString());
    }

    [System.Serializable]
    public class GlobalTime
    {
        public Times globalTime;

        public DayCycle cycle;

        [InfoBox("Time Flow = 1 / 12")]
        [Space]
        [SerializeField] private float timeFlow = 1f;
        public float TimeFlow
        {
            get => timeFlow;
            set
            {
                timeFlow = value;

                seconds = new WaitForSeconds(timeFlow);
            }
        }

        public bool isStopped = false;


        private Coroutine timeCoroutine = null;
        public bool IsTimeProccess => timeCoroutine != null;

        private MonoBehaviour owner;
        public WaitForSeconds seconds;

        public void StartTime(MonoBehaviour owner)
        {
            if (!IsTimeProccess)
            {
                this.owner = owner;
                seconds = new WaitForSeconds(timeFlow);
                timeCoroutine = this.owner.StartCoroutine(TimeRoutine());
            }
        }
        private IEnumerator TimeRoutine()
        {
            while (true)
            {
                globalTime.seconds += 1;
                globalTime.CheckTime();

                UpdateCicly();
                yield return seconds;

                while (isStopped)
                    yield return null;
            }

            StopTime();
        }
        private void StopTime()
        {
            if (IsTimeProccess)
            {
                owner.StopCoroutine(timeCoroutine);
                timeCoroutine = null;
            }
        }


        [Button]
        public void Init()
        {
            globalTime.RandomHours();
            globalTime.RandomMinutes();
            globalTime.RandomSeconds();
            UpdateCicly();
        }
        [Button]
        public void Reset()
        {
            globalTime.Reset();
            UpdateCicly();
        }
        [Button]
        private void UpdateCicly()
        {
            cycle.UpdateCycle(globalTime.GetDayPercent());
        }

        public override string ToString()
        {
            return globalTime.ToString();
        }
    }

    [System.Serializable]
    public class DayCycle
    {
        [SerializeField] private LightingPresetSD preset;
        [Space]
        [SerializeField] private Illuminate sun;
        [SerializeField] private Illuminate moon;

        public void UpdateCycle(float dayTime)
        {
            UpdateLights(dayTime);
        }
        private void UpdateLights(float timePercent)
        {
            sun.SetRotation(Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0)));
            //moon.SetRotation(Quaternion.Euler(dayTime * 360f + 180, 180, 0));

            //sun.UpdateIntensity(timePercent);
            //moon.UpdateIntensity(timePercent);

            RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
            RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);
            sun.SetColor(preset.DirectionalColor.Evaluate(timePercent));
        }


        [System.Serializable]
        public class Illuminate
        {
            [SerializeField] private Light light;
            [OnValueChanged("RefreshLightIntensity")]
            [Min(0)]
            [SerializeField] private float lightBaseIntensity = 1;
            [SerializeField] private AnimationCurve lightIntensityCurve;

            public void UpdateIntensity(float dayTime)
            {
                light.intensity = lightBaseIntensity * lightIntensityCurve.Evaluate(dayTime);
            }

            public void SetColor(Color color)
            {
                light.color = color;
            }
            public void SetRotation(Quaternion rotation)
            {
                light.transform.localRotation = rotation;
            }

            private void RefreshLightIntensity()
            {
                light.intensity = lightBaseIntensity;
            }
        }
    }
}

[System.Serializable]
public struct Times
{
    [SuffixLabel("d", true)]
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
        while(seconds >= 60)
        {
            seconds -= 60;
            minutes += 1;
        }
        while(minutes >= 60)
        {
            minutes -= 60;
            hours += 1;
        }
        while(hours >= 24)
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
        int allMinutes = 0;

        for (int i = 0; i < hours; i++)
        {
            allMinutes += 60;
        }
        allMinutes += minutes;

        return allMinutes / 1440f;
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


    public static Times operator +(Times currTime, Times addTime)
    {
        currTime.days += addTime.days;
        currTime.hours += addTime.hours;
        currTime.minutes += addTime.minutes;
        currTime.seconds += addTime.seconds;

        currTime.CheckTime();

        return currTime;
    }

    public override string ToString()
    {
        return days + ":" + hours + ":" + minutes + ":" + seconds;
    }
    public string ToStringSimplification(bool showSecs = false)
    {
        string result = "";
        if (days != 0) result += days + "D";
        if (hours != 0) result += hours + "H";
        if (minutes != 0) result += minutes + "M";
        if (showSecs && seconds != 0) result += seconds + "S";

        result = result == "" ? "-" : result;

        return result;
    }
}
