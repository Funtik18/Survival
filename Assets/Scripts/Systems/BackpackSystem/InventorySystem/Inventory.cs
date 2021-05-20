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
    public UnityAction onChanged;

    private InventoryData data;

    [HideInInspector] public List<Item> items = new List<Item>();

    public bool IsEmpty => items.Count == 0;

    public Inventory SetData(InventoryData data)
    {
        this.data = data;
        return this;
    }
    public virtual void Init()
    {
        ItemDataWrapper[] initItems = data.items;
        for (int i = 0; i < initItems.Length; i++)
        {
            AddItem(initItems[i]);
        }
    }

    public bool AddItem(ItemDataWrapper itemData)
    {
        Item item = null;

        if (itemData != null)
        {
            if (itemData.scriptableData.isInfinityStack)
            {
                item = GetBySD(itemData.scriptableData);

                if(item != null && item.itemData.scriptableData == itemData.scriptableData)
                {

                    if(itemData.scriptableData is WaterItemSD waterItem)
                    {
                        item.itemData.CurrentWeight += itemData.CurrentWeight;
                    }
                    else
                    {
                        item.itemData.CurrentStackSize += itemData.CurrentStackSize;
                    }
                }
                else
                {
                    item = new Item(itemData);
                    items.Add(item);
                }
            }
            else
            {
                List<Item> findedSameItems = GetAllBySD(itemData.scriptableData);

                if (findedSameItems.Count > 0)
                {
                    for (int i = 0; i < findedSameItems.Count; i++)
                    {
                        item = findedSameItems[i];
                        ItemDataWrapper findedData = item.itemData;
                        int difference = findedData.StackDiffrence;

                        if (difference != 0)
                        {
                            if (itemData.CurrentStackSize >= difference)
                            {
                                itemData.CurrentStackSize -= difference;
                                findedData.CurrentStackSize += difference;
                            }
                            else
                            {
                                if (itemData.CurrentStackSize == 0) break;//maybe delete

                                findedData.CurrentStackSize += itemData.CurrentStackSize;
                                itemData.CurrentStackSize -= itemData.CurrentStackSize;
                            }
                        }

                        if (itemData.CurrentStackSize == 0) break;
                    }

                    if (itemData.CurrentStackSize > 0)
                    {
                        item = new Item(itemData);
                        items.Add(item);
                    }
                }
                else
                {
                    item = new Item(itemData);
                    items.Add(item);
                }
            }
            

            onCollectionChanged?.Invoke(items);
            onChanged?.Invoke();
            return true;
        }
        return false;
    }


    public bool RemoveItem(Item item, int count)
    {
        if (item != null)
        {
            ItemDataWrapper data = item.itemData;

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
                onChanged?.Invoke();
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

    public float GetWeight()//оптимизировать
    {
        float weight = 0f;
        for (int i = 0; i < items.Count; i++)
        {
            ItemDataWrapper data = items[i].itemData;

            weight += data.CurrentStackSize * data.scriptableData.weight;
        }
        return weight;
    }

    public Item FindItemByData(ItemDataWrapper data) => items.FindLast((x) => x.itemData == data);
    public Item GetBySD(ItemSD sd) => items.FindLast((x) => x.itemData.scriptableData == sd);
    public Item GetBySD<SD>() => items.FindLast((x) => x.itemData.scriptableData is SD);

    public List<Item> GetAllBySD(ItemSD sd) => items.FindAll((x) => x.itemData.scriptableData == sd);
    public List<Item> GetAllBySD<SD>() => items.FindAll((x) => x.itemData.scriptableData is SD);

    public List<Item> GetAllFood(bool onlyCookableFood = false)
    {
        List<Item> finded = null;

        if (onlyCookableFood)
            finded = items.FindAll((x) => x.itemData.scriptableData is FoodItemSD && (x.itemData.scriptableData as FoodItemSD).isCookable == true);
        else
            finded = items.FindAll((x) => x.itemData.scriptableData is FoodItemSD);

        return finded;
    }

    public List<Item> GetAllBoilLiquid()
    {
        List<Item> liquids = new List<Item>();

        Item unsafeWater = GetBySD(ItemsData.Instance.UnSafeWater);

        if (unsafeWater != null)
        {
            liquids.Add(unsafeWater);
        }


        return liquids;
    }

    public bool ContainsType<SD>() => items.FirstOrDefault((x) => x.itemData.scriptableData is SD) != null;
    public bool ContainsType(ItemsData.Categories categories)
    {
        if ((categories & ItemsData.Categories.FireFuelSD) != ItemsData.Categories.None)
        {
            if (!ContainsType<FireFuelSD>())
            {
                return false;
            }
        }
        if ((categories & ItemsData.Categories.FireStarterSD) != ItemsData.Categories.None)
        {
            if (!ContainsType<FireStarterSD>())
            {
                return false;
            }
        }
        if ((categories & ItemsData.Categories.FireTinderSD) != ItemsData.Categories.None)
        {
            if (!ContainsType<FireTinderSD>())
            {
                return false;
            }
        }

        return true;
    }


    private ItemDataWrapper[] GetItemsData()
    {
        List<ItemDataWrapper> itemDatas = new List<ItemDataWrapper>();

        for (int i = 0; i < items.Count; i++)
        {
            itemDatas.Add(items[i].itemData);
        }

        return itemDatas.ToArray();
    }

    public InventoryData GetData()
    {
        InventoryData data = new InventoryData()
        {
            items = GetItemsData(),
        };
        return data;
    }
}
[System.Serializable]
public struct InventoryData
{
    public ItemDataWrapper[] items;
}


[System.Serializable]
public class PlayerInventory : Inventory
{
    public UnityAction<List<BlueprintAvailability>> onBlueprintsReCreated;
    public UnityAction onBlueprintsUpdated;

    private List<BlueprintAvailability> blueprintAvailabilities = new List<BlueprintAvailability>();
    public List<BlueprintAvailability> BlueprintAvailabilities
    {
        get
        {
            if(blueprintAvailabilities.Count == 0)
            {
                SetupBlueprints();
                onChanged = UpdateBlueprints;
                UpdateBlueprints();
            }
            return blueprintAvailabilities;
        }
    }

    public void RemoveItem(BlueprintItem blueprint)
    {
        RemoveItem(GetBySD(blueprint.item), blueprint.count);////Могут быть ошибки
    }


    private void SetupBlueprints()
    {
        List<BlueprintSD> blueprints = ItemsData.Instance.AllBlueprints;

        if (blueprints.Count == 0) Debug.LogError("ERROR");

        for (int i = 0; i < blueprints.Count; i++)
        {
            blueprintAvailabilities.Add(new BlueprintAvailability(blueprints[i]));
        }

        onBlueprintsReCreated?.Invoke(blueprintAvailabilities);

    }

    private void UpdateBlueprints()
    {
        for (int i = 0; i < blueprintAvailabilities.Count; i++)
        {
            blueprintAvailabilities[i].availability = IsContainsComponents(blueprintAvailabilities[i].blueprint);
        }

        onBlueprintsUpdated?.Invoke();
    }

    public bool IsContainsComponents(BlueprintSD blueprint)
    {
        List<BlueprintItem> components = blueprint.components;

        for (int i = 0; i < components.Count; i++)
        {
            if(IsContainsBlueprintItem(components[i]) == false)
            {
                return false;
            }
        }

        return true;
    }
    public void BlueprintExchange(BlueprintSD blueprint)
    {
        List<BlueprintItem> components = blueprint.components;

        for (int i = 0; i < components.Count; i++)
        {
            BlueprintExchange(components[i]);
        }
        AddItem(blueprint.itemYield);
    }


    public bool IsContainsBlueprintItem(BlueprintItem blueprintItem)
    {
        Item item = GetBySD(blueprintItem.item);

        if (item != null)
        {
            if (item.itemData.CurrentStackSize < blueprintItem.count)//тут нужно проверять общее колво всех айтемов в инвентаре
                return false;
        }
        else return false;

        return true;
    }
    public void BlueprintExchange(BlueprintItem blueprintItem)
    {
        RemoveItem(blueprintItem);
    }


    private List<Item> fastAccess = new List<Item>();

    public Item FastAccessCheckItem(ItemSD itemSD)
    {
        for (int i = 0; i < fastAccess.Count; i++)
        {
            if(fastAccess[i].itemData.scriptableData == itemSD)
            {
                return fastAccess[i];
            }
        }
        return null;
    }

    public Item FastAccessGetItem(ItemSD itemSD)
    {
        Item item = FastAccessCheckItem(itemSD);

        if(item == null)
        {
            item = GetBySD(itemSD);

            if(item != null)
                fastAccess.Add(item);
        }

        return item;
    }
}