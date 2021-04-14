using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class CamfireBuilding : FireBuilding
{
    public override bool IsCanBeBuild
    {
        get//заменить
        { 
            Inventory inventory = GeneralAvailability.PlayerInventory;
            if (inventory.ContainsType<FireStarterSD>() && inventory.ContainsType<FireTinderSD>() && inventory.ContainsType<FireFuelSD>())
                return true;
            return false;
        }
    }

    public override void Place()
    {
        SetMaterial();

        IsPlacement = true;

        OpenIgnitionWindow();
    }
}
