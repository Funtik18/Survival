using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
	public UnityAction<ContainerSlotUI> onSlotChoosen;

	[SerializeField] private ContainerGridUI grid;

	[HideInInspector] public Inventory currentInventory;

    private void Awake()
    {
        grid.onSlotChoosen += SlotChoosen;
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

    public void RefreshContainer()
    {
        grid.RefreshScroll();
        RefreshSlots();
    }
    public void RefreshSlots()
    {
        grid.UnChooseLastSlot();
    }

    private void SlotChoosen(ContainerSlotUI slot)
    {
		onSlotChoosen?.Invoke(slot); 
	}
}