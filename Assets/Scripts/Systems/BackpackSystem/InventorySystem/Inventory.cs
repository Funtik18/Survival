using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class Inventory
{
    public UnityAction<List<Item>> onCollectionChanged;
    public UnityAction onChanged;

    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<ItemDataWrapper> initItems = new List<ItemDataWrapper>();
    [HideInInspector] public List<Item> items = new List<Item>();

    public bool IsEmpty => items.Count == 0;

    public virtual void Init()
    {
        for (int i = 0; i < initItems.Count; i++)
        {
            AddItem(initItems[i]);
        }
        //initItems.Clear();
    }

    public bool AddItem(ItemDataWrapper itemData)
    {
        if (itemData != null)
        {
            if (itemData.scriptableData.isInfinityStack)
            {
                Item item = GetBySD(itemData.scriptableData);

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
                    items.Add(new Item(itemData));
                }
            }
            else
            {
                List<Item> findedSameItems = GetAllBySD(itemData.scriptableData);

                if (findedSameItems.Count > 0)
                {
                    for (int i = 0; i < findedSameItems.Count; i++)
                    {
                        ItemDataWrapper findedData = findedSameItems[i].itemData;
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
                        items.Add(new Item(itemData));
                }
                else
                {
                    items.Add(new Item(itemData));
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
            Item item = GetBySD(components[i].item);

            if (item != null)
            {
                if (item.itemData.CurrentStackSize < components[i].count)//тут нужно проверять общее колво всех айтемов в инвентаре
                    return false;
            }
            else
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
            RemoveItem(components[i]);
        }
        AddItem(blueprint.itemYield);
    }
}