using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "RadialOption", menuName = "RadialMenu/BuildingOption")]
public class RadialOptionBuildingScriptableData : RadialOptionScriptableData
{
    [AssetList]
    public BuildingObject building;

    public override void EventInvoke()
    {
        base.EventInvoke();

        GeneralAvailability.Build.BuildBuilding(building);//build
    }
}
