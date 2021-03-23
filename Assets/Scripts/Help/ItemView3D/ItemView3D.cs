using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ItemView3D : MonoBehaviour
{
	[SerializeField] private Transform modelPlace;
	[SerializeField] private Camera cam;


	private Transform itemTransform;

	[SerializeField] private float rotationSpeedX = 10f;
	[SerializeField] private float rotationSpeedY = 10f;

	////coroutine
	//private Coroutine inspectCoutine = null;
	//public bool IsInspectProccess => inspectCoutine != null;

	//private bool isInspect = false;
	//private bool isItemNeedDestroy = false;

	public ItemObject InstantiateModel(ItemObject model)
	{
		DisposePlace();

		ItemObject itemModel = Instantiate(model, modelPlace);

		itemModel.ColliderEnable(false);

		itemTransform = itemModel.transform;

		itemTransform.localPosition = Vector3.zero;
		return itemModel;
	}

	public void DisposePlace()
	{
		for(int i = 0; i < modelPlace.childCount; i++)
		{
			Destroy(modelPlace.GetChild(i).gameObject);
		}
	}

	private void Update()
	{
		if(itemTransform)
		{
			float rotX = Input.GetAxis("Mouse X") * rotationSpeedX;
			float rotY = Input.GetAxis("Mouse Y") * rotationSpeedY;

			Vector3 right = Vector3.Cross(cam.transform.up, itemTransform.position - cam.transform.position);
			Vector3 up = Vector3.Cross(itemTransform.position - cam.transform.position, right);
			itemTransform.rotation = Quaternion.AngleAxis(-rotX, up) * itemTransform.rotation;
			itemTransform.rotation = Quaternion.AngleAxis(rotY, right) * itemTransform.rotation;
		}
	}


	//public void StartInspect()
	//{
	//	if(!IsInspectProccess)
	//	{
	//		isInspect = true;
	//		isItemNeedDestroy = false;
	//		inspectCoutine = StartCoroutine(Inspect());
	//	}
	//}


	//private IEnumerator Inspect()
	//{

	//	yield return InspectItem();

	//	StopInspect();
	//}


	///// <summary>
	///// Детальный осмотр предмета с поворотами по X и Y.
	///// </summary>
	///// <returns></returns>
	//private IEnumerator InspectItem()
	//{
	//	while(isInspect)
	//	{
	//		if(Input.GetMouseButton(0))
	//		{
	//			float rotX = Input.GetAxis("Mouse X") * rotationSpeedX;
	//			float rotY = Input.GetAxis("Mouse Y") * rotationSpeedY;

	//			//Vector3 right = Vector3.Cross(cameraUI.transform.up, itemTransform.position - cameraUI.transform.position);
	//			//Vector3 up = Vector3.Cross(itemTransform.position - cameraUI.transform.position, right);
	//			//itemTransform.rotation = Quaternion.AngleAxis(-rotX, up) * itemTransform.rotation;
	//			//itemTransform.rotation = Quaternion.AngleAxis(rotY, right) * itemTransform.rotation;
	//		}

	//		yield return null;
	//	}
	//}

	//public void StopInspect()
	//{
	//	if(IsInspectProccess)
	//	{
	//		StopCoroutine(inspectCoutine);
	//		inspectCoutine = null;

	//		currentItem = null;
	//	}
	//}
}