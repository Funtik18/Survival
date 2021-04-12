using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class ItemInspectorUI : MonoBehaviour
{
	public UnityAction<Item> onUse;
	public UnityAction<Item> onActions;
	public UnityAction<Item> onDrop;

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
	[Space]
	[SerializeField] private ItemView3D view3D;

	//cash
	private ContainerSlotUI currentSlot;

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

			itemTittle.text = data.name;
			itemDescription.text = data.description;

			view3D.InstantiateModel(data.model);

			CheckItemType(data);
		}
		else
		{
			view3D.DisposePlace();

			buttonUse.gameObject.SetActive(false);
			buttonActions.gameObject.SetActive(false);
			buttonDrop.gameObject.SetActive(false);

			itemTittle.text = "";
			itemDescription.text = "";
		}
	}


	private void CheckItemType(ItemSD item)
    {
		buttonDrop.gameObject.SetActive(true);
	
		if(item is ConsuableItemSD consuable)
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
	}

	private void Use()
    {
		onUse?.Invoke(currentSlot.item);
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