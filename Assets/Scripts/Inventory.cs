using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class Inventory 
{
    public UnityAction<List<Item>> onCollectionChanged;

    [ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private List<ItemScriptableData> initItems = new List<ItemScriptableData>();
    [HideInInspector] public List<Item> items = new List<Item>();

    public void Init()
    {
        for (int i = 0; i < initItems.Count; i++)
        {
            AddItem(initItems[i]);
        }
    }

    public bool AddItem(ItemScriptableData item)
    {
        items.Add(new Item(item));
        onCollectionChanged?.Invoke(items);
        return true;
    }
    public bool RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            onCollectionChanged?.Invoke(items);
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class PlayerInventory : Inventory { }