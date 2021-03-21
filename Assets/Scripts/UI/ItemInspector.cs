using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInspector : MonoBehaviour
{
    [SerializeField] private Camera cameraUI;

	[SerializeField] private float rotationSpeedX = 10f;
	[SerializeField] private float rotationSpeedY = 10f;

	private InspectAnimationType inspectType = InspectAnimationType.WorldToLocal;
	
	private float thresholdDistacnce = 0.05f;

	//coroutine
	private Coroutine inspectCoutine = null;
	public bool IsInspectProccess => inspectCoutine != null;

	private bool isInspect = false;
	private bool isItemNeedDestroy = false;

	//cash
	private Item currentItem;
	private Transform itemTransform;

	private Vector3 oldInspectPosition;

	private Transform oldParent;
	private Vector3 oldWorldPosition;
	private Quaternion oldWorldRotation;

	//properties
	private Transform trans;
	public Transform Transform
	{
		get
		{
			if(trans == null)
				trans = transform;
			return trans;
		}
	}

	private WindowItemInspector inspector;
	private WindowItemInspector Inspector
	{
		get
		{
			if(inspector == null)
			{
				inspector = Player.Instance.playerUI.itemInspector;
				inspector.onTakeIt += ItemTake;
				inspector.onLeaveIt += ItemLeave;
			}
			return inspector;
		}
	}


	public void SetItem(Item item, InspectAnimationType type = InspectAnimationType.WorldToLocal)
	{
		if(!item) return;

		item.ColliderEnable(false);

		inspectType = type;

		if(inspectType == InspectAnimationType.WorldToLocal)
		{
			currentItem = item;
			itemTransform = currentItem.transform;

			oldParent = itemTransform.parent;
			oldWorldPosition = itemTransform.position;
			oldWorldRotation = itemTransform.rotation;
		}
		else if(inspectType == InspectAnimationType.OnlyLocal)
		{
			currentItem = Instantiate(item, Transform);
			itemTransform = currentItem.transform;

			itemTransform.localPosition = Vector3.zero;
			itemTransform.localRotation = Quaternion.identity;
		}

		Player.Instance.Lock(true);

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
		yield return LerpItem(itemTransform, Transform.position, Transform.rotation);
		itemTransform.SetParent(Transform);

		Inspector.SetInformation(currentItem.data);
		Inspector.ShowWindow();

		yield return InspectItem();

		Inspector.HideWindow();

		if(isItemNeedDestroy)
			Destroy(itemTransform.gameObject);
		else
		{
			yield return LerpItem(itemTransform, oldWorldPosition, oldWorldRotation);
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
		while((posTo - item.position).magnitude >= thresholdDistacnce && (item.rotation.eulerAngles - rotTo.eulerAngles).magnitude >= thresholdDistacnce)
		{
			item.position = Vector3.Lerp(item.position, posTo, t);
			item.rotation = Quaternion.Lerp(item.rotation, rotTo, t);
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

				Vector3 right = Vector3.Cross(cameraUI.transform.up, itemTransform.position - cameraUI.transform.position);
				Vector3 up = Vector3.Cross(itemTransform.position - cameraUI.transform.position, right);
				itemTransform.rotation = Quaternion.AngleAxis(-rotX, up) * itemTransform.rotation;
				itemTransform.rotation = Quaternion.AngleAxis(rotY, right) * itemTransform.rotation;
			}

			oldInspectPosition = Input.mousePosition;
			yield return null;
		}
	}

	public void StopInspect()
	{
		if(IsInspectProccess)
		{
			StopCoroutine(inspectCoutine);
			inspectCoutine = null;
			
			Player.Instance.Lock(false);

			currentItem.ColliderEnable(true);
			currentItem = null;
		}
	}


	private void ItemTake()
	{
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