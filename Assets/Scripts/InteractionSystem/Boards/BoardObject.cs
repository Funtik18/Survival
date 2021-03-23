public class BoardObject : WorldBoard
{
    public BoardScriptableData scriptableData;

    public override void StartObserve()
    {
        base.StartObserve();
        ControlUI.targetPoint.SetToolTipText(scriptableData.data.description).ShowToolTip();
    }
}
