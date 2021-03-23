using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemInspector : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI itemTittle;
	[SerializeField] private TMPro.TextMeshProUGUI itemDescription;

	[SerializeField] private ItemView3D view3D;

	//cash
	private ItemScriptableData currentItem;

	private void Awake()
	{
		SetItem(null);
	}

	public void SetItem(ItemScriptableData item)
	{
		currentItem = item;

		if(currentItem != null)
		{
			itemTittle.text = currentItem.data.name;
			itemDescription.text = currentItem.data.description;

			view3D.InstantiateModel(currentItem.data.model);
		}
		else
		{
			view3D.DisposePlace();


			itemTittle.text = "";
			itemDescription.text = "";
		}
	}
}