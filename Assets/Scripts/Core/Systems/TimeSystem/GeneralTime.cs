using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Funly.SkyStudio;
using Sirenix.OdinInspector;
using UnityEngine.Events;

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

    public UnityAction onDay;
    public UnityAction onState;
    public UnityAction onHour;
    public UnityAction onMinute;
    public UnityAction onSecond;

    public UnityAction onUpdate;

    [OnValueChanged("UpdateTimeToSeconds")]
    public Times globalTime;

    [InfoBox("Time Flow = 1f / timeFlow")]
    public float timeFlow = 12f;

    [SerializeField] private Times updateRealTime;
    [SerializeField] private Times updateCicleGameTime;

    [SerializeField] private TimeOfDayController controller;

    [SerializeField] private bool showTime = false;

    public bool IsTimeStopped { get; set; }

    private int frequenceTimeSeconds;
    private int frequenceCycleSeconds;

    [ReadOnly] public Times.TimesState lastState = Times.TimesState.Noon;

    private Coroutine timeCoroutine = null;
    public bool IsTimeProccess => timeCoroutine != null;

    private WaitForSeconds seconds;

    [SerializeField] private List<TimeUnityEvent> events = new List<TimeUnityEvent>();

    private void OnEnable()
    {
        StartTime();
    }

    public void StartTime()
    {
        if (!IsTimeProccess)
        {
            frequenceTimeSeconds = updateRealTime.TotalSeconds;
            seconds = new WaitForSeconds(frequenceTimeSeconds * (1f / timeFlow));
            frequenceCycleSeconds = updateCicleGameTime.TotalSeconds;
            timeCoroutine = StartCoroutine(TimeRoutine());
        }
    }
    private IEnumerator TimeRoutine()
    {
        while (true)
        {
            globalTime.TotalSeconds += frequenceTimeSeconds;

            yield return seconds;

            if(globalTime.TotalSeconds % frequenceCycleSeconds == 0)
                UpdateCycle();

            UpdateActions();
            
            while (IsTimeStopped)
                yield return null;
        }

        BreakTime();
    }
    private void Update()
    {
        onUpdate.Invoke();
    }
    public void BreakTime()
    {
        if (IsTimeProccess)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }
    }

    public void SetTime(int secs)
    {
        globalTime.TotalSeconds = secs;

        UpdateCycle();
    }
    public void ChangeTimeOn(int secs)
    {
        int diff = secs - globalTime.TotalSeconds;

        for (int i = 0; i < diff; i++)
        {
            globalTime.TotalSeconds += 1;

            UpdateActions();
        }

        UpdateCycle();
    }

    public void AddEvent(TimeUnityEvent unityEvent)
    {
        events.Add(unityEvent);
    }

    #region Private
    private void UpdateTimeToSeconds()
    {
        globalTime.CheckTimeSeconds();
        UpdateCycle();
    }
    [Button]
    private void ResetTime()
    {
        globalTime.Reset();
        UpdateCycle();
    }
    [Button]
    private void RandomTime()
    {
        globalTime.RandomTime();
        UpdateCycle();
    }

    private void UpdateCycle()
    {
        controller.skyTime = globalTime.GetSkyPercent();
        controller.UpdateLunum(globalTime.State);
    }

    private void UpdateActions()
    {
        onUpdate.Invoke();

        onSecond.Invoke();

        if (globalTime.TotalSeconds % 60 == 0)
        {
            onMinute?.Invoke();
        }
        if (globalTime.TotalSeconds % 3600 == 0)
        {
            onHour?.Invoke();
        }
        if (globalTime.TotalSeconds % 86400 == 0)
        {
            onDay?.Invoke();
        }

        Times.TimesState currentState = globalTime.State;
        if (currentState != lastState)
        {
            lastState = currentState;
            onState?.Invoke();
        }

        for (int i = 0; i < events.Count; i++)
        {
            TimeUnityEvent unityEvent = events[i];

            if (unityEvent.IsNeedDelete)
            {
                events.Remove(unityEvent);
            }
            else
            {
                unityEvent.CheckInvoke(globalTime);
            }
        }

        //actions?.Invoke();
        //actionsTimes?.Invoke(globalTime);

        //for (int i = 0; i < actionsByTime.Count; i++)
        //{
        //    ActionByTime timeAction = actionsByTime[i];

        //    timeAction.InvokeCallBack(globalTime);

        //    if (timeAction.Check(globalTime))
        //    {
        //        actionsByTime.Remove(timeAction);
        //    }
        //}
    }
    #endregion

    public override string ToString()
    {
        return globalTime.ToString();
    }

    private void OnGUI()
    {
        if(showTime)
            GUI.Box(new Rect(150, 0, 80, 20), globalTime.ToString());
    }

    [System.Serializable]
    public class TimeUnityEvent
    {
        [SerializeField] private UnityAction<Times> callback;

        [SerializeField] private EventType eventType = EventType.ExecuteEveryTime;

        [Space]
        [HideLabel]
        [SerializeField] private Times time;
        [SerializeField] private UnityEvent events;

        private UnityAction onEnd;

        private bool isNeedDelete = false;
        public bool IsNeedDelete => isNeedDelete;

        public void AddEvent(EventType eventType, Times time, UnityAction action = null, UnityAction<Times> callback = null, UnityAction onEnd = null)
        {
            this.eventType = eventType;

            this.time = time;

            if(action != null)
                events.AddListener(action);
            this.callback += callback;
            this.onEnd += onEnd;
        }

        public void CheckInvoke(Times globalTime)
        {
            if(eventType == EventType.ExecuteEveryTime)
            {
                if(globalTime.TotalSeconds % time.TotalSeconds == 0)
                {
                    Invoke(globalTime);
                }
            }
            else
            {
                if(globalTime > time)
                {
                    isNeedDelete = true;
                    onEnd?.Invoke();
                }
                else
                {
                    Invoke(globalTime);
                }
            }
        }

        public void AddTime(Times time)
        {
            this.time += time;
        }
        public void SetTime(Times time)//хз шо будет
        {
            this.time = time;
        }
        public void RemoveTime(Times time)//хз шо будет
        {
            this.time -= time;
        }


        private void Invoke(Times time)
        {
            Invoke();
            InvokeCallBack(time);
        }

        private void Invoke()
        {
            events?.Invoke();
        }
        private void InvokeCallBack(Times time)
        {
            callback?.Invoke(time);
        }

        public enum EventType
        {
            ExecuteEveryTime,//выполнять всегда в это время
            ExecuteInTime,//выполнять пока не наступит это время
        }
    }
}
public static class SkipExtensionsTime
{
    private static GeneralTime instance;

