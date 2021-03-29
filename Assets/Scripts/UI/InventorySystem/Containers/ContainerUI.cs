using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
	public UnityAction<Item> onItemChoosen;

	[SerializeField] private ContainerGridUI grid;

	[HideInInspector] public Inventory currentInventory;

    private void Awake()
    {
        grid.onItemChoosen += ItemChoosen;
    }

    public void SubscribeInventory(Inventory inventory)
    {
		currentInventory = inventory;

		currentInventory.onCollectionChanged += grid.PutItemsList;
        grid.PutItemsList(currentInventory.items);
	}
	public void UnSubscribeInventory()
    {
        if (currentInventory != null)
        {
            currentInventory.onCollectionChanged -= grid.PutItemsList;
            currentInventory = null;
        }
    }

	private void ItemChoosen(Item item)
    {
		onItemChoosen?.Invoke(item);
	}
}