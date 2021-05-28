using UnityEngine;

[CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Accelerant", fileName = "Accelerant")]
public class FireAccelerantSD : FireItemSD 
{
    [Tooltip("Сколько секунд розжигать.")]
    [Min(1f)]
    public float holdTime;
}