    private static UnityAction<float> onProgress;
    private static UnityAction<Times> onTimes;
    private static UnityAction onCompletely;
    private static UnityAction onStart;
    private static UnityAction onEnd;
    private static UnityAction onBreak;

    private static Times skipTime;
    private static bool isForGlobal = true;
    private static float waitSkipTimeReal = 10f;

    private static Coroutine skipCoroutine = null;
    public static bool IsSkipProccess(this GeneralTime generalTime) => skipCoroutine != null;


    /// <summary>
    /// globalTime + times = skip time
    /// </summary>
    /// <param name="times"></param>
    public static GeneralTime SkipSetup(this GeneralTime generalTime, UnityAction<Times> time = null, UnityAction<float> progress = null, UnityAction completely = null, UnityAction start = null, UnityAction end = null, UnityAction brek = null)
    {
        instance = generalTime;

        onTimes = time;
        onProgress = progress;
        onCompletely = completely;
        onStart = start;
        onEnd = end;
        onBreak = brek;

        return instance;
    }

    public static void StartSkip(this GeneralTime generalTime, Times times, float waitTime)
    {
        skipTime = times;
        waitSkipTimeReal = waitTime;
        isForGlobal = true;

        StartSkip(generalTime);
    }
    public static void StartSkip(this GeneralTime generalTime, Times times, AnimationCurve waitTime)
    {
        //skipTime = times;
        //waitSkipTimeReal = waitTime;
        //isForGlobal = true;

        //StartSkip(generalTime);
    }

    private static void StartSkip(this GeneralTime generalTime)
    {
        if (!IsSkipProccess(instance))
        {
            instance.IsTimeStopped = true;

            int start = instance.globalTime.TotalSeconds;
            int end;

            if (isForGlobal)
                end = start + skipTime.TotalSeconds;
            else
                end = skipTime.TotalSeconds;

            onStart?.Invoke();

            skipCoroutine = instance.StartCoroutine(SkipTime(start, end));
        }
    }

    private static IEnumerator SkipTime(int startTime, int endTime)
    {
        int secs = 0;

        float currentTime = Time.deltaTime;

        Times time = new Times();
        time.TotalSeconds = endTime;

        while (currentTime < waitSkipTimeReal)
        {
            float progress = currentTime / waitSkipTimeReal;

            secs = (int)Mathf.Lerp(startTime, endTime, progress);
            instance.ChangeTimeOn(secs);

            time.TotalSeconds = endTime -secs;

            onProgress?.Invoke(progress);
            onTimes?.Invoke(time);

            currentTime += Time.deltaTime;

            yield return null;
        }


        onCompletely?.Invoke();

        StopSkip();
    }
    public static void BreakSkipTime(this GeneralTime generalTime)
    {
        if (IsSkipProccess(instance))
        {
            onBreak?.Invoke();
            StopSkip();
        }
    }
    private static void StopSkip()
    {
        if (IsSkipProccess(instance))
        {
            instance.StopCoroutine(skipCoroutine);
            skipCoroutine = null;

            onEnd?.Invoke();
            instance.IsTimeStopped = false;
        }
    }
}