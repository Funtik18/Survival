using System.Collections;

using UnityEngine;

public class ContainerObject : WorldObject
{
    public ContainerSD scriptableData;

    public Inventory containerInventory;

    private Coroutine holdCoroutine = null;
    public bool IsHoldProccess => holdCoroutine != null;

    private float endTime = 4f;
    private float currentTime;

    private bool isFirstTime = true;
    private bool isInspected = false;

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
        Debug.LogError("Open");
        GeneralAvailability.PlayerUI.OpenInventoryWithContainer(containerInventory);
        Statistics.TotalContainersOpened++;
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

            Overseer.Instance.Subscribe(this);
            Statistics.TotalContainersInspected++;
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


    public void SetData(Data data)
    {
        containerInventory.SetData(data.data).Init();
        isInspected = true;
        isFirstTime = false;
    }

    public Data GetData()
    {
        Data data = new Data()
        {
            index = Overseer.Instance.IndexOfContainer(this),
            data = containerInventory.GetData(),
        };
        return data;
    }

    [System.Serializable]
    public class Data 
    {
        public int index;
        public Inventory.Data data;
    }
}