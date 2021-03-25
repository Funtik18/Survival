using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
	public UnityAction<Item> onItemChoosen;

	[SerializeField] private ContainerGridUI grid;

	public void Setup(Inventory inventory)
    {
        inventory.onCollectionChanged += grid.PutItemsList;
        grid.PutItemsList(inventory.items);

        grid.onItemChoosen += ItemChoosen;
	}

	private void ItemChoosen(Item item)
    {
		onItemChoosen?.Invoke(item);
	}
}