public class ContainerObject : WorldObject
{
    public ContainerScriptableData scriptableData;

	public override void StartObserve()
	{
		base.StartObserve();
		ControlUI.buttonSearch.IsEnable = true;
		ControlUI.targetPoint.SetToolTipText(scriptableData.data.name).ShowToolTip();
	}
    public override void EndObserve()
    {
        base.EndObserve();
		ControlUI.buttonSearch.IsEnable = false;
	}

	public override void Interact()
	{
		print("Searching...");
	}
}