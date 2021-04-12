using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentBuilding : BuildingObject
{
    public override void StartObserve()
    {
        base.StartObserve();
        InteractionButton.pointer.AddPressListener(OpenRestingWindow);

        Debug.LogError("Here");
    }
    public override void EndObserve()
    {
        base.EndObserve();

        Debug.LogError("Out");

        InteractionButton.pointer.RemoveAllPressListeners();
    }

    private void OpenRestingWindow()
    {
        GeneralAvailability.PlayerUI.OpenResting();
    }
}
