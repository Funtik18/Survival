using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Events;

public class InventoryGrid : MonoBehaviour
{
	public UnityAction<ItemScriptableData> onChoosenItem;

	[SerializeField] private ScrollRect scrollrect;
	[SerializeField] private Scrollbar scrollbar;
	[AssetList]
	[SerializeField] private InventorySlot slotPrefab;

	[ListDrawerSettings(ShowIndexLabels = true, NumberOfItemsPerPage = 5, DraggableItems = false)]
	public List<InventorySlot> slots = new List<InventorySlot>();

	public void UpdateScrollBarSteps()
	{
		int rows = Mathf.CeilToInt(transform.childCount / 5);
		scrollbar.numberOfSteps = (rows - 4) + 1;
	}

	public List<InventorySlot> GetSlots()
	{
		if(transform.childCount != slots.Count)
		{
			slots.Clear();
			for(int i = 0; i < transform.childCount; i++)
			{
				InventorySlot slot = transform.GetChild(i).GetComponent<InventorySlot>();
				slot.onClick = ChooseSlot;
				slots.Add(slot);
			}
		}
		return slots;
	}

	public void PutItem(ItemScriptableData item)
	{
		UpdateScrollBarSteps();
		GetSlots();

		for(int i = 0; i < slots.Count; i++)
		{
			if(slots[i].IsEmpty)
			{
				slots[i].SetItem(item);
				return;
			}
		}
		int lastSize = slots.Count;
		AddRow();
		slots[lastSize].SetItem(item);//check it
	}

	public void DisposeSlot()
	{

	}

	public void ChooseSlot(ItemScriptableData data)
	{
		onChoosenItem?.Invoke(data);
	}


	public void AddRow()
	{
		for(int i = 0; i < 5; i++)
		{
			slots.Add(Instantiate(slotPrefab, transform));
		}

		UpdateScrollBarSteps();
	}
	public void RemoveRow()
	{
		Debug.LogError(transform.childCount);

		List<GameObject> row = new List<GameObject>();

		for(int i = 0; i < 5; i++)//check
		{
			GameObject go = transform.GetChild(transform.childCount - 1).gameObject;
			slots.Remove(go.GetComponent<InventorySlot>());
			go.transform.SetParent(null);
			row.Add(go);
		}

		for(int i = 0; i < row.Count; i++)
		{
			Destroy(row[i]);
		}

		UpdateScrollBarSteps();
	}
}