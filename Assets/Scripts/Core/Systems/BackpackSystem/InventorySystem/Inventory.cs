using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using System;
using System.Collections;

[System.Serializable]
public class Inventory
{
    public UnityAction<List<Item>> onCollectionChanged;
    protected UnityAction onChanged;

    private Data data;

    private List<Item> allItems = new List<Item>();
    public List<Item> AllItems => allItems;

    private List<Item> currentItems = new List<Item>();
    public List<Item> CurrentItems => currentItems;
    public List<Item> CurrentItemsCopy => new List<Item>(CurrentItems);


    private InventorySortGlobal currentGlobalSort = InventorySortGlobal.None;
    private InventorySort currentSort = InventorySort.None;

    public bool IsEmpty => allItems.Count == 0;

    public virtual void Init()
    {
        ItemDataWrapper[] initItems = data.items;
        for (int i = 0; i < initItems.Length; i++)
        {
            AddItem(initItems[i]);
        }

        onChanged += delegate { SetSort(currentSort); };
        SetSort(InventorySort.All);
    }

    public bool AddItem(ItemDataWrapper itemData)
    {
        if (itemData != null)
        {
            Item item = null;

            ItemSD itemSD = itemData.scriptableData;

            if (itemSD.isInfinityWeight)
            {
                item = GetBySD(itemSD);

                if(item != null && item.itemData.scriptableData == itemSD)//нашли одинаковый
                    item.itemData.CurrentBaseWeight += itemData.CurrentBaseWeight;
                else
                    CreateAddItem(itemData);
            }
            else if (itemSD.isInfinityStack)
            {
                item = GetBySD(itemSD);
                if (item != null && item.itemData.scriptableData == itemSD)//нашли одинаковый
                    item.itemData.CurrentStackSize += itemData.CurrentStackSize;
                else
                    CreateAddItem(itemData);
            }
            else
            {
                List<Item> findedSameItems = GetAllBySD(itemSD);

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
                                if (itemData.IsStackEmpty) break;//maybe delete

                                findedData.CurrentStackSize += itemData.CurrentStackSize;
                                itemData.CurrentStackSize -= itemData.CurrentStackSize;
                            }
                        }

                        if (itemData.IsStackEmpty) break;
                    }

                    if (!itemData.IsStackEmpty)
                        CreateAddItem(itemData);
                }
                else
                    CreateAddItem(itemData);
            }

            onChanged?.Invoke();
            return true;
        }
        return false;
    }
    private void CreateAddItem(ItemDataWrapper itemData)
    {
        Item item = new Item(itemData);
        allItems.Add(item);

        Sort();
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
            if (allItems.Contains(item))
            {
                allItems.Remove(item);
                onChanged?.Invoke();

                Sort();

                return true;
            }
        }
        return false;
    }

    public string CurrentStringWeight 
    {
        get
        {
            if(IsEmpty)
            {
                return "EMPTY";
            }
            else
            {
                return GetWeight() + " KG";
            }
        }
    }


    public Inventory SetSort(InventorySort sortType)
    {
        currentSort = sortType;
        CurrentItems.Clear();

        if (currentSort == InventorySort.All)
        {
            CurrentItems.AddRange(allItems);
        }
        else if (currentSort == InventorySort.FireItems)
        {
            CurrentItems.AddRange(GetAllBySD<FireItemSD>());
        }
        else if(currentSort == InventorySort.FirstAidItems)
        {
            CurrentItems.AddRange(GetAllBySD<FirstAidItemSD>());
        }
        else if(currentSort == InventorySort.ClothItems)
        {
            CurrentItems.AddRange(GetAllBySD<ClothingItemSD>());
        }
        else if(currentSort == InventorySort.FoodItems)
        {
            CurrentItems.AddRange(GetAllBySD<ConsumableItemSD>());
        }
        else if(currentSort == InventorySort.ToolsItems)
        {
            CurrentItems.AddRange(GetAllBySD<ToolItemSD>());
        }
        else if(currentSort == InventorySort.MaterialsItems)
        {
            CurrentItems.AddRange(GetAllBySD<MaterialItemSD>());
        }

        onCollectionChanged?.Invoke(CurrentItems);

        return this;
    }
    public Inventory SetSort(InventorySortGlobal sortType)
    {
        if (currentGlobalSort == sortType) return this;

        currentGlobalSort = sortType;

        Sort();

        return this;
    }

    private void Sort()
    {
        if (currentGlobalSort == InventorySortGlobal.ByName)
        {
            SortAlphabetically();
        }
        else if (currentGlobalSort == InventorySortGlobal.ByWeight)
        {
            SortWeight();
        }

        SetSort(currentSort);
    }
    private void SortAlphabetically()
    {
        allItems.Sort((x, y) => string.Compare(x.itemData.scriptableData.objectName, y.itemData.scriptableData.objectName));
    }
    private void SortWeight()
    {
        allItems.Sort((x, y) => x.itemData.CurrentWeight.CompareTo(y.itemData.CurrentWeight));
    }


    public Item FindItemByData(ItemDataWrapper data) => allItems.FindLast((x) => x.itemData == data);
    public Item GetBySD(ItemSD sd) => allItems.FindLast((x) => x.itemData.scriptableData == sd);
    public Item GetBySD<SD>() => allItems.FindLast((x) => x.itemData.scriptableData is SD);

    public List<Item> GetAllBySD(ItemSD sd) => allItems.FindAll((x) => x.itemData.scriptableData == sd);
    public List<Item> GetAllBySD<SD>() => allItems.FindAll((x) => x.itemData.scriptableData is SD);

    public List<Item> GetAllFood(bool onlyCookableFood = false)
    {
        List<Item> finded = null;

        if (onlyCookableFood)
            finded = allItems.FindAll((x) => x.itemData.scriptableData is FoodItemSD && (x.itemData.scriptableData as FoodItemSD).isCookable == true);
        else
            finded = allItems.FindAll((x) => x.itemData.scriptableData is FoodItemSD);

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

    public bool ContainsType<SD>() => allItems.FirstOrDefault((x) => x.itemData.scriptableData is SD) != null;
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


    private float GetWeight()//оптимизировать
    {
        float weight = 0f;
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemDataWrapper data = allItems[i].itemData;

            weight += data.CurrentWeight;
        }
        return (float)System.Math.Round(weight, 2);
    }


 



    private ItemDataWrapper[] GetItemsData()
    {
        List<ItemDataWrapper> itemDatas = new List<ItemDataWrapper>();

        for (int i = 0; i < allItems.Count; i++)
        {
            itemDatas.Add(allItems[i].itemData);
        }

        return itemDatas.ToArray();
    }
    public Inventory SetData(Data data)
    {
        this.data = data;
        return this;
    }
    public Data GetData()
    {
        Data data = new Data()
        {
            items = GetItemsData(),
        };
        return data;
    }

    [System.Serializable]
    public class Data
    {
        public ItemDataWrapper[] items;
    }
}
public enum InventorySortGlobal
{
    None,
    ByName,
    ByWeight,
}
public enum InventorySort
{
    None,
    All,
    FireItems,
    FirstAidItems,
    ClothItems,
    FoodItems,
    ToolsItems,
    MaterialsItems,
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
        AddItem(blueprint.yield.item);
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