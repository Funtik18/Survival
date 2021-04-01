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

	[SerializeField] private Button buttonUse;
	[SerializeField] private Button buttonActions;
	[SerializeField] private Button buttonDrop;

	[SerializeField] private ItemView3D view3D;

	//cash
	private Item currentItem;

	private void Awake()
	{
		buttonUse.onClick.AddListener(Use);
		buttonActions.onClick.AddListener(Actions);
		buttonDrop.onClick.AddListener(Drop);

		SetItem(null);
	}

	public void SetItem(Item item)
	{
		currentItem = item;

		if(currentItem != null)
		{
			ItemScriptableData data = currentItem.ScriptableItem;

			itemTittle.text = data.name;
			itemDescription.text = data.description;

			buttonDrop.gameObject.SetActive(true);

			view3D.InstantiateModel(data.model);
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


	private void Use()
    {
		onUse?.Invoke(currentItem);
	}
	private void Actions()
    {
		onActions?.Invoke(currentItem);
	}
	private void Drop()
    {
		onDrop?.Invoke(currentItem);
		SetItem(null);
	}
}