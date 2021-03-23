using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ItemInspector : MonoBehaviour
{
	private static ItemInspector instance;
	public static ItemInspector Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<ItemInspector>();
			}
			return instance;
		}
	}


	[SerializeField] private WindowItemInspector inspector;
	[SerializeField] private Transform modelPlace;
	[SerializeField] private Camera cam;
	[Space]
	[SerializeField] private float rotationSpeedX = 10f;
	[SerializeField] private float rotationSpeedY = 10f;

	private InspectAnimationType inspectType = InspectAnimationType.WorldToLocal;
	
	private float thresholdDistacnce = 0.05f;
	private float thresholdAngle = 0.1f;

	//coroutine
	private Coroutine inspectCoutine = null;
	public bool IsInspectProccess => inspectCoutine != null;

	private bool isInspect = false;
	private bool isItemNeedDestroy = false;

	//cash
	private ItemObject currentItem;
	private Transform itemTransform;

	private Transform oldParent;
	private Vector3 oldWorldPosition;
	private Quaternion oldWorldRotation;

	private void Awake()
	{
		inspector.onTakeIt += ItemTake;
		inspector.onLeaveIt += ItemLeave;
	}

	public void SetItem(ItemObject item, InspectAnimationType inspectType)
	{
		if (item == null) { Debug.LogError("Error"); return; }

		item.ColliderEnable(false);

		Player.Instance.Lock();


		this.inspectType = inspectType;

		if(inspectType == InspectAnimationType.WorldToLocal)
		{
			currentItem = item;
			itemTransform = currentItem.transform;

			oldParent = itemTransform.parent;
			oldWorldPosition = itemTransform.position;
			oldWorldRotation = itemTransform.rotation;
		}
		//else if(inspectType == InspectAnimationType.OnlyLocal)
		//{
		//	currentItem = Instantiate(item, modelPlace);
		//	itemTransform = currentItem.transform;

		//	itemTransform.localPosition = Vector3.zero;
		//	itemTransform.localRotation = Quaternion.identity;
		//}

		StartInspect();
	}
	public void StartInspect()
	{
		if(!IsInspectProccess)
		{
			isInspect = true;
			isItemNeedDestroy = false;
			inspectCoutine = StartCoroutine(Inspect());
		}
	}

	private IEnumerator Inspect()
	{
		//if(currentItem.itemAngle == ItemInspectorAngle.World)
		//{
		//	yield return LerpItem(itemTransform, modelPlace.position, itemTransform.rotation);
		//}
		//else
		{
			yield return LerpItem(itemTransform, modelPlace.position, modelPlace.rotation);//lerp item from world to local
		}
		itemTransform.SetParent(modelPlace);

		inspector.SetInformation(currentItem.scriptableData.data);
		inspector.ShowWindow();

		yield return InspectItem();

		inspector.HideWindow();

		if(isItemNeedDestroy)
			Destroy(itemTransform.gameObject);
		else
		{
			yield return LerpItem(itemTransform, oldWorldPosition, oldWorldRotation);//lerp item back to world
			itemTransform.SetParent(oldParent);
		}

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
			yield return null;
		}
		item.position = posTo;
		item.rotation = rotTo;
	}

	/// <summary>
	/// Детальный осмотр предмета с поворотами по X и Y.
	/// </summary>
	/// <returns></returns>
	private IEnumerator InspectItem()
	{
		while(isInspect)
		{
			if(Input.GetMouseButton(0))
			{
				float rotX = Input.GetAxis("Mouse X") * rotationSpeedX;
				float rotY = Input.GetAxis("Mouse Y") * rotationSpeedY;

				Vector3 right = Vector3.Cross(cam.transform.up, itemTransform.position - cam.transform.position);
				Vector3 up = Vector3.Cross(itemTransform.position - cam.transform.position, right);
				itemTransform.rotation = Quaternion.AngleAxis(-rotX, up) * itemTransform.rotation;
				itemTransform.rotation = Quaternion.AngleAxis(rotY, right) * itemTransform.rotation;
			}

			yield return null;
		}
	}

	public void StopInspect()
	{
		if(IsInspectProccess)
		{
			StopCoroutine(inspectCoutine);
			inspectCoutine = null;

			Player.Instance.UnLock();

			currentItem.ColliderEnable(true);
			currentItem = null;
		}
	}


	private void ItemTake()
	{
		Player.Instance.AddItem(currentItem.scriptableData);

		isItemNeedDestroy = true;
		isInspect = false;
	}
	private void ItemLeave()
	{
		isItemNeedDestroy = false;
		isInspect = false;
	}
}
public enum InspectAnimationType
{
	WorldToLocal,
	WorldToLocalNoAnim,
	OnlyLocal,
}