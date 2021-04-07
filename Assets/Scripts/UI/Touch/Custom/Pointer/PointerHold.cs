using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerHold : Pointer
{
    public UnityEvent onHoldStarted;
    public UnityAction<float> onHoldChanged;
    public UnityEvent onHoldCompletely;
    public UnityAction<float> onHoldBreaked;
    public UnityEvent onHoldStoped;

    private float endTime = 1f;
    private bool isSetupet = false;

    private bool isCompletely = false;
    
    private HoldType holdType = HoldType.WithLoader;
    private Coroutine holdCoroutine = null;
    public bool IsHoldProccess => holdCoroutine != null;

    private float currentTime;

    public void SetupHold(float endTime, float startTime = 0)
    {
        this.endTime = endTime;

        isSetupet = true;
    }

    public override void PressButton()
    {
        if(isSetupet && endTime > 0)
            StartHold();
        else
            base.PressButton();
    }
    public override void UnPressButton()
    {
        base.UnPressButton();

        StopHold();
    }

    public void StartHold()
    {
        if (!IsHoldProccess)
        {
            onHoldStarted?.Invoke();
            holdCoroutine = StartCoroutine(Hold(endTime));
        }
    }
    private IEnumerator Hold(float maxTime)
    {
        float startTime = Time.time;
        currentTime = Time.time - startTime;
        while (currentTime <= maxTime)
        {
            onHoldChanged?.Invoke(currentTime / maxTime);

            currentTime = Time.time - startTime;

            yield return null;
        }

        isCompletely = true;
        onHoldStoped?.Invoke();

        UnPressButton();
    }
    public void StopHold()
    {
        if (IsHoldProccess)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;

            isSetupet = false;
            if (isCompletely)
            {
                onHoldCompletely?.Invoke();
                isCompletely = false;
            }
            else
            {
                onHoldBreaked?.Invoke(currentTime);
            }
        }
    }

    public void AddHoldStartListener(UnityAction action)
    {
        onHoldStarted.AddListener(action);
    }
    public void AddHoldChangeListener(UnityAction<float> action)
    {
        onHoldChanged += action;
    }
    public void AddHoldCompletelyListener(UnityAction action)
    {
        onHoldCompletely.AddListener(action);
    }
    public void AddHoldBreakListener(UnityAction<float> action)
    {
        onHoldBreaked += (action);
    }
    public void AddHoldStopListener(UnityAction action)
    {
        onHoldStoped.AddListener(action);
    }

    public void RemoveAllHoldListeners()
    {
        onHoldStarted.RemoveAllListeners();
        onHoldChanged = null;
        onHoldCompletely.RemoveAllListeners();
        onHoldBreaked = null;
        onHoldStoped.RemoveAllListeners();
    }

    public void Clear()
    {
        endTime = 0;

        RemoveAllPressListeners();
        RemoveAllHoldListeners();
    }
}
public enum HoldType
{
    WithLoader,
}
