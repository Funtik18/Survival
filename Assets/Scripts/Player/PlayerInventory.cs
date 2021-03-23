using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerInventory : MonoBehaviour
{
    [ListDrawerSettings(ShowIndexLabels = true, NumberOfItemsPerPage = 20)]
    public List<ItemScriptableData> items = new List<ItemScriptableData>();

    [SerializeField] private InventoryGrid grid;
	[SerializeField] private InventoryItemInspector inspector;

	private void Awake()
	{
		grid.onChoosenItem += inspector.SetItem;

		for(int i = 0; i < items.Count; i++)
		{
			grid.PutItem(items[i]);
		}
	}

	public void AddItem(ItemScriptableData item)
	{
		items.Add(item);
		grid.PutItem(item);
	}
	public void RemoveItem(ItemScriptableData item)
	{

	}
}