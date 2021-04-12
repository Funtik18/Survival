using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Fire/Starter", fileName = "Starter")]
public class FireStarterSD : FireSD 
{
    [Tooltip("Сколько секунд розжигать.")]
    [Min(1f)]
    public float holdTime;

    [Tooltip("Время на розжиг")]
    public Times kindleTime;

    [Tooltip("К времени горения.")]
    public Times addFireTime;
}