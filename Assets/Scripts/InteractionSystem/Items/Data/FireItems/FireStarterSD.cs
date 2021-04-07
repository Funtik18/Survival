using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Fire/Starter", fileName = "Starter")]
public class FireStarterSD : FireSD 
{
    [Tooltip("Время на розжиг")]
    public Times KindleTime;

    [Tooltip("К времени горения.")]
    public Times addFireTime;
}