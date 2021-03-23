public class ItemObject : WorldObject
{
	public ItemScriptableData scriptableData;

	private ItemInspector inspector;
	public ItemInspector Inspector
    {
        get
        {
			if (inspector == null)
				inspector = ItemInspector.Instance;
			return inspector;
		}
    }

	public override void StartObserve()
	{
		base.StartObserve();
		ControlUI.buttonPickUp.IsEnable = true;
		ControlUI.targetPoint.SetToolTipText(scriptableData.data.name).ShowToolTip();
	}
    public override void EndObserve()
    {
        base.EndObserve();
		ControlUI.buttonPickUp.IsEnable = false;
	}

	public override void Interact()
	{
		Inspector.SetItem(this, InspectAnimationType.WorldToLocal);
	}
}