using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContainerSlotUI : MonoBehaviour 
{
	public UnityAction<ContainerSlotUI> onClick;

	[HideInInspector] public Item item;
	[Space]
	[SerializeField] private Image itemIcon;
	[SerializeField] private Image abstractImage;
	[SerializeField] private Image isItemChoosenImage;
	[SerializeField] private Image isItemEqupedImage;
	[Space]
	[SerializeField] private CanvasGroup itemWeightCanvasGroup;
	[SerializeField] private CanvasGroup itemCountCanvasGroup;
	[SerializeField] private CanvasGroup itemDurabilityCanvasGroup;
	[Space]
	[SerializeField] private TMPro.TextMeshProUGUI countText;
	[SerializeField] private TMPro.TextMeshProUGUI durabilityText;
	[SerializeField] private TMPro.TextMeshProUGUI weightText;
	[Space]
	[SerializeField] private Pointer pointer;

	[Space]
	[SerializeField] private List<Sprite> abstractSprites = new List<Sprite>();

	public bool IsEmpty => item == null;

	public ItemDataWrapper Data => item.itemData;
	public ItemSD ScriptableData => item.itemData.scriptableData;

	private ContainerUI owner;

	public ContainerSlotUI Setup(ContainerUI owner)
    {
		this.owner = owner;

		pointer.AddPressListener(ClickSlot);
		return this;
	}
	public void SetItem(Item item)
	{
		this.item = item;
        if (!IsEmpty)
			item.itemData.onDataChanged += UpdateUI;
		UpdateUI();
	}

	private void UpdateUI()
	{
        if (IsEmpty)
        {
			HideItemInformation();
		}
		else
        {
			ShowItemInformation();
			owner.UpdateWeight();
		}
	}

	private void HideItemInformation()
    {
		isItemEqupedImage.enabled = false;

		itemIcon.enabled = false;

		abstractImage.enabled = false;

		ShowHideItemWeight(false);
		ShowHideItemCount(false);
		ShowHideItemDurability(false);
	}
	private void ShowItemInformation()
    {
		isItemEqupedImage.enabled = false;

		itemIcon.sprite = ScriptableData.itemSprite;
		itemIcon.enabled = true;

		abstractImage.sprite = abstractSprites[Random.Range(0, abstractSprites.Count)];//делете
		abstractImage.enabled = true;

		weightText.text = Data.CurrentStringWeight;
		ShowHideItemWeight(true);

		if (ScriptableData.isBreakable)
		{
			durabilityText.text = Data.CurrentStringDurability;
			ShowHideItemDurability(true);
		}
		else
			ShowHideItemDurability(false);

		if (ScriptableData.stackSize == 1)
        {
			ShowHideItemCount(false);
		}
		else
		{
			countText.text = "x" + Data.CurrentStackSize;
			ShowHideItemCount(true);
		}
	}

	private void ShowHideItemWeight(bool trigger)
    {
		itemWeightCanvasGroup.IsEnabled(trigger, true);
	}
	private void ShowHideItemDurability(bool trigger)
	{
		itemDurabilityCanvasGroup.IsEnabled(trigger, true);
	}
	private void ShowHideItemCount(bool trigger)
    {
		itemCountCanvasGroup.IsEnabled(trigger, true);
	}


	[Button]
	public void Choose()
    {
		isItemChoosenImage.enabled = true;
	}
	[Button]
	public void UnChoose()
    {
		isItemChoosenImage.enabled = false;
	}
	[Button]
	private void OpenSlot()
    {
		itemIcon.enabled = true;
		isItemChoosenImage.enabled = false;

		abstractImage.enabled = true;
		abstractImage.sprite = abstractSprites[Random.Range(0, abstractSprites.Count)];

		ShowHideItemWeight(true);
		ShowHideItemCount(true);
		ShowHideItemDurability(true);
	}
	[Button]
	private void CloseSlot()
    {
		HideItemInformation();
	}

	private void ClickSlot()
	{
		onClick?.Invoke(this);
	}
}