using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "HarvestingSD", menuName = "Buildings/Harvesting")]
public class HarvestingSD : ObjectSD 
{
    [AssetList]
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public HarvestingObject model;

    [Tooltip("Сколько секунд собирать.")]
    [Min(1f)]
    public float holdTime = 5f;
    [SuffixLabel("cal", true)]
    [Min(0)]
    public float howManyCalories = 10;
    public Times howLong;
    

    public List<ItemData> items = new List<ItemData>();
    public List<ItemSD> tools = new List<ItemSD>();
}