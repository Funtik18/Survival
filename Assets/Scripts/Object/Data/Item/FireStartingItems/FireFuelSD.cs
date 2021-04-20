using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/Fire/Fuel", fileName = "Fuel")]
public class FireFuelSD : FireSD 
{
    [Range(0, 21f)]
    public float addTemperature = 0;

    [Tooltip("К времени горения.")]
    public Times addFireTime;
}