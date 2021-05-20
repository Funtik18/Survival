using UnityEngine;

public class SleepingBagBuilding : BuildingObject 
{
    public override void Place()
    {
        base.Place();

        OpenSleepingWindow();
    }


    public override void StartObserve()
    {
        base.StartObserve();

        InteractionButton.pointer.AddPressListener(OpenSleepingWindow);
    }
    public override void EndObserve()
    {
        base.EndObserve();

        InteractionButton.pointer.RemoveAllPressListeners();
    }


    private void OpenSleepingWindow()
    {
        GeneralAvailability.PlayerUI.OpeRestingForBag(TakeIt);
    }

    private void TakeIt()
    {
        GeneralAvailability.PlayerInventory.AddItem(StoredItem.itemData);
        DestroyImmediate(gameObject);
    }
}