using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContainerSlotUI : MonoBehaviour 
{
	public UnityAction<Item> onClick;

	[SerializeField] private Item item;
	[Space]
	[SerializeField] private Image itemIcon;
	[SerializeField] private Image weightIcon;
	[SerializeField] private ContainerSlotInteractionUI interaction;

	public bool IsEmpty => item == null;


    private void OnEnable()
    {
		interaction.onClick += ClickSlot;
	}
    private void OnDisable()
    {
		Debug.LogError("OnDisable");
		interaction.onClick -= ClickSlot;
	}

	public void SetItem(Item data)
	{
		this.item = data;
		UpdateUI();
	}

	private void UpdateUI()
	{
		itemIcon.enabled = !IsEmpty;
		weightIcon.enabled = !IsEmpty;

		if(!IsEmpty)
		{
			itemIcon.sprite = item.ScriptableItem.itemSprite;
		}
	}

	private void ClickSlot()
	{
		onClick?.Invoke(item);
	}
}