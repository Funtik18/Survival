using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Game/Inventory/Items/Tools/Container", fileName = "Item")]
public class ToolContainerItemSD : ToolItemSD
{
    [InfoBox("Объём измеряется в литрах.")]
    [Min(0.25f)]
    public float volume;
}
