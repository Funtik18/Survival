using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System;
using System.Linq;

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

    private const string Assets = "Resources/Assets/";


    [Title("Items")]
    [AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
    [OnValueChanged("UpdateContainers")]
    public List<ItemSD> allItems = new List<ItemSD>();

    //Consumable Items

    [TitleGroup("Consumable Items")]
    [AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
    [SerializeField] private List<ConsumableItemSD> allConsumables = new List<ConsumableItemSD>();
    [HorizontalGroup("Consumable Items/Split")]
    [VerticalGroup("Consumable Items/Split/Left")]
    [AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
    [SerializeField] private List<PotionItemSD> allDrinks = new List<PotionItemSD>();

    [LabelWidth(100)]
    [VerticalGroup("Consumable Items/Split/Left")] 
    [SerializeField] private WaterItemSD potableWater;
    public WaterItemSD PotableWater => potableWater;

    [LabelWidth(100)]
    [VerticalGroup("Consumable Items/Split/Left")]
    [SerializeField] private WaterItemSD unsafeWater;
    public WaterItemSD UnSafeWater => unsafeWater;

    [VerticalGroup("Consumable Items/Split/Right")]
    [AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
    [SerializeField] private List<FoodItemSD> allFood = new List<FoodItemSD>();
    [LabelWidth(100)]
    [VerticalGroup("Consumable Items/Split/Right")]
    [SerializeField] private SnowItemSD snow;

    //Fire Items

    [TitleGroup("Fire Items")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [SerializeField] private List<FireSD> allFires = new List<FireSD>();

    [HorizontalGroup("Fire Items/Split")]
    [VerticalGroup("Fire Items/Split/Left")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [SerializeField] private List<FireStarterSD> allStarters = new List<FireStarterSD>();
    [VerticalGroup("Fire Items/Split/Left")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [SerializeField] private List<FireFuelSD> allFuels = new List<FireFuelSD>();

    [VerticalGroup("Fire Items/Split/Right")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [SerializeField] private List<FireTinderSD> allTinders = new List<FireTinderSD>();

    [VerticalGroup("Fire Items/Split/Right")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [SerializeField] private List<FireAccelerantSD> allAccelerants = new List<FireAccelerantSD>();

    //Tools Items

    [TitleGroup("Tools Items")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [SerializeField] private List<ToolItemSD> allTools = new List<ToolItemSD>();

    //Blueprints

    [TitleGroup("Blueprints")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [SerializeField] private List<BlueprintSD> allBlueprints = new List<BlueprintSD>();
    public List<BlueprintSD> AllBlueprints => allBlueprints;

    //Containers

    [TitleGroup("Containers")]
    [ReadOnly] [SerializeField] private List<ItemSD> containerItems = new List<ItemSD>();


    private ItemDataWrapper snowItem;
    public ItemDataWrapper Snow
    {
        get
        {
            if(snowItem == null)
            {
                snowItem = new ItemDataWrapper();
                snowItem.scriptableData = snow;
            }
            return snowItem;
        }
    }


    public ItemDataWrapper GetRandomItem()
    {
        ItemDataWrapper item = new ItemDataWrapper();

        item.scriptableData = GetRandomItemSD();

        return item;
    }

    public ItemSD GetRandomItemSD() => allItems.GetRandomItem();//исключить воды
    public ItemSD GetRandomConsumableItemSD() => allConsumables.GetRandomItem();
    public ItemSD GetRandomDrinkItemSD() => allDrinks.GetRandomItem();
    public ItemSD GetRandomFiresItemSD() => allFires.GetRandomItem();


    public ItemDataWrapper GetWater(float volume, bool isPotable)
    {
        ItemDataWrapper dataWrapper = new ItemDataWrapper();

        if(isPotable)
            dataWrapper.scriptableData = potableWater;
        else
            dataWrapper.scriptableData = unsafeWater;

        dataWrapper.CurrentWeight = volume;

        return dataWrapper;
    }

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
    private void UpdateContainers()
    {
        containerItems = MultipleLists(containerItems.ToArray(), allStarters.ToArray(), allAccelerants.ToArray());
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




    private bool Limits(ItemSD item)
    {
        return !(item is WaterItemSD) && !(item is SnowItemSD);
    }
}