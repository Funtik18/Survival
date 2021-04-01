using System.Collections;

using UnityEngine;

public class ContainerObject : WorldObject
{
    public ContainerScriptableData scriptableData;

	[SerializeField] private Inventory containerInventory;

	public bool isInspected = false;
	public bool saveTimeResult = false;

	private Coroutine holdCoroutine = null;
	private bool IsHoldProccess => holdCoroutine != null;

    private void Awake()
    {
		containerInventory.Init();//изменить
	}

    public override void StartObserve()
	{
		base.StartObserve();

        if (isInspected)
        {
            Button.SetIconOnInteraction();
            Button.pointer.AddPressListener(OpenContainer);
        }
        else
        {
            Button.SetIconOnSearch();
            Button.pointer.AddPressListener(StartHold);
            Button.pointer.AddUnPressListener(StopHold);
        }

        Button.OpenButton();

        GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.data.name).ShowToolTip();
    }
    public override void EndObserve()
    {
        base.EndObserve();
        Button.CloseButton();

        if (isInspected)
        {
            Button.pointer.RemovePressListener(OpenContainer);
        }
        else
        {
            Button.pointer.RemovePressListener(StartHold);
            Button.pointer.RemoveUnPressListener(StopHold);
        }
    }

	private void StartHold()
    {
        if (!IsHoldProccess)
        {
			GeneralAvailability.Player.LockMovement();

			holdCoroutine = StartCoroutine(Hold());
		}
    }
	private IEnumerator Hold()
    {
        GeneralAvailability.Loader.ShowLoader();

        float startTime = Time.time;
        float maxTime = scriptableData.data.time;
        float currentTime = Time.time - startTime;
        while (currentTime <= maxTime)
        {
            GeneralAvailability.Loader.LoaderFillAmount = currentTime / maxTime;

            currentTime = Time.time - startTime;

            yield return null;
        }

        Button.pointer.UnPressButton();

        isInspected = true;

		Interact();

		StopHold();
	}
	private void StopHold()
    {
        if (IsHoldProccess)
        {
			StopCoroutine(holdCoroutine);
            holdCoroutine = null;

            GeneralAvailability.Loader.HideLoader();

            Player.Instance.UnLockMovement();
		}
	}

	private void OpenContainer()
    {
        GeneralAvailability.BackpackWindow.secondaryContainer.SubscribeInventory(containerInventory);
        GeneralAvailability.BackpackWindow.ShowBackpackWithContainer();
    }

	public override void Interact()
	{
        GeneralAvailability.Inspector.ItemsReview(containerInventory);
	}
}