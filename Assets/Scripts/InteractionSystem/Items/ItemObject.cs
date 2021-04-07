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
		InteractionButton.pointer.AddPressListener(Interact);
		InteractionButton.SetIconOnPickUp();
		InteractionButton.OpenButton();
		GeneralAvailability.TargetPoint.SetToolTipText(itemData.scriptableData.name).ShowToolTip();
	}
    public override void EndObserve()
    {
        base.EndObserve();
		GeneralAvailability.TargetPoint.HideToolTip();
		InteractionButton.CloseButton();
		InteractionButton.pointer.RemovePressListener(Interact);
	}

	public override void Interact()
	{
		GeneralAvailability.Inspector.SetItem(this);
	}
}