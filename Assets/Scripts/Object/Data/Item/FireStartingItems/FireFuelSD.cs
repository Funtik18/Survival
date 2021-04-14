using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/Fire/Fuel", fileName = "Fuel")]
public class FireFuelSD : FireSD 
{
    [Tooltip("К времени горения.")]
    public Times addFireTime;
}