using UnityEngine;

public class ItemObject : WorldObject
{
	[SerializeField] private ItemScriptableData scriptableData;
	[HideInInspector] public Item item;

	private void Awake()
    {
		if (scriptableData == null) Debug.LogError("ItemError", this);

		item = new Item(scriptableData);
    }

    public override void StartObserve()
	{
		base.StartObserve();
		Button.pointer.AddPressListener(Interact);
		Button.SetIconOnPickUp();
		Button.OpenButton();
		GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.name).ShowToolTip();
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