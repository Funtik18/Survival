using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public UnityAction onUpdated;

    public UnityAction<ContainerSlotUI> onSlotChoosen;

    [SerializeField] private ContainerGridUI grid;
    [SerializeField] private TMPro.TextMeshProUGUI weightText;

    public Inventory Inventory { get; private set; }

    public ContainerUI Setup()
    {
        grid.Setup(this).onSlotChoosen += SlotChoosen;

        return this;
    }

    public void SubscribeInventory(Inventory inventory)
    {
        Inventory = inventory;

        Inventory.onCollectionChanged = UpdateGrid;
        UpdateGrid(Inventory.CurrentItems);
    }
    public void UnSubscribeInventory()
    {
        if (Inventory != null)
        {
            Inventory.onCollectionChanged = null;
            Inventory = null;
        }
    }

    public void RefreshContainer()
    {
        grid.RefreshScroll();
        grid.UnChooseLastSlot();
        UpdateWeight();
    }
    public void UpdateWeight()
    {
        weightText.text = Inventory.CurrentStringWeight;
    }

    private void UpdateGrid(List<Item> items)
    {
        grid.PutItemsList(items);

        UpdateWeight();

        onUpdated?.Invoke();
    }
    private void SlotChoosen(ContainerSlotUI slot)
    {
		onSlotChoosen?.Invoke(slot); 
	}
}