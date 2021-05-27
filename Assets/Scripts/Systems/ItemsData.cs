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
    [ReadOnly] [SerializeField] private List<ItemSD> allItems = new List<ItemSD>();

    //Consumable Items

    [TitleGroup("Consumable Items")]
    [AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
    [ReadOnly] [SerializeField] private List<ConsumableItemSD> allConsumables = new List<ConsumableItemSD>();
    [HorizontalGroup("Consumable Items/Split")]
    [VerticalGroup("Consumable Items/Split/Left")]
    [AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
    [ReadOnly] [SerializeField] private List<PotionItemSD> allDrinks = new List<PotionItemSD>();

    [LabelWidth(100)]
    [VerticalGroup("Consumable Items/Split/Left")]
    [ReadOnly] [SerializeField] private WaterItemSD potableWater;
    public WaterItemSD PotableWater => potableWater;

    [LabelWidth(100)]
    [VerticalGroup("Consumable Items/Split/Left")]
    [ReadOnly] [SerializeField] private WaterItemSD unsafeWater;
    public WaterItemSD UnSafeWater => unsafeWater;

    [VerticalGroup("Consumable Items/Split/Right")]
    [AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
    [ReadOnly] [SerializeField] private List<FoodItemSD> allFood = new List<FoodItemSD>();
    [LabelWidth(100)]
    [VerticalGroup("Consumable Items/Split/Right")]
    [ReadOnly] [SerializeField] private SnowItemSD snow;
    [LabelWidth(100)]
    [VerticalGroup("Consumable Items/Split/Right")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<MeatItemSD> allMeat = new List<MeatItemSD>();

    //Fire Items

    [TitleGroup("Fire Items")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<FireSD> allFires = new List<FireSD>();

    [HorizontalGroup("Fire Items/Split")]
    [VerticalGroup("Fire Items/Split/Left")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<FireStarterSD> allStarters = new List<FireStarterSD>();
    [VerticalGroup("Fire Items/Split/Left")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<FireFuelSD> allFuels = new List<FireFuelSD>();

    [VerticalGroup("Fire Items/Split/Right")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<FireTinderSD> allTinders = new List<FireTinderSD>();

    [VerticalGroup("Fire Items/Split/Right")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<FireAccelerantSD> allAccelerants = new List<FireAccelerantSD>();

    //Tools Items

    [TitleGroup("Tools Items")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<ToolItemSD> allTools = new List<ToolItemSD>();

    [AssetList(AutoPopulate = true, Path = Assets)]
    [HorizontalGroup("Tools Items/Split")]
    [VerticalGroup("Tools Items/Split/Left")] 
    [ReadOnly][SerializeField] private List<ToolWeaponSD> allWeapons = new List<ToolWeaponSD>();


    //Materials Items

    [TitleGroup("Materials Items")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<MaterialItemSD> allMaterials = new List<MaterialItemSD>();

    //Blueprints

    [TitleGroup("Blueprints")]
    [AssetList(AutoPopulate = true, Path = Assets)]
    [ReadOnly] [SerializeField] private List<BlueprintSD> allBlueprints = new List<BlueprintSD>();
    public List<BlueprintSD> AllBlueprints => allBlueprints;

    //Containers
    [TitleGroup("Containers")]
    [SerializeField] private Container playerContainer;
    [SerializeField] private Container container;
    public InventoryData PlayerContainer => playerContainer.GetInventoryData();
    public InventoryData Container => container.GetInventoryData();

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

        dataWrapper.CurrentBaseWeight = volume;

        return dataWrapper;
    }

    [Button]
    private void UpdateContainers()
    {
        playerContainer.dinamicPosibleItems = MultipleLists(allConsumables.ToArray());
        container.dinamicPosibleItems = MultipleLists(allConsumables.ToArray(), allStarters.ToArray(), allAccelerants.ToArray());
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
        return !(item is WaterItemSD) && !(item is SnowItemSD) && !(item is MeatItemSD);
    }

    [System.Flags]
    public enum Categories
    {
        None = 0,
        FireStarterSD = 1,
        FireFuelSD = 2,
        FireTinderSD = 4,

        All = FireStarterSD | FireFuelSD | FireTinderSD,
    }
}
[System.Serializable]
public class Container
{
    [InfoBox("Items которые точно будут в контейнере")]
    public List<ItemDataRandom> staticItems = new List<ItemDataRandom>();

    [MinMaxSlider(0, 32)]
    public Vector2Int itemsAddCount;
    [InfoBox("Items которые динамичные могут появится, а могут не появится")]
    [ReadOnly] public List<ItemSD> dinamicPosibleItems = new List<ItemSD>();


    public List<ItemDataWrapper> GetItems()
    {
        List<ItemDataWrapper> items = new List<ItemDataWrapper>();

        for (int i = 0; i < staticItems.Count; i++)
        {
            items.Add(staticItems[i].RndData());
        }

        int itemsCount = itemsAddCount.RandomNumBtw();

        for (int i = 0; i < itemsCount; i++)
        {
            ItemDataWrapper data = new ItemDataWrapper();
            data.scriptableData = dinamicPosibleItems[UnityEngine.Random.Range(0, dinamicPosibleItems.Count)];
            items.Add(data.RndData());
        }

        return items;
    }

    public InventoryData GetInventoryData()
    {
        InventoryData data = new InventoryData()
        {
            items = GetItems().ToArray(),
        };

        return data;
    }
}