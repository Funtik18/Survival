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

    [OnValueChanged("UpdateTimeToSeconds")]
    public Times globalTime;


    [InfoBox("Time Flow = 1f / timeFlow")]
    public float timeFlow = 12f;

    public int frequenceCycleSeconds = 300;

    [SerializeField] private TimeOfDayController controller;

    public bool IsStopped { get; set; }

    private Coroutine timeCoroutine = null;
    public bool IsTimeProccess => timeCoroutine != null;

    private WaitForSeconds seconds;

    private List<TimeAction> actions = new List<TimeAction>();



    private void OnEnable()
    {
        StartTime();
    }

    public void StartTime()
    {
        if (!IsTimeProccess)
        {
            seconds = new WaitForSeconds(1f / timeFlow);
            timeCoroutine = StartCoroutine(TimeRoutine());
        }
    }
    private IEnumerator TimeRoutine()
    {
        while (true)
        {
            globalTime.TotalSeconds += 1;

            yield return seconds;

            if(globalTime.TotalSeconds % frequenceCycleSeconds == 0)
                UpdateCycle();

            for (int i = 0; i < actions.Count; i++)
            {
                TimeAction timeAction = actions[i];

                timeAction.InvokeCallBack(globalTime);

                if (timeAction.Check(globalTime))
                {
                    actions.Remove(timeAction);
                }
            }

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
    }

    /// <summary>
    /// Создать события по времени. Если пришло время то выполняется событие.
    /// </summary>
    public void AddActionByTime(UnityAction action, UnityAction<Times> callBack, Times futureTime)
    {
        if(globalTime < futureTime)
        {
            TimeAction timeAction = new TimeAction(action, callBack, futureTime);
            actions.Add(timeAction);
        }
        else
        {
            Debug.LogError("FUTURE TIME ERROR");
        }
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
        controller.skyTime = globalTime.GetDayPercent();
    }

    public override string ToString()
    {
        return globalTime.ToString();
    }

    public class TimeAction
    {
        private UnityAction action;
        private UnityAction<Times> callBack;
        private Times time;

        public TimeAction(UnityAction action, UnityAction<Times> callBack, Times time)
        {
            this.action = action;
            this.callBack = callBack;
            this.time = time;
        }

        public bool Check(Times times)
        {
            if (times >= time)
            {
                Invoke();
                return true;
            }
            return false;
        }
        public void Invoke()
        {
            action?.Invoke();
        }
        public void InvokeCallBack(Times time)
        {
            callBack?.Invoke(time);
        }
    }
}