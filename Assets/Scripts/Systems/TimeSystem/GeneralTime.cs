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
    public UnityAction onHour;
    public UnityAction onMinute;
    public UnityAction onSecond;

    [OnValueChanged("UpdateTimeToSeconds")]
    public Times globalTime;

    [InfoBox("Time Flow = 1f / timeFlow")]
    public float timeFlow = 12f;

    [SerializeField] private Times updateRealTime;
    [SerializeField] private Times updateCicleGameTime;

    [SerializeField] private TimeOfDayController controller;


    [SerializeField] private bool showTime = false;

    public bool IsStopped { get; set; }

    private int frequenceTimeSeconds;
    private int frequenceCycleSeconds;

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
            
            while (IsStopped)
                yield return null;
        }

        BreakTime();
    }
    public void BreakTime()
    {
        if (IsTimeProccess)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }
    }

    public void ChangeTimeOn(int secs)
    {
        globalTime.TotalSeconds = secs;

        UpdateCycle();
        UpdateActions();
    }

    public void AddEvent(TimeUnityEvent unityEvent)
    {
        events.Add(unityEvent);
    }

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
        onSecond?.Invoke();

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