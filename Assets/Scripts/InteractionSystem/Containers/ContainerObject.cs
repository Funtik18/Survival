
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ContainerObject : WorldObject
{
    public ContainerScriptableData scriptableData;

	[SerializeField] private List<ItemScriptableData> items = new List<ItemScriptableData>();

	public bool saveTimeResult = false;

	private ItemInspector inspector;
	public ItemInspector Inspector
	{
		get
		{
			if (inspector == null)
				inspector = ItemInspector.Instance;
			return inspector;
		}
	}

	private HoldLoader loader;
	private HoldLoader Loader
    {
        get
        {
			if(loader == null)
				loader = ControlUI.targetPoint.holdLoader;
			return loader;
        }
    }


	private Coroutine holdCoroutine = null;
	private bool IsHoldProccess => holdCoroutine != null;

	public override void StartObserve()
	{
		base.StartObserve();
		ControlUI.buttonSearch.onPressed.AddListener(StartHold);
		ControlUI.buttonSearch.onUnPressed.AddListener(StopHold);
		ControlUI.buttonSearch.IsEnable = true;
		ControlUI.targetPoint.SetToolTipText(scriptableData.data.name).ShowToolTip();
	}
    public override void EndObserve()
    {
        base.EndObserve();
		ControlUI.buttonSearch.IsEnable = false;
		ControlUI.buttonSearch.onPressed.RemoveListener(StartHold);
		ControlUI.buttonSearch.onUnPressed.RemoveListener(StopHold);
	}

	private void StartHold()
    {
        if (!IsHoldProccess)
        {
			Player.Instance.LockMovement();

			holdCoroutine = StartCoroutine(Hold());
		}
    }
	private IEnumerator Hold()
    {
		Loader.ShowLoader();

		float startTime = Time.time;
		float maxTime = scriptableData.data.time;
		float currentTime = Time.time - startTime;
		while (currentTime <= maxTime)
        {
			Loader.LoaderFillAmount = currentTime / maxTime;

			currentTime = Time.time - startTime;

			yield return null;
		}

		ControlUI.buttonSearch.UnPressButton();

		Interact();

		StopHold();
	}
	private void StopHold()
    {
        if (IsHoldProccess)
        {
			StopCoroutine(holdCoroutine);
            holdCoroutine = null;

			Loader.HideLoader();
		
			Player.Instance.UnLockMovement();
		}
	}

	public override void Interact()
	{
		Inspector.SetItem(items[Random.Range(0, items.Count)]);
	}
}