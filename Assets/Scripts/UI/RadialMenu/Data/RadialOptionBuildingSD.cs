using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "RadialOption", menuName = "Game/RadialMenu/BuildingOption")]
public class RadialOptionBuildingSD : RadialOptionSD
{
    [AssetList]
    public BuildingObject building;

    public override void EventInvoke()
    {
        base.EventInvoke();

        GeneralAvailability.Build.BuildBuilding(building);//build
    }
}
