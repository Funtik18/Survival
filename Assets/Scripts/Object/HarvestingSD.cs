using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "HarvestingSD", menuName = "Buildings/Harvesting")]
public class HarvestingSD : ObjectSD 
{
    [AssetList]
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public HarvestingObject model;

    [InfoBox("0 index == hands")]
    public List<HarvestingData> addDatas = new List<HarvestingData>();

    public List<ItemYield> yields = new List<ItemYield>();

    [System.Serializable]
    public class HarvestingData
    {
        public ToolItemSD tool;
        [Min(1f)]
        public float holdTime = 5f;
        public TimeLimits timeLimits;
    }
}