using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;

public class WindowBackpack : WindowUI
{
	public UnityAction onBack;

	[SerializeField] private NavigationHeaderUI navigationHeader;
	[SerializeField] private InventorySystemUI inventorySystem;
	public InventorySystemUI InventorySystem => inventorySystem;
	[SerializeField] private CraftingSystemUI craftingSystem;

	[Space]
	[SerializeField] private List<BackpackWindow> windowsOrder = new List<BackpackWindow>();
	[SerializeField] private bool isLoop = false;
	[Min(0), MaxValue("MaxIndex")]
	[SerializeField] private int initIndex;

	[Space]
	[SerializeField] private Button buttonBack;

	private PlayerInventory inventory;
	private PlayerInventory Inventory
	{
		get
		{
			if (inventory == null)
			{
				inventory = GeneralAvailability.PlayerInventory;
			}
			return inventory;
		}
	}

	private int currentIndex = -1;
	public int CurrentIndex
	{
		get => currentIndex;
		set
		{
			int lastIndex = currentIndex;

			currentIndex = value;

			OpenWindow(currentIndex);

			if (lastIndex > currentIndex)//forward
			{
				UpdateNavigator(true);
			}
			else//backward
			{
				UpdateNavigator(false);
			}
		}
	}

	#region Editor
	private int MaxIndex => windowsOrder.Count - 1;
	#endregion

	private void Awake()
	{
		buttonBack.onClick.AddListener(Back);

		navigationHeader.Setup(windowsOrder.Count, Left, Right);

		for (int i = 0; i < windowsOrder.Count; i++)//all windows upd
        {
			windowsOrder[i].Setup(Inventory);
		}

		CurrentIndex = initIndex;

		Inventory.onBlueprintsUpdated += craftingSystem.UpdateUI;
	}


	public void ShowBackpack()
    {
		ShowWindowByIndex(windowsOrder.FindIndex((x) => x == inventorySystem));
		ShowWindow();
	}
	public void ShowBackpackWithContainer()
    {
		navigationHeader.Enable(false);
		CloseAll();
		inventorySystem.OpenWindow(false);
		ShowWindow();
	}
	public void ShowBackpackCrafting()
    {
		ShowWindowByIndex(windowsOrder.FindIndex((x) => x == craftingSystem));
		ShowWindow();
	}
	public void HideBackpack()
    {
		inventorySystem.CloseWindow();
		craftingSystem.CloseWindow();
		HideWindow();
	}



	public WindowBackpack SetSecondaryContainer(Inventory inventory)
	{
		inventorySystem.SetSecondaryContainer(inventory);

		return this;
	}


	private void ShowWindowByIndex(int index)
	{
		currentIndex = index;
		UpdateHeader();
		navigationHeader.Enable(true);

		OpenWindow(currentIndex);
	}
	private void OpenWindow(int index)
	{
		CloseAll();

		windowsOrder[index].OpenWindow();
	}

	private void Left()
    {
		if (isLoop)//loop
		{
			if (CurrentIndex == 0)
				CurrentIndex = windowsOrder.Count - 1;
			else
				CurrentIndex--;
		}
		else//clamp
		{
			if (CurrentIndex != 0)
			{
				if (CurrentIndex == 0)
					CurrentIndex = windowsOrder.Count - 1;
				else
					CurrentIndex--;
			}
		}
	}
	private void Right()
    {
		if (isLoop)
        {
			if ((CurrentIndex + 1) >= windowsOrder.Count)
				CurrentIndex = 0;
			else
				CurrentIndex++;
		}
        else
        {
			if (CurrentIndex != windowsOrder.Count - 1)
			{

				if ((CurrentIndex + 1) >= windowsOrder.Count)
					CurrentIndex = 0;
				else
					CurrentIndex++;
			}
		}
	}
	
	private void UpdateNavigator(bool direction)
    {
		navigationHeader.SetSelectorDirection(direction);

		UpdateHeader();
	}
	private void UpdateHeader()
    {
		navigationHeader.SetSelectorText(windowsOrder[CurrentIndex].windowName);
		navigationHeader.SetIndicatorsIndex(CurrentIndex);
	}

	private void CloseAll()
    {
		for (int i = 0; i < windowsOrder.Count; i++)
		{
			windowsOrder[i].CloseWindow();
		}
	}

	[Button]
	private void Open()
    {
        for (int i = 0; i < windowsOrder.Count; i++)
        {
			windowsOrder[i].gameObject.SetActive(false);
		}
		inventorySystem.gameObject.SetActive(true);
	}
	[Button]
	private void OpenCraft()
    {
		for (int i = 0; i < windowsOrder.Count; i++)
		{
			windowsOrder[i].gameObject.SetActive(false);
		}
		craftingSystem.gameObject.SetActive(true);
	}

	private void Back()
    {
		onBack?.Invoke();
	}
}