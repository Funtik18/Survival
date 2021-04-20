using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/ItemsData")]
public class ItemsData : MonoBehaviour
{
    private static ItemsData instance;
    public static ItemsData Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<ItemsData>();

                if(instance == null)
                {
                    instance = new GameObject("_ItemsData").AddComponent<ItemsData>();
                }
                if(Application.isPlaying)
                    DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }


    [Title("Items")]
    [AssetList(AutoPopulate = true)]
    [OnValueChanged("UpdateLists")]
    public List<ItemSD> allItems = new List<ItemSD>();

    //

    [TitleGroup("Consumable Items")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<ConsumableItemSD> allConsumables = new List<ConsumableItemSD>();
    [HorizontalGroup("Consumable Items/Split")]
    [VerticalGroup("Consumable Items/Split/Left")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<PotionItemSD> allDrinks = new List<PotionItemSD>();
    [VerticalGroup("Consumable Items/Split/Right")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<FoodItemSD> allFood = new List<FoodItemSD>();

    //

    [TitleGroup("Fire Items")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<FireSD> allFires = new List<FireSD>();

    [HorizontalGroup("Fire Items/Split")]
    [VerticalGroup("Fire Items/Split/Left")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<FireStarterSD> allStarters = new List<FireStarterSD>();
    [VerticalGroup("Fire Items/Split/Left")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<FireFuelSD> allFuels = new List<FireFuelSD>();

    [VerticalGroup("Fire Items/Split/Right")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<FireTinderSD> allTinders = new List<FireTinderSD>();

    [VerticalGroup("Fire Items/Split/Right")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<FireAccelerantSD> allAccelerants = new List<FireAccelerantSD>();

    [Space]
    [ReadOnly] [SerializeField] private List<ItemSD> containerItems = new List<ItemSD>();

    public ItemDataWrapper GetRandomItem()
    {
        ItemDataWrapper item = new ItemDataWrapper();

        item.scriptableData = GetRandomItemSD();

        return item;
    }

    public ItemSD GetRandomItemSD() => allItems.GetRandomItem();
    public ItemSD GetRandomConsumableItemSD(bool waterInclude) => GetConsumableItemsSD(waterInclude).GetRandomItem();
    public ItemSD GetRandomDrinkItemSD(bool waterInclude)
    {
        List<PotionItemSD> list;

        if (waterInclude)
            list = allDrinks;
        else
            list = allDrinks.FindAll((x) => !(x is WaterItemSD));

        return list.GetRandomItem();
    }

    public ItemSD GetRandomFiresItemSD()
    {
        return allFires.GetRandomItem();
    }

    public ItemSD[] GetConsumableItemsSD(bool waterInclude) => waterInclude ? allConsumables.ToArray() : allConsumables.FindAll((x) => !(x is WaterItemSD)).ToArray();


    public List<ItemSD> GetRandomContainer(int count)
    {
        List<ItemSD> items = new List<ItemSD>();

        for (int i = 0; i < count; i++)
        {
            items.Add(containerItems.GetRandomItem());
        }
        
        items.Shuffle();
        
        return items;
    }



    [Button]
    private void UpdateLists()
    {
        containerItems = MultipleLists(GetConsumableItemsSD(false), allStarters.ToArray(), allAccelerants.ToArray());
    }

    private List<ItemSD> MultipleLists(params ItemSD[][] items)
    {
        List<ItemSD> output = new List<ItemSD>();

        for (int i = 0; i < items.Length; i++)
        {
            output.AddRange(items[i]);
        }
        return output;
    }
}