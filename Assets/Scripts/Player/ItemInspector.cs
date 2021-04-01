using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class ItemInspector : MonoBehaviour
{
	public UnityAction onItemTake;
	public UnityAction onItemLeave;

	[SerializeField] private Transform modelPlace;
	[SerializeField] private Camera cam;
	[Space]
	[SerializeField] private float rotationSpeedXPC = 10f;
	[SerializeField] private float rotationSpeedYPC = 10f;
	[SerializeField] private float rotationSpeedXMobile = 0.1f;
	[SerializeField] private float rotationSpeedYMobile = 0.1f;

	private InspectAnimationType inspectType = InspectAnimationType.WorldToLocal;
	
	private float thresholdDistacnce = 0.05f;
	private float thresholdAngle = 0.1f;

	//coroutine
	private List<Item> cashItemsToDelete = new List<Item>();

	public bool IsInspectItem { get; private set; }

	private Coroutine reviewCoroutine = null;
	public bool IsReviewProccess => reviewCoroutine != null;

	private Coroutine inspectCoutine = null;
	public bool IsInspectProccess => inspectCoutine != null;

	//cash
	private Inventory inventory;

	private Item item;
	private ItemObject itemObject;
	private Transform ItemTransform => itemObject.transform;

	private Transform oldParent;
	private Vector3 oldWorldPosition;
	private Quaternion oldWorldRotation;

    public void SetItem(ItemObject itemObject)
	{
        GeneralAvailability.Player.Lock();

        this.itemObject = itemObject;

        SetupItem(itemObject.item);

        onItemTake = FromWorldTake;
        onItemLeave = ToWorldLeave;

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

		if (instantiateModel)
			this.itemObject = Instantiate(this.item.ScriptableItem.model, modelPlace);

		if(inspect == InspectAnimationType.WorldToLocal)
        {
			oldParent = ItemTransform.parent;
			oldWorldPosition = ItemTransform.position;
			oldWorldRotation = ItemTransform.rotation;
		}
		else if(inspect == InspectAnimationType.OnlyLocal)
        {
			ItemTransform.localPosition = Vector3.zero;
			ItemTransform.localRotation = Quaternion.identity;
		}

		itemObject.ColliderEnable(false);
		OpenUI();
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

			while (IsInspectProccess)
			{
				yield return null;
			}
		}

		inventory.RemoveItems(cashItemsToDelete);
		cashItemsToDelete.Clear();
		CloseUI();
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
		yield return LerpItem(ItemTransform, modelPlace.position, modelPlace.rotation, 0.3f);//lerp item from world to local
		ItemTransform.SetParent(modelPlace);

		yield return InspectItem();

		ItemTransform.SetParent(oldParent);
		yield return LerpItem(ItemTransform, oldWorldPosition, oldWorldRotation, 0.3f);//lerp item back to world

		itemObject.ColliderEnable(true);
		Dispose();

		StopInspect();
	}

	/// <summary>
	/// Анимация подбора и сброса предмета.
	/// </summary>
	/// <param name="item">Трансформ предмета которого подбираем.</param>
	/// <param name="posTo">К какому вектору позиции стремится.</param>
	/// <param name="rotTo">К какому квантариону стремиться.</param>
	/// <param name="t"></param>
	/// <returns></returns>
	private IEnumerator LerpItem(Transform item, Vector3 posTo, Quaternion rotTo, float t = 0.2f)
	{
		while((posTo - item.position).magnitude >= thresholdDistacnce || Quaternion.Angle(item.rotation, rotTo) >= thresholdAngle)
		{
			item.position = Vector3.Lerp(item.position, posTo, t);
			item.rotation = Quaternion.Slerp(item.rotation, rotTo, t);
			yield return new WaitForFixedUpdate();
		}
		item.position = posTo;
		item.rotation = rotTo;
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


	private void OpenUI()
    {
		GeneralAvailability.Player.Lock();

		GeneralAvailability.InspectorWindow.SetInformation(item.ScriptableItem);
		GeneralAvailability.InspectorWindow.ShowWindow();
	}
	private void CloseUI()
    {
		GeneralAvailability.Player.UnLock();

		GeneralAvailability.InspectorWindow.HideWindow();
	}


	#region Take Leave
	public void ItemTake()
	{
		onItemTake?.Invoke();
	}
	public void ItemLeave()
	{
		onItemLeave?.Invoke();
	}

	private void FromWorldTake()
    {
		GeneralAvailability.PlayerInventory.AddItem(item.ScriptableItem);

		CloseUI();

		StopInspect();

		if (itemObject)
		{
			Destroy(itemObject.gameObject);
		}
		Dispose();
	}
	private void ToWorldLeave()
    {
		CloseUI();
		IsInspectItem = false;
	}
	private void FromInventoryTake()
	{
		GeneralAvailability.PlayerInventory.AddItem(item.ScriptableItem);

		StopInspect();

		cashItemsToDelete.Add(item);

        if (itemObject)
		{
			Destroy(itemObject.gameObject);
		}
		Dispose();
	}
	private void ToInventoryLeave()
	{
		StopInspect();

		if (itemObject)
		{
			Destroy(itemObject.gameObject);
		}
		Dispose();
	}
	#endregion


	private void Dispose()
    {
		itemObject = null;
		item = null;

		oldParent = null;
		oldWorldPosition = Vector3.zero;
		oldWorldRotation = Quaternion.identity;
	}
}
public enum InspectAnimationType
{
	WorldToLocal,
	WorldToLocalNoAnim,
	OnlyLocal,
}