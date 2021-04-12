using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

using Sirenix.OdinInspector;

public class WindowBackpack : WindowUI
{
	public UnityAction onBack;

	public ContainerUI primaryContainer;
	public ContainerUI secondaryContainer;
	public ItemInspectorUI itemInspector;

	[SerializeField] private TMPro.TextMeshProUGUI weight;
	[Title("Buttons")]
	[SerializeField] private Button buttonBack;

	private PlayerOpportunities opportunities;

	private void Awake()
	{
		itemInspector.onUse += UseItem;
		itemInspector.onActions += ActionsWith;
		itemInspector.onDrop += DropItem;
		primaryContainer.onUpdated += RefreshItemInspector;
		primaryContainer.onUpdated += RefreshWeight;
		buttonBack.onClick.AddListener(Back);

		opportunities = GeneralAvailability.Player.Opportunities;
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

	private void RefreshItemInspector()
    {
		itemInspector.SetItem(null);
	}
	private void RefreshWeight()
    {
		weight.text = primaryContainer.currentInventory.GetWeight() + " / " + "30" + "KG";
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

	private void Back()
    {
		onBack?.Invoke();
	}
}
