using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/Animal")]
public class AnimalSD : ScriptableObject
{
    public string animalName;

    [Space]
    public MeatItemSD rawMeat;
    [SuffixLabel("kg", true)]
    [Min(0)]
    public float minMeatWeight;
    [SuffixLabel("kg", true)]
    [MinValue("minMeatWeight")]
    public float maxMeatWeight;

    [InfoBox("0 index == hands")]
    [Title("Warm Meat")]
    public List<HarvestingSD.HarvestingData> addDatasMeatKG = new List<HarvestingSD.HarvestingData>();

    [Space]
    public ItemSD hide;
    public int hides;
    public List<HarvestingSD.HarvestingData> addDatasHide = new List<HarvestingSD.HarvestingData>();

    [Space]
    public ItemSD gut;
    public int guts;
    public List<HarvestingSD.HarvestingData> addDatasGuts = new List<HarvestingSD.HarvestingData>();

    public ItemDataWrapper GenerateMeat()
    {
        ItemDataWrapper data = new ItemDataWrapper();
        data.scriptableData = rawMeat;
        data.CurrentBaseWeight = Random.Range(minMeatWeight, maxMeatWeight);
        return data;
    }
}