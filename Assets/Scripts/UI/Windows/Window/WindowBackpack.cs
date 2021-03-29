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
		buttonBack.onClick.AddListener(Back);
	}

	public void ShowBackpackInspector()
    {
		primaryContainer.onItemChoosen = itemInspector.SetItem;//события для осмотра предмета

		OpenItemInspector();
		ShowWindow();
	}
	public void ShowBackpackWithContainer()
    {
		primaryContainer.onItemChoosen = (x) => ItemShift(primaryContainer, secondaryContainer, x);
		secondaryContainer.onItemChoosen = (x) => ItemShift(secondaryContainer, primaryContainer, x);

		OpenSecondaryContainer();
		ShowWindow();
	}
	public void HideBackpack()
    {
        if (itemInspector.gameObject.activeSelf)
        {
			itemInspector.SetItem(null);
        }else if (secondaryContainer.gameObject.activeSelf)
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



	private static void ItemShift(ContainerUI from, ContainerUI to, Item x)
    {
        if (x != null)
        {
            to.currentInventory.AddItem(x.ScriptableItem);
            from.currentInventory.RemoveItem(x);
        }
    }

    private void DropItem(Item item)
    {
        Player.Instance.Inventory.RemoveItem(item);
    }

	private void Back()
    {
		onBack?.Invoke();
	}
}
