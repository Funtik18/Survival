using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using System;

[System.Serializable]
public class Inventory
{
    public UnityAction<List<Item>> onCollectionChanged;

    [ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private List<ItemDataWrapper> initItems = new List<ItemDataWrapper>();
    [HideInInspector] public List<Item> items = new List<Item>();

    public bool IsEmpty => items.Count == 0;


    public bool ContainsType<SD>() => items.FirstOrDefault((x) => x.itemData.scriptableData is SD) != null;

    public void Init()
    {
        for (int i = 0; i < initItems.Count; i++)
        {
            AddItem(initItems[i]);
        }
    }

    public bool AddItem(ItemDataWrapper itemData)
    {
        if (itemData != null)
        {
            List<Item> findedSameItems = GetAllBySD(itemData.scriptableData);

            if(findedSameItems.Count > 0)
            {
                for (int i = 0; i < findedSameItems.Count; i++)
                {
                    ItemDataWrapper findedData = findedSameItems[i].itemData;
                    int difference = findedData.StackDiffrence;

                    if (difference != 0)
                    {
                        if(itemData.StackSize >= difference)
                        {
                            itemData.StackSize -= difference;
                            findedData.StackSize += difference;
                        }
                        else
                        {
                            if (itemData.StackSize == 0) break;//maybe delete

                            findedData.StackSize += itemData.StackSize;
                            itemData.StackSize -= itemData.StackSize;
                        }
                    }
                    
                    if (itemData.StackSize == 0) break;
                }

                if(itemData.StackSize > 0)
                    items.Add(new Item(itemData));
            }
            else
            {
                items.Add(new Item(itemData));
            }

            onCollectionChanged?.Invoke(items);
            return true;
        }
        return false;
    }
    public bool RemoveItem(Item item, int count)
    {
        if (item != null)
        {
            ItemDataWrapper data = item.itemData;

            if (data.StackSize > count && count > 0)
            {
                data.StackSize -= count;
            }
            else
            {
                RemoveItem(item);
            }

            return true;
        }
        return false;
    }
    public bool RemoveItem(Item item)
    {
        if (item != null)
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

    public List<Item> GetAllBySD(ItemSD sd) => items.FindAll((x) => x.itemData.scriptableData == sd);
    public List<Item> GetAllBySD<SD>() => items.FindAll((x) => x.itemData.scriptableData is SD);
}

[System.Serializable]
public class PlayerInventory : Inventory { }