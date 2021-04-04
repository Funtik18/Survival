using UnityEngine;

public class ItemObject : WorldObject
{
	[SerializeField] private ItemData itemData;
	[HideInInspector] public Item item;

	private void Awake()
    {
		if (itemData == null) Debug.LogError("ItemError", this);

		item = new Item(itemData);
    }

    public override void StartObserve()
	{
		base.StartObserve();
		Button.pointer.AddPressListener(Interact);
		Button.SetIconOnPickUp();
		Button.OpenButton();
		GeneralAvailability.TargetPoint.SetToolTipText(itemData.scriptableData.name).ShowToolTip();
	}
    public override void EndObserve()
    {
        base.EndObserve();
		GeneralAvailability.TargetPoint.HideToolTip();
		Button.CloseButton();
		Button.pointer.RemovePressListener(Interact);
	}

	public override void Interact()
	{
		GeneralAvailability.Inspector.SetItem(this);
	}
}