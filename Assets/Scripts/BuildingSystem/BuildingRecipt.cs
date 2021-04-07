using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Recipt", menuName = "Buildings/BuildingRecipt")]
public class BuildingRecipt : ScriptableObject
{
    public List<ItemSD> tools = new List<ItemSD>();

    public Times time;

    public List<BuildingMaterial> materials = new List<BuildingMaterial>();
}
[System.Serializable]
public class BuildingMaterial
{
    public ItemSD data;
    public int count;
}