using System.Collections.Generic;

using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemScriptableData> items = new List<ItemScriptableData>();

    [SerializeField] private InventoryGrid grid;
}