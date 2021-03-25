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

	[Button]
	private void CloseItemInspector()
	{
		secondaryContainer.gameObject.SetActive(false);
		itemInspector.gameObject.SetActive(true);
	}
	[Button]
	private void OpenSecondaryContainer()
	{
		secondaryContainer.gameObject.SetActive(true);
		itemInspector.gameObject.SetActive(false);
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
