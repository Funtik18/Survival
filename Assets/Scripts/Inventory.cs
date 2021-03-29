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
        if(item != null)
        {
            items.Add(new Item(item));
            onCollectionChanged?.Invoke(items);
            return true;
        }
        return false;
    }
    public bool RemoveItem(Item item)
    {
        if(item != null)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                onCollectionChanged?.Invoke(items);
                return true;
            }
        }
        return false;
    }

    public bool RemoveItems(List<Item> removers)
    {
        if(removers.Count > 0)
        {
            for (int i = 0; i < removers.Count; i++)
            {
                RemoveItem(removers[i]);
            }
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class PlayerInventory : Inventory { }