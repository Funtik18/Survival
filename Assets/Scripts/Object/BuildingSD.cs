using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "BuildingSD", menuName = "Buildings/Building")]
public class BuildingSD : ObjectSD
{
    [PreviewField]
    public Sprite buildingSprite;
    [AssetList]
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public BuildingObject model;

    public bool isFromInventory = false;
    [ShowIf("isFromInventory")]
    public bool isFromInventoryComplex = false;
    [ShowIf("IsFromInventoryComplex")]
    public bool isTypes = false;

    [ShowIf("IsTypes")]
    public ItemsData.Categories categories;

    [ShowIf("IsListItems")]
    public List<BlueprintItem> items = new List<BlueprintItem>();

    [ShowIf("IsItem")]
    public BlueprintItem item;
    [ShowIf("IsExchanger")]
    public bool exchangeAfterBuild = false;

    private bool IsExchanger => IsItem || IsListItems;
    private bool IsFromInventoryComplex => isFromInventory && isFromInventoryComplex;
    private bool IsTypes => IsFromInventoryComplex && isTypes;
    public bool IsListItems => IsFromInventoryComplex && !isTypes;
    public bool IsItem => isFromInventory && !isFromInventoryComplex;


    //[TypeFilter("GetFilteredTypeList")]
    //public List<ItemSD> types = new List<ItemSD>();
    //private IEnumerable<Type> GetFilteredTypeList()
    //{
    //    var q = typeof(ItemSD).Assembly.GetTypes()
    //        .Where(x => !x.IsAbstract)// Excludes ItemSD
    //        .Where(x => typeof(ItemSD).IsAssignableFrom(x));// Excludes classes not inheriting from ItemSD
    //    return q;
    //}
}