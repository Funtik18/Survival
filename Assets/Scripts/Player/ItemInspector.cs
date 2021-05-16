using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class ItemInspector : MonoBehaviour
{
	[SerializeField] private Transform modelPlace;
	[SerializeField] private Camera cam;
	[Space]
	[SerializeField] private float rotationSpeedXPC = 10f;
	[SerializeField] private float rotationSpeedYPC = 10f;
	[SerializeField] private float rotationSpeedXMobile = 0.1f;
	[SerializeField] private float rotationSpeedYMobile = 0.1f;

	private InspectAnimationType inspectType = InspectAnimationType.WorldToLocal;
	
	private float thresholdDistacnce = 0.1f;
	private float thresholdAngle = 0.1f;

	#region Coroutine
    private List<Item> cashItemsToDelete = new List<Item>();

	public bool IsInspectItem { get; private set; }

	private Coroutine reviewCoroutine = null;
	public bool IsReviewProccess => reviewCoroutine != null;

	private Coroutine inspectCoutine = null;
	public bool IsInspectProccess => inspectCoutine != null;
    #endregion

    //cash
    private ObjectPool pool;
	private ObjectPool Pool
    {
        get
        {
			if(pool == null)
            {
				pool = ObjectPool.Instance;
			}
			return pool;
        }
    }

	private Inventory inventory;

	private Item item;
	private ItemObject itemObject;
	private Transform ItemTransform => itemObject.transform;

	private Transform oldParent;
	private Vector3 oldWorldPosition;
	private Quaternion oldWorldRotation;

    private void Awake()
    {
		WindowItemInspector windowItemInspector = GeneralAvailability.PlayerUI.windowsUI.itemInspectorWindow;

		windowItemInspector.onLeaveIt = ItemLeave;
		windowItemInspector.onAction = ItemAction;
		windowItemInspector.onTakeIt = ItemTake;
	}


    public void SetItem(ItemObject itemObject)
	{
        this.itemObject = itemObject;

        SetupItem(itemObject.Item);

        onItemTake = FromWorldTake;
        onItemLeave = ToWorldLeave;

		GeneralAvailability.PlayerUI.OpenItemInspector(itemObject);

		StartInspect();
    }
	public void ItemsReview(Inventory from)
	{
		this.inventory = from;

		onItemTake = FromInventoryTake;
		onItemLeave = ToInventoryLeave;

		StartReview();
	}

	private void SetupItem(Item item, bool instantiateModel = false, InspectAnimationType inspect = InspectAnimationType.WorldToLocal)
    {
		this.item = item;

		ItemSD sd = this.item.itemData.scriptableData;

		if (instantiateModel)
        {
			this.itemObject = Instantiate(sd.model, modelPlace);
		}

		if (inspect == InspectAnimationType.WorldToLocal)
        {
			oldParent = ItemTransform.parent;
			oldWorldPosition = ItemTransform.position;
			oldWorldRotation = ItemTransform.rotation;
		}
		else if(inspect == InspectAnimationType.OnlyLocal)
        {
			ItemTransform.position = sd.orientation.position;
			ItemTransform.rotation = sd.orientation.rotation;
		}

		itemObject.ColliderEnable(false);
	}

	#region Review
	private void StartReview()
    {
        if (!IsReviewProccess)
        {
			reviewCoroutine = StartCoroutine(Review());
		}
    }
	private IEnumerator Review()
    {
		List<Item> items = inventory.items;

        foreach (var item in items)
        {
			SetupItem(item, true, InspectAnimationType.OnlyLocal);
			
			StartInspect();

			GeneralAvailability.PlayerUI.OpenItemInspector(item);


			while (IsInspectProccess)
			{
				yield return null;
			}
		}

		inventory.RemoveItems(cashItemsToDelete);
		cashItemsToDelete.Clear();

		StopReview();
	}
	private void StopReview()
    {
        if (IsReviewProccess)
        {
			StopCoroutine(reviewCoroutine);
			reviewCoroutine = null;
		}
    }
	#endregion

	#region Inspect
	private void StartInspect()
	{
		if(!IsInspectProccess)
		{
			inspectCoutine = StartCoroutine(Inspect());
		}
	}
	private IEnumerator Inspect()
	{
		ItemTransform.SetParent(modelPlace);
		yield return LerpItem(ItemTransform, item.itemData.scriptableData.orientation.position, item.itemData.scriptableData.orientation.rotation, 0.25f, true);//lerp item from world to local

		yield return InspectItem();

		ItemTransform.SetParent(oldParent);
		yield return LerpItem(ItemTransform, oldWorldPosition, oldWorldRotation, 0.3f);//lerp item back to world

		
		Dispose();

		StopInspect();
	}

	/// <summary>
	/// Анимация подбора и сброса предмета.
	/// </summary>
	/// <param name="item">Трансформ предмета которого подбираем.</param>
	/// <param name="endPosition">К какому вектору позиции стремится.</param>
	/// <param name="endRotation">К какому квантариону стремиться.</param>
	/// <param name="duration"></param>
	/// <returns></returns>
	private IEnumerator LerpItem(Transform item, Vector3 endPosition, Quaternion endRotation, float duration = 0.2f, bool isLocal = false)
	{
		Vector3 startPosition;
		Quaternion startRotation;

		if (isLocal)
        {
			startPosition = item.localPosition;
			startRotation = item.localRotation;

			float time = 0;

			while (time < duration)
			{
				float normalStep = time / duration;

				item.localPosition = Vector3.Lerp(startPosition, endPosition, normalStep);
				item.localRotation = Quaternion.Lerp(startRotation, endRotation, normalStep);

				time += Time.deltaTime;

				yield return null;
			}
			item.localPosition = endPosition;
			item.localRotation = endRotation;
		}
        else
        {
			startPosition = item.position;
			startRotation = item.rotation;

			float time = 0;

			while (time < duration)
			{
				float normalStep = time / duration;

				item.position = Vector3.Lerp(startPosition, endPosition, normalStep);
				item.rotation = Quaternion.Lerp(startRotation, endRotation, normalStep);

				time += Time.deltaTime;

				yield return null;
			}
			item.position = endPosition;
			item.rotation = endRotation;
		}
	}

	#region InspectItem
	/// <summary>
	/// Берём X Y и записываем в RotateItem
	/// </summary>
	/// <returns></returns>
	private IEnumerator InspectItem()
    {
		IsInspectItem = true;

		if (GeneralSettings.IsPlatformMobile)
		{
			yield return InspectItemMobile();
		}
		else if (GeneralSettings.IsPlatformPC)
		{
			yield return InspectItemPC();
		}
	}
	private IEnumerator InspectItemPC()
	{
		while(IsInspectItem)
		{
			if(Input.GetMouseButton(0))
            {
                float rotX = Input.GetAxis("Mouse X") * rotationSpeedXPC;
                float rotY = Input.GetAxis("Mouse Y") * rotationSpeedYPC;

                RotateItem(rotX, rotY);
            }
            yield return null;
		}
	}
    private IEnumerator InspectItemMobile()
	{
		while (IsInspectItem)
		{
			if (Input.touchCount > 0)
			{
				float rotX = Input.touches[0].deltaPosition.x * rotationSpeedXMobile;
				float rotY = Input.touches[0].deltaPosition.y * rotationSpeedYMobile;

				RotateItem(rotX, rotY);
			}
			yield return null;
		}
	}
    #endregion

    private void StopInspect()
	{
		if(IsInspectProccess)
		{
			StopCoroutine(inspectCoutine);
			inspectCoutine = null;

			IsInspectItem = false;
		}
	}
    #endregion

    /// <summary>
    /// Детальный осмотр предмета с поворотами по X и Y.
    /// </summary>
    /// <returns></returns>
    private void RotateItem(float rotX, float rotY)
	{
		Vector3 right = Vector3.Cross(cam.transform.up, ItemTransform.position - cam.transform.position);
		Vector3 up = Vector3.Cross(ItemTransform.position - cam.transform.position, right);
		ItemTransform.rotation = Quaternion.AngleAxis(-rotX, up) * ItemTransform.rotation;
		ItemTransform.rotation = Quaternion.AngleAxis(rotY, right) * ItemTransform.rotation;
	}


	#region Take Leave
	private UnityAction onItemTake;
	private UnityAction onItemLeave;

	private void FromWorldTake()
    {
		GeneralAvailability.PlayerInventory.AddItem(item.itemData);

		StopInspect();

		if (itemObject)
		{
			if(itemObject is ItemObjectLiquidContainer liquidContainer)
            {
				GeneralAvailability.PlayerInventory.AddItem(liquidContainer.GetItem());
            }

			Pool.ReturnGameObject(itemObject.gameObject);
		}
		Dispose();
	}
	private void ToWorldLeave()
    {
		IsInspectItem = false;
	}
	private void FromInventoryTake()
	{
		GeneralAvailability.PlayerInventory.AddItem(item.itemData);

		StopInspect();

		cashItemsToDelete.Add(item);

        if (itemObject)
		{
			Pool.ReturnGameObject(itemObject.gameObject);
		}
		Dispose();
	}
	private void ToInventoryLeave()
	{
		StopInspect();

		if (itemObject)
		{
			Pool.ReturnGameObject(itemObject.gameObject);
		}
		Dispose();
	}
	#endregion


	private void Dispose()
    {
		itemObject.ColliderEnable(true);

		itemObject = null;
		item = null;

		oldParent = null;
		oldWorldPosition = Vector3.zero;
		oldWorldRotation = Quaternion.identity;
	}


	private void ItemTake()
	{
		onItemTake?.Invoke();

		GeneralAvailability.PlayerUI.CloseItemInspector();
	}
	private void ItemAction()
    {
		onItemLeave?.Invoke();
		
		itemObject.ActionItem();

		GeneralAvailability.PlayerUI.CloseItemInspector();
	}
	private void ItemLeave()
	{
		onItemLeave?.Invoke();

		GeneralAvailability.PlayerUI.CloseItemInspector();
	}
}
public enum InspectAnimationType
{
	WorldToLocal,
	WorldToLocalNoAnim,
	OnlyLocal,
}