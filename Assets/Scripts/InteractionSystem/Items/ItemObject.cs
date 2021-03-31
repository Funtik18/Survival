using UnityEngine;

public class ItemObject : WorldObject
{
	[SerializeField] private ItemScriptableData scriptableData;
	[HideInInspector] public Item item;
	public ItemData ItemData => item.ItemData;

	private void Awake()
    {
		if (scriptableData == null) Debug.LogError("ItemError", this);

		item = new Item(scriptableData);
    }

    public override void StartObserve()
	{
		base.StartObserve();
		GeneralAvailability.ButtonPickUp.onClicked += Interact;
		GeneralAvailability.ButtonPickUp.IsEnable = true;
		GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.information.name).ShowToolTip();
	}
    public override void EndObserve()
    {
        base.EndObserve();
		GeneralAvailability.TargetPoint.HideToolTip();
		GeneralAvailability.ButtonPickUp.IsEnable = false;
		GeneralAvailability.ButtonPickUp.onClicked -= Interact;
	}

	public override void Interact()
	{
		GeneralAvailability.Inspector.SetItem(this);
	}
}