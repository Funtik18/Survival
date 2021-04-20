using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorObject : WorldObject
{
    public UnityAction onStartObserve;
    public UnityAction onInteract;

    [SerializeField] private bool isOpened = true;

    private Coroutine holdCoroutine = null;
    public bool IsHoldProccess => holdCoroutine != null;

    private float endTime = 2f;
    private float currentTime;

    public override void StartObserve()
    {
        base.StartObserve();

        InteractionButton.SetIconOnInteraction();
        InteractionButton.pointer.AddPressListener(StartHold);
        InteractionButton.pointer.AddUnPressListener(StopHold);
        InteractionButton.OpenButton();

        onStartObserve?.Invoke();
    }
    public override void EndObserve()
    {
        base.EndObserve();

        InteractionButton.CloseButton();
        InteractionButton.pointer.RemoveAllPressListeners();
        InteractionButton.pointer.RemoveAllUnPressListeners();
    }

    public override void Interact()
    {
        base.Interact();

        InteractionButton.pointer.RemoveAllListeners();

        onInteract?.Invoke();
    }


    public void StartHold()
    {
        if (!IsHoldProccess)
        {
            GeneralAvailability.Player.LockMovement();
            GeneralAvailability.TargetPoint.ShowLowBar();
            holdCoroutine = StartCoroutine(Hold(endTime));
        }
    }
    private IEnumerator Hold(float maxTime)
    {
        float startTime = Time.time;
        currentTime = Time.time - startTime;
        while (currentTime <= maxTime)
        {
            GeneralAvailability.TargetPoint.SetBarLowValue(currentTime / maxTime);

            currentTime = Time.time - startTime;

            yield return null;
        }

        Interact();
        yield return null;

        StopHold();
    }
    public void StopHold()
    {
        if (IsHoldProccess)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;

            GeneralAvailability.TargetPoint.HideLowBar();
            GeneralAvailability.Player.UnLockMovement();
        }
    }
}