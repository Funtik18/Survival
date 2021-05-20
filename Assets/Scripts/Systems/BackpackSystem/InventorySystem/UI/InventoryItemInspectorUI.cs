using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class InventoryItemInspectorUI : MonoBehaviour
{
	public UnityAction<Item> onUse;
	public UnityAction<Item> onActions;
	public UnityAction<Item> onDrop;

	[SerializeField] private InspectorUI inspectorUI;
	[Space]
	[SerializeField] private TMPro.TextMeshProUGUI itemTittle;
	[SerializeField] private TMPro.TextMeshProUGUI itemDescription;
	[Space]
	[SerializeField] private Button buttonUse;
	[SerializeField] private Button buttonActions;
	[SerializeField] private Button buttonDrop;
	[Space]
	[SerializeField] private TMPro.TextMeshProUGUI useText;
	[SerializeField] private TMPro.TextMeshProUGUI actionsText;
	[SerializeField] private TMPro.TextMeshProUGUI dropText;

	private ContainerSlotUI currentSlot;
	private ItemDataWrapper itemData;

	private void Awake()
	{
		buttonUse.onClick.AddListener(Use);
		buttonActions.onClick.AddListener(Actions);
		buttonDrop.onClick.AddListener(Drop);

		SetItem(null);
	}

	public void SetItem(ContainerSlotUI slot)
	{
		currentSlot = slot;

		if(currentSlot != null && !currentSlot.IsEmpty)
		{
			ItemSD data = currentSlot.ScriptableData;

			itemTittle.text = data.objectName;
			itemDescription.text = data.description;

			inspectorUI.InstantiateModel(data.model);

			itemData = currentSlot.Data;

			CheckItemType();
		}
		else
		{
			inspectorUI.Dispose();

			buttonUse.gameObject.SetActive(false);
			buttonActions.gameObject.SetActive(false);
			buttonDrop.gameObject.SetActive(false);

			itemTittle.text = "";
			itemDescription.text = "";
		}
	}

	private void CheckItemType()
    {
		buttonDrop.gameObject.SetActive(true);
	
		if(itemData.scriptableData is ConsumableItemSD consuable)
        {
			if(consuable is PotionItemSD)
            {
				useText.text = "DRINK";
			}
			else if(consuable is FoodItemSD)
            {
				useText.text = "EAT";
			}

			buttonUse.gameObject.SetActive(true);
        }
		else if(itemData.scriptableData is ToolWeaponSD)
        {
			if (GeneralAvailability.Player.Status.opportunities.IsEquiped(itemData))
            {
				useText.text = "UNEQUIP";
            }
            else
            {
				useText.text = "EQUIP";
			}

			buttonUse.gameObject.SetActive(true);
		}
		else
        {
			buttonUse.gameObject.SetActive(false);
		}
	}

	private void Use()
    {
		onUse?.Invoke(currentSlot.item);

		CheckItemType();
	}
	private void Actions()
    {
		onActions?.Invoke(currentSlot.item);
	}
	private void Drop()
    {
		onDrop?.Invoke(currentSlot.item);
	}
}