using UnityEngine;

public class InventorySystemUI : BackpackWindow
{
	[SerializeField] private ContainerUI primaryContainer;
	[SerializeField] private ContainerUI secondaryContainer;
	[SerializeField] private InventoryItemInspectorUI itemInspector;

    public override void Setup(PlayerInventory inventory)
    {
        base.Setup(inventory);

		PlayerOpportunities opportunities = GeneralAvailability.Player.Status.opportunities;

		itemInspector.onUse += opportunities.UseItem;
		itemInspector.onActions += opportunities.ActionsItem;
		itemInspector.onDrop += opportunities.DropItem;

		primaryContainer.SubscribeInventory(inventory);

		primaryContainer.onSlotChoosen = itemInspector.SetItem;//события для осмотра предмета
	}


	/// <summary>
	/// Открытие инвенторя с инспектором
	/// </summary>
	public override void OpenWindow()
    {
		OpenWindow(true);
    }

    public void OpenWindow(bool isInspector = true)
    {
        if (isInspector)
        {
			OpenItemInspector();
        }
        else
        {
			OpenSecondaryContainer();
		}

        base.OpenWindow();
    }

	public void SetSecondaryContainer(Inventory inventory)
	{
		secondaryContainer.SubscribeInventory(inventory);
	}

	public override void CloseWindow()
	{
		base.CloseWindow();

		itemInspector.SetItem(null);

		primaryContainer.RefreshContainer();
	}

	/// <summary>
	/// Открытие инвенторя с инспектором
	/// </summary>
	private void OpenItemInspector()
	{

		secondaryContainer.gameObject.SetActive(false);
		itemInspector.gameObject.SetActive(true);
	}
	/// <summary>
	/// Открытие инвенторя с доп контейнером
	/// </summary>
	private void OpenSecondaryContainer()
	{
		primaryContainer.onSlotChoosen = (x) => ItemShift(primaryContainer, secondaryContainer, x.item);
		secondaryContainer.onSlotChoosen = (x) => ItemShift(secondaryContainer, primaryContainer, x.item);

		secondaryContainer.gameObject.SetActive(true);
		itemInspector.gameObject.SetActive(false);
	}

	private void ItemShift(ContainerUI from, ContainerUI to, Item item)
	{
		if (item != null)
		{
			to.currentInventory.AddItem(item.itemData);
			from.currentInventory.RemoveItem(item);
		}
	}
}