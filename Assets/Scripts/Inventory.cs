using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class Inventory
{
    public UnityAction<List<Item>> onCollectionChanged;

    [ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private List<ItemData> initItems = new List<ItemData>();
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

    public bool AddItem(ItemData itemData)
    {
        if (itemData != null)
        {
            Item findedItem = items.FindLast((x) => itemData.scriptableData == x.itemData.scriptableData);

            if (findedItem != null)//если нашёл тот же айтем
            {
                ItemData findedData = findedItem.itemData;

                int findedDataMaxSize = findedData.scriptableData.stackSize;
                if (findedData.CurrentStackSize < findedDataMaxSize) // если у айтема которого нашли есть свободное место
                {
                    int count = findedData.CurrentStackSize + itemData.CurrentStackSize;
                    if (count <= findedDataMaxSize)
                    {
                        findedData.CurrentStackSize += itemData.CurrentStackSize;
                    }
                    else
                    {
                        findedData.CurrentStackSize = findedDataMaxSize;

                        Item newItem = new Item(itemData);
                        newItem.itemData.CurrentStackSize = count % findedDataMaxSize;
                        items.Add(newItem);
                    }
                }
                else//последний айтем полный
                {
                    items.Add(new Item(itemData));
                }
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
            ItemData data = item.itemData;

            if (data.CurrentStackSize > count && count > 0)
            {
                data.CurrentStackSize -= count;
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

    public List<Item> GetAllBySD<SD>() => items.FindAll((x) => x.itemData.scriptableData is SD);
}

[System.Serializable]
public class PlayerInventory : Inventory { }