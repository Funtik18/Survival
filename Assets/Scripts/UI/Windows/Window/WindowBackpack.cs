using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBackpack : WindowUI
{
	public UnityAction onBack;

	public ContainerUI primaryContainer;
	public ContainerUI secondaryContainer;
	public ItemInspectorUI itemInspector;

	[Title("Buttons")]
	[SerializeField] private Button buttonBack;

	private void Awake()
	{
		itemInspector.onDrop += DropItem;
		primaryContainer.onUpdated += RefreshItemInspector;
		buttonBack.onClick.AddListener(Back);
	}

	public void ShowBackpackInspector()
    {
		primaryContainer.onSlotChoosen = itemInspector.SetItem;//события для осмотра предмета

		OpenItemInspector();
		ShowWindow();
	}
	public void ShowBackpackWithContainer()
    {
		primaryContainer.onSlotChoosen = (x) => ItemShift(primaryContainer, secondaryContainer, x.item);
		secondaryContainer.onSlotChoosen = (x) => ItemShift(secondaryContainer, primaryContainer, x.item);

		OpenSecondaryContainer();
		ShowWindow();
	}
	public void HideBackpack()
    {
        if (itemInspector.gameObject.activeSelf)
        {
			itemInspector.SetItem(null);
			primaryContainer.RefreshContainer();
		}
		else if (secondaryContainer.gameObject.activeSelf)
        {
			secondaryContainer.UnSubscribeInventory();
		}
		HideWindow();
	}


	public void OpenItemInspector()
	{
		secondaryContainer.gameObject.SetActive(false);
		itemInspector.gameObject.SetActive(true);
	}
	public void OpenSecondaryContainer()
	{
		secondaryContainer.gameObject.SetActive(true);
		itemInspector.gameObject.SetActive(false);
	}

	public void RefreshItemInspector()
    {
		itemInspector.SetItem(null);
	}

	private static void ItemShift(ContainerUI from, ContainerUI to, Item x)
    {
        if (x != null)
        {
            to.currentInventory.AddItem(x.itemData);
            from.currentInventory.RemoveItem(x);
        }
    }

    private void DropItem(Item item)
    {
		ItemData itemData = item.itemData;
		if(itemData.CurrentStackSize > 1)
        {
			if(itemData.CurrentStackSize > 4)
            {
				GeneralAvailability.ExchangerWindow.SetItem(item);
            }
            else
            {
				GeneralAvailability.PlayerInventory.RemoveItem(item, 1);
			}
		}
        else
        {
			GeneralAvailability.PlayerInventory.RemoveItem(item);
		}
	}

	private void Back()
    {
		onBack?.Invoke();
	}
}
