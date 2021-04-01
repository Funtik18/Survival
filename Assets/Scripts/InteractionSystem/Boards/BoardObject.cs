public class BoardObject : WorldBoard
{
    public BoardScriptableData scriptableData;

    public override void StartObserve()
    {
        base.StartObserve();
        GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.description).ShowToolTip();
    }
}
