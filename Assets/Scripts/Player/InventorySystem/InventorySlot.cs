using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour 
{
	public UnityAction<ItemScriptableData> onClick;

	[SerializeField] private ItemScriptableData item;
	[Space]
	[SerializeField] private Image itemIcon;
	[SerializeField] private Image weightIcon;
	[SerializeField] private InventorySlotInteraction interaction;

	public bool IsEmpty => item == null;

	private void Awake()
	{
		interaction.onClick += ClickSlot;
	}

	public void SetItem(ItemScriptableData data)
	{
		this.item = data;
		UpdateUI();
	}

	public void UpdateUI()
	{
		itemIcon.enabled = !IsEmpty;
		weightIcon.enabled = !IsEmpty;

		if(!IsEmpty)
		{
			itemIcon.sprite = item.data.itemSprite;
		}
	}

	private void ClickSlot()
	{
		onClick?.Invoke(item);
	}
}