using UnityEngine;

public class HarvestingObject : WorldObject<HarvestingSD> 
{
    [SerializeField] private bool isBreakable = true;
    public bool IsBreakable => isBreakable;

    public override void StartObserve()
    {
        base.StartObserve();
        InteractionButton.SetIconOnInteraction();
        InteractionButton.pointer.AddPressListener(OpenHarvestingWindow);
        InteractionButton.OpenButton();
    }
    public override void EndObserve()
    {
        base.EndObserve();
        InteractionButton.CloseButton();
        InteractionButton.pointer.RemoveAllPressListeners();
    }
    private void OpenHarvestingWindow()
    {
        GeneralAvailability.PlayerUI.OpenHarvesting(this);
    }
}