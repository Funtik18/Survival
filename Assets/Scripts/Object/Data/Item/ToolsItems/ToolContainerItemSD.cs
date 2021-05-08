using UnityEngine;

[CreateAssetMenu(menuName = "Game/Inventory/Items/Tools/Container", fileName = "Item")]
public class ToolContainerItemSD : ToolItemSD
{
    [Min(0.25f)]
    public float waterVolume;
}
