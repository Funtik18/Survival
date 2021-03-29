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

		GeneralAvailability.ButtonSearch.IsEnable = true;

        if (isInspected)
        {
			GeneralAvailability.ButtonSearch.onClicked.AddListener(OpenContainer);
        }
        else
        {
			GeneralAvailability.ButtonSearch.onPressed.AddListener(StartHold);
			GeneralAvailability.ButtonSearch.onUnPressed.AddListener(StopHold);
        }
		GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.data.name).ShowToolTip();
    }
    public override void EndObserve()
    {
        base.EndObserve();
		GeneralAvailability.ButtonSearch.IsEnable = false;

        if (isInspected)
        {
			GeneralAvailability.ButtonSearch.onClicked.RemoveListener(OpenContainer);
        }
        else
        {
			GeneralAvailability.ButtonSearch.onPressed.RemoveListener(StartHold);
			GeneralAvailability.ButtonSearch.onUnPressed.RemoveListener(StopHold);
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

        GeneralAvailability.ButtonSearch.UnPressButton();

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