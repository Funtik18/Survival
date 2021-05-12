using UnityEngine;

public class InventorySystemUI : BackpackWindow
{
	[SerializeField] private ContainerUI primaryContainer;
	[SerializeField] private ContainerUI secondaryContainer;
	[SerializeField] private InventoryItemInspectorUI itemInspector;

	private PlayerOpportunities opportunities;

    public override void Setup(PlayerInventory inventory)
    {
        base.Setup(inventory);

		itemInspector.onUse += UseItem;
		itemInspector.onActions += ActionsWith;
		itemInspector.onDrop += DropItem;

		opportunities = GeneralAvailability.Player.Status.opportunities;

		primaryContainer.SubscribeInventory(inventory);
	}

	public void OpenItemInspector()
	{
		primaryContainer.onSlotChoosen = itemInspector.SetItem;//события для осмотра предмета

		secondaryContainer.gameObject.SetActive(false);
		itemInspector.gameObject.SetActive(true);
	}
	public void OpenSecondaryContainer()
	{
		primaryContainer.onSlotChoosen = (x) => ItemShift(primaryContainer, secondaryContainer, x.item);
		secondaryContainer.onSlotChoosen = (x) => ItemShift(secondaryContainer, primaryContainer, x.item);

		secondaryContainer.gameObject.SetActive(true);
		itemInspector.gameObject.SetActive(false);
	}

	public void SetSecondaryContainer(Inventory inventory)
	{
		secondaryContainer.SubscribeInventory(inventory);
	}

    public override void CloseWindow()
    {
		base.CloseWindow();

		if (itemInspector.gameObject.activeSelf)
		{
			primaryContainer.RefreshContainer();
		}
		else if (secondaryContainer.gameObject.activeSelf)
		{
			secondaryContainer.UnSubscribeInventory();
		}

		itemInspector.SetItem(null);

		primaryContainer.RefreshContainer();
	}


    private static void ItemShift(ContainerUI from, ContainerUI to, Item item)
	{
		if (item != null)
		{
			to.currentInventory.AddItem(item.itemData);
			from.currentInventory.RemoveItem(item);
		}
	}

	private void UseItem(Item item)
	{
		opportunities.UseItem(item);
	}
	private void ActionsWith(Item item)
	{

	}
	private void DropItem(Item item)
	{
		opportunities.DropItem(item);
	}
}