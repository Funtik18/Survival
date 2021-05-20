using UnityEngine;

[CreateAssetMenu(menuName = "Game/Inventory/Items/Tools/Weapon", fileName = "Item")]
public class ToolWeaponSD : ToolItemSD
{
    [Min(1)]
    public int magazineCapacity;
    public ItemAmmunitionSD ammunition;
}