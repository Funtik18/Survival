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

    private void Awake()
    {
        grid.onSlotChoosen += SlotChoosen;
    }

    public void SubscribeInventory(Inventory inventory)
    {
        currentInventory = inventory;

        currentInventory.onCollectionChanged += UpdateGrid;
        UpdateGrid(currentInventory.items);
    }
    public void UnSubscribeInventory()
    {
        if (currentInventory != null)
        {
            currentInventory.onCollectionChanged -= UpdateGrid;
            currentInventory = null;
        }
    }

    public void RefreshContainer()
    {
        grid.RefreshScroll();
        grid.UnChooseLastSlot();
    }

    private void UpdateGrid(List<Item> items)
    {
        grid.PutItemsList(items);

        UpdateWeight();

        onUpdated?.Invoke();
    }

    private void UpdateWeight()
    {
        weightText.text = currentInventory.GetWeight() + " KG";
    }

    private void SlotChoosen(ContainerSlotUI slot)
    {
		onSlotChoosen?.Invoke(slot); 
	}
}