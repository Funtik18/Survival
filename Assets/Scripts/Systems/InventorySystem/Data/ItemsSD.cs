using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/ItemsData")]
public class ItemsSD : SingletonScriptableObject<ItemsSD>
{
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<ItemSD> allItems = new List<ItemSD>();

    //

    [TitleGroup("Consuable Items")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<ConsuableItemSD> allConsuables = new List<ConsuableItemSD>();
    [HorizontalGroup("Consuable Items/Split")]
    [VerticalGroup("Consuable Items/Split/Left")]
    [AssetList(AutoPopulate = true)]
    [SerializeField] private List<PotionItemSD> allDrinks = new List<PotionItemSD>();
    [VerticalGroup("Consuable Items/Split/Right")]
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




    public ItemSD GetRandomItemSD()
    {
        return null;
    }
}