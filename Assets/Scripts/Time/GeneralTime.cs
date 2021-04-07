using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Funly.SkyStudio;
using Sirenix.OdinInspector;

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

    public Times globalTime;

    [InfoBox("Time Flow = 1f / timeFlow")]
    public float timeFlow = 12f;

    [SerializeField] private TimeOfDayController controller;

    public bool IsStopped { get; set; }

    private Coroutine timeCoroutine = null;
    public bool IsTimeProccess => timeCoroutine != null;

    private WaitForSeconds seconds;

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
            globalTime.seconds += 1;

            globalTime.CheckTime();

            yield return seconds;

            if(globalTime.seconds == 0)
                UpdateCycle();

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

    public void ChangeTimeOn(Times time)
    {
        globalTime = time;
        UpdateCycle();
    }
    public void ChangeTimeOn(int secs)
    {
        globalTime.SetTimeBySeconds(secs);
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
        globalTime.RandomHours();
        globalTime.RandomMinutes();
        globalTime.RandomSeconds();
        UpdateCycle();
    }
    [Button]
    private void UpdateCycle()
    {
        controller.skyTime = globalTime.GetDayPercent();
    }

    public override string ToString()
    {
        return globalTime.ToString();
    }
}