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

		primaryContainer.Setup();
		secondaryContainer.Setup();

		primaryContainer.SubscribeInventory(inventory);

		primaryContainer.onSlotChoosen = itemInspector.SetItem;//события для осмотра предмета
		primaryContainer.onUpdated = UpdateInspector;
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


    #region Передача прдмета из одного контейнера в другой
    private Item itemShift;
	private ItemDataWrapper itemShiftData;
	private ContainerUI from, to;
	private void ItemShift(ContainerUI from, ContainerUI to, Item item)
	{
		if (item != null)
		{
			this.itemShift = item;
			itemShiftData = item.itemData;

			this.from = from;
			this.to = to;

            if (itemShiftData.IsInfinityWeight && itemShiftData.CurrentBaseWeight > 0.25f)
            {
				GeneralAvailability.PlayerUI.blockPanel.Enable(true);
				GeneralAvailability.PlayerUI.OpenExchander(0.25f, itemShiftData.CurrentBaseWeight, 0.25f, ok: ExchangerWeightkOk, all: ExchangerAll, cancel: ExchangerCancel);
			}
			else if (itemShiftData.CurrentStackSize > 1)
			{
				GeneralAvailability.PlayerUI.blockPanel.Enable(true);
				GeneralAvailability.PlayerUI.OpenExchander(1, itemShiftData.CurrentStackSize, 1, ok: ExchangerStackOk, all: ExchangerAll, cancel: ExchangerCancel);
            }
            else
            {
				ExchangerAll();
			}
		}
	}

	private void ExchangerCancel()
    {
		from.RefreshContainer();
	}
	private void ExchangerStackOk(float value)
    {
		from.RefreshContainer();

		ItemDataWrapper itemData = itemShiftData.Copy();
		itemData.CurrentStackSize = (int)value;
		to.currentInventory.AddItem(itemData);

		itemShiftData.CurrentStackSize -= (int)value;
		if(itemShiftData.IsStackEmpty)
			from.currentInventory.RemoveItem(itemShift);

		GeneralAvailability.PlayerUI.blockPanel.Enable(false);
	}
	private void ExchangerWeightkOk(float value)
	{
		from.RefreshContainer();

		ItemDataWrapper itemData = itemShiftData.Copy();
		itemData.CurrentBaseWeight = value;
		to.currentInventory.AddItem(itemData);

		itemShiftData.CurrentBaseWeight -= value;
		if (itemShiftData.IsWeightEmpty)
			from.currentInventory.RemoveItem(itemShift);

		GeneralAvailability.PlayerUI.blockPanel.Enable(false);
	}
	private void ExchangerAll()
    {
		from.RefreshContainer();

		to.currentInventory.AddItem(itemShift.itemData);
		from.currentInventory.RemoveItem(itemShift);

		GeneralAvailability.PlayerUI.blockPanel.Enable(false);
	}
    #endregion

    private void UpdateInspector()
    {
		itemInspector.SetItem(null);
	}
}