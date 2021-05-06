public class TentBuilding : BuildingObject
{
    public override void StartObserve()
    {
        base.StartObserve();
        InteractionButton.pointer.AddPressListener(OpenRestingWindow);
    }
    public override void EndObserve()
    {
        base.EndObserve();

        InteractionButton.pointer.RemoveAllPressListeners();
    }

    private void OpenRestingWindow()
    {
        GeneralAvailability.PlayerUI.OpenResting();
    }
}