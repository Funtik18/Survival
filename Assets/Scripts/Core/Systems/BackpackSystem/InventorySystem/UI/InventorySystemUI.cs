using UnityEngine;
using UnityEngine.UI;

public class InventorySystemUI : BackpackWindow
{
	[SerializeField] private ContainerUI primaryContainer;
	[SerializeField] private ContainerUI secondaryContainer;
	[SerializeField] private InventoryItemInspectorUI itemInspector;
	[Space]
	[SerializeField] private TMPro.TextMeshProUGUI titleSort;
	[Space]
	[SerializeField] private Pointer buttonAll;
	[SerializeField] private Pointer buttonFire;
	[SerializeField] private Pointer buttonFirstAid;
	[SerializeField] private Pointer buttonCloth;
	[SerializeField] private Pointer buttonFood;
	[SerializeField] private Pointer buttonTools;
	[SerializeField] private Pointer buttonMaterials;
	[Space]
	[SerializeField] private Toggle toggleAZ;
	[SerializeField] private Toggle toggleWeight;

	private InventorySortGlobal currentGlobalSort = InventorySortGlobal.None;
	public InventorySortGlobal CurrentGlobalSort
    {
		get => currentGlobalSort;
        set
        {
			if(currentGlobalSort != value)
            {
				currentGlobalSort = value;
				inventory.SetSort(currentGlobalSort);
				primaryContainer.RefreshContainer();

				if (secondaryContainer.Inventory != null)
				{
					secondaryContainer.Inventory.SetSort(currentSort);
					secondaryContainer.RefreshContainer();
				}
			}
        }
    }
	private InventorySort currentSort = InventorySort.All;
	public InventorySort CurrentSort 
	{
		get => currentSort;
        set
        {
			if(currentSort != value)
            {
				currentSort = value;
				inventory.SetSort(currentSort);

				primaryContainer.RefreshContainer();

				if(secondaryContainer.Inventory != null)
                {
					secondaryContainer.Inventory.SetSort(currentSort);
					secondaryContainer.RefreshContainer();
				}
			}
        }
	}


	public override void Setup(PlayerInventory inventory)
    {
        base.Setup(inventory);

		primaryContainer.Setup().SubscribeInventory(inventory);
		secondaryContainer.Setup();

		//
		PlayerOpportunities opportunities = GeneralAvailability.Player.Status.opportunities;

		itemInspector.onUse += opportunities.UseItem;
		itemInspector.onActions += opportunities.ActionsItem;
		itemInspector.onDrop += opportunities.DropItem;

		//buttons
		buttonAll.AddPressListener(SortAll);
		buttonFire.AddPressListener(SortFire);
		buttonFirstAid.AddPressListener(SortFirstAid);
		buttonCloth.AddPressListener(SortCloth);
		buttonFood.AddPressListener(SortFood);
		buttonTools.AddPressListener(SortTools);
		buttonMaterials.AddPressListener(SortMaterials);

		toggleAZ.onValueChanged.AddListener(SortByName);
		toggleWeight.onValueChanged.AddListener(SortByWeight);
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
		primaryContainer.onSlotChoosen = itemInspector.SetItem;//события для осмотра предмета
		primaryContainer.onUpdated = UpdateInspector;

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
		to.Inventory.AddItem(itemData);

		itemShiftData.CurrentStackSize -= (int)value;
		if(itemShiftData.IsStackEmpty)
			from.Inventory.RemoveItem(itemShift);

		GeneralAvailability.PlayerUI.blockPanel.Enable(false);
	}
	private void ExchangerWeightkOk(float value)
	{
		from.RefreshContainer();

		ItemDataWrapper itemData = itemShiftData.Copy();
		itemData.CurrentBaseWeight = value;
		to.Inventory.AddItem(itemData);

		itemShiftData.CurrentBaseWeight -= value;
		if (itemShiftData.IsWeightEmpty)
			from.Inventory.RemoveItem(itemShift);

		GeneralAvailability.PlayerUI.blockPanel.Enable(false);
	}
	private void ExchangerAll()
    {
		from.RefreshContainer();

		to.Inventory.AddItem(itemShift.itemData);
		from.Inventory.RemoveItem(itemShift);

		GeneralAvailability.PlayerUI.blockPanel.Enable(false);
	}
    #endregion

    private void UpdateInspector()
    {
		itemInspector.SetItem(null);
	}


	private void SortAll()
    {
		CurrentSort = InventorySort.All;

		titleSort.text = "ALL";
	}
	private void SortFire()
    {
		CurrentSort = InventorySort.FireItems;

		titleSort.text = "Fire Starting";
	}
	private void SortFirstAid()
	{
		CurrentSort = InventorySort.FirstAidItems;

		titleSort.text = "First Aid";
	}
	private void SortCloth()
	{
		CurrentSort = InventorySort.ClothItems;

		titleSort.text = "Cloth";
	}
	private void SortFood()
	{
		CurrentSort = InventorySort.FoodItems;

		titleSort.text = "Food";
	}
	private void SortTools()
	{
		CurrentSort = InventorySort.ToolsItems;

		titleSort.text = "Tools";
	}
	private void SortMaterials()
	{
		CurrentSort = InventorySort.MaterialsItems;

		titleSort.text = "Materials";
	}

	private void SortByName(bool trigger)
    {
		if (trigger)
			CurrentGlobalSort = InventorySortGlobal.ByName;
	}
	private void SortByWeight(bool trigger)
	{
		if (trigger)
			CurrentGlobalSort = InventorySortGlobal.ByWeight;
	}
}