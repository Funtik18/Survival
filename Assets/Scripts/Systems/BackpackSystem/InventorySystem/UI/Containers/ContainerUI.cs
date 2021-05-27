using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public UnityAction onUpdated;

    public UnityAction<ContainerSlotUI> onSlotChoosen;

    [SerializeField] private ContainerGridUI grid;
    [SerializeField] private TMPro.TextMeshProUGUI weightText;


    [HideInInspector] public Inventory currentInventory;

    public void Setup()
    {
        grid.Setup(this).onSlotChoosen += SlotChoosen;
    }

    public void SubscribeInventory(Inventory inventory)
    {
        currentInventory = inventory;

        currentInventory.onCollectionChanged = UpdateGrid;
        UpdateGrid(currentInventory.CurrentItems);
    }
    public void UnSubscribeInventory()
    {
        if (currentInventory != null)
        {
            currentInventory.onCollectionChanged = null;
            currentInventory = null;
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
        weightText.text = currentInventory.CurrentStringWeight;
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