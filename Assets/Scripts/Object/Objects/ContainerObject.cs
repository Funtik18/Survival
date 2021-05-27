using System.Collections;

using UnityEngine;

public class ContainerObject : WorldObject
{
    public ContainerSD scriptableData;

    public Inventory containerInventory;

	public bool isInspected = false;
	//public bool saveTimeResult = false;

    private Coroutine holdCoroutine = null;
    public bool IsHoldProccess => holdCoroutine != null;

    private float endTime = 4f;
    private float currentTime;

    private bool isFirstTime = true;

    public override void StartObserve()
	{
		base.StartObserve();

        if (isInspected)
            SetButtonOnInteraction();
        else
            SetButtonOnSearch();

        InteractionButton.OpenButton();

        UpdateToolTip();
    }
    public override void EndObserve()
    {
        base.EndObserve();

        InteractionButton.CloseButton();

        InteractionButton.pointer.RemoveAllListeners();
    }

    private void OpenContainer()
    {
        GeneralAvailability.PlayerUI.OpenInventoryWithContainer(containerInventory);
    }

	public override void Interact()
	{
        if (containerInventory.IsEmpty)
            SetButtonOnInteraction();
        else
            GeneralAvailability.Inspector.ItemsReview(containerInventory);
    }

    private void SetButtonOnInteraction()
    {
        InteractionButton.SetIconOnInteraction();
        InteractionButton.pointer.AddPressListener(OpenContainer);
    }
    private void SetButtonOnSearch()
    {
        InteractionButton.SetIconOnSearch();

        InteractionButton.pointer.AddPressListener(StartHold);
        InteractionButton.pointer.AddUnPressListener(StopHold);
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

        InteractionButton.pointer.RemoveAllListeners();
        isInspected = true;

        if (isFirstTime)
        {
            containerInventory.SetData(ItemsData.Instance.Container).Init();
            isFirstTime = false;
        }

        UpdateToolTip();

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
            if (isInspected)
            {
                Interact();
            }
        }
    }



    private void UpdateToolTip()
    {
        if (isInspected && containerInventory.IsEmpty)
            GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.objectName + " - Empty").ShowToolTip();
        else
            GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.objectName).ShowToolTip();
    }
}