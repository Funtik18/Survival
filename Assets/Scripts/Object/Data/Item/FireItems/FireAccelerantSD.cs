using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Fire/Accelerant", fileName = "Accelerant")]
public class FireAccelerantSD : FireSD 
{
    [Tooltip("Сколько секунд розжигать.")]
    [Min(1f)]
    public float holdTime;
}
