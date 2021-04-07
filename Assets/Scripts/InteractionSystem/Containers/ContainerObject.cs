using System.Collections;

using UnityEngine;

public class ContainerObject : WorldObject
{
    public ContainerScriptableData scriptableData;

	[SerializeField] private Inventory containerInventory;

	public bool isInspected = false;
	public bool saveTimeResult = false;

    private void Awake()
    {
		containerInventory.Init();//изменить
	}

    public override void StartObserve()
	{
		base.StartObserve();

        if (isInspected)
            SetButtonOnInteraction();
        else
            SetButtonOnSearch();

        InteractionButton.OpenButton();

        GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.name).ShowToolTip();
    }
    public override void EndObserve()
    {
        base.EndObserve();

        InteractionButton.CloseButton();

        InteractionButton.pointer.Clear();
    }

    private void OpenContainer()
    {
        GeneralAvailability.BackpackWindow.secondaryContainer.SubscribeInventory(containerInventory);
        GeneralAvailability.BackpackWindow.ShowBackpackWithContainer();
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

        PointerHold holder = InteractionButton.pointer;
        holder.SetupHold(scriptableData.time);
        holder.AddHoldStartListener(StartHold);
        holder.AddHoldChangeListener(ChangeHold);
        holder.AddHoldBreakListener(BreakHold);
        holder.AddHoldStopListener(StopHold);
    }

    private void StartHold()
    {
        GeneralAvailability.Player.LockMovement();
        GeneralAvailability.Loader.ShowLoader();

    }
    private void ChangeHold(float value)
    {
        GeneralAvailability.Loader.LoaderFillAmount = value;
    }
    private void BreakHold(float time)
    {
        GeneralAvailability.Loader.HideLoader();
        Player.Instance.UnLockMovement();

        InteractionButton.pointer.SetupHold(scriptableData.time);
    }
    private void StopHold()
    {
        isInspected = true;

        GeneralAvailability.Loader.HideLoader();
        Player.Instance.UnLockMovement();

        Interact();
    }
}