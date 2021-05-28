using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowFireMenu : WindowUI
{
    public UnityAction onBack;

    [SerializeField] private TMPro.TextMeshProUGUI pointerUseText;
    [SerializeField] private CustomPointer pointerUse;
    [SerializeField] protected CustomPointer pointerBack;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI fireDurationText;
    [SerializeField] private TMPro.TextMeshProUGUI fireTemperatureText;
    [SerializeField] private Image fireIcon;
    [Space]
    [SerializeField] private FireMenuPrimary menuPrimary;
    [SerializeField] private FireMenu fireMenu;

    private Pointer BackPointer => pointerBack.pointer;

    private List<Item> fuelItems;
    private List<Item> cookingItems;
    private List<Item> boilingItems;

    private Inventory inventory;
    private FireBuilding fire;
    
    private Item item;
    private Item tool;
    float volume;

    private void Awake()
    {
        pointerUse.pointer.AddPressListener(UseItem);

        menuPrimary.onBack += Back;
        menuPrimary.onFuel += Fuel;
        menuPrimary.onCooking += Cooking;
        menuPrimary.onBoiling += Boiling;

        fireMenu.onUpdated += ItemUpdated;
    }

    public void Setup(Inventory inventory, FireBuilding camfireBuilding)
    {
        this.inventory = inventory;
        this.fire = camfireBuilding;

        fire.onFireDuration = FireDuration;
        fire.onFireTemperature = FireTempretature;
        fire.onFireTemperaturePercent = FireTemperaturePercent;


        fuelItems = inventory.GetAllBySD<FireFuelSD>();
        cookingItems = inventory.GetAllFood(true);
        boilingItems = inventory.GetAllBySD<ToolContainerItemSD>();

        if(fuelItems.Count > 0)
            menuPrimary.buttonFuel.Accept();
        else
            menuPrimary.buttonFuel.Reject();

        if (cookingItems.Count > 0 && !fire.IsFull)
            menuPrimary.buttonCooking.Reject();
        else
            menuPrimary.buttonCooking.Reject();

        if (boilingItems.Count > 0 && !fire.IsFull)
            menuPrimary.buttonBoliling.Accept();
        else
            menuPrimary.buttonBoliling.Reject();

        BackOnPrimaryMenu();
        ShowWindow();
    }


    private void UseItem()
    {
        if(item.itemData.scriptableData is ToolItemSD)
        {
            tool = item;
            List<Item> boiling = inventory.GetAllBoilLiquid();
            boiling.Insert(0, new Item(ItemsData.Instance.Snow));
            fireMenu.Setup(boiling);

            PreparationToBoil();
        }
        else
        {
            //передача айтема огню
            if (tool != null)
            {
                if (tool.itemData.scriptableData is ToolContainerItemSD toolContainer)
                {
                    //boiling
                    volume = toolContainer.volume;

                    if (volume > 0.5f && item.itemData.scriptableData is SnowItemSD)
                    {
                        HideWindow();

                        GeneralAvailability.PlayerUI.OpenExchander(0.5f, volume, 0.5f, AddItemByVolume, AddItemFull, Back);
                    }
                    else if (volume > 0.5f && item.itemData.CurrentBaseWeight > 0.5f)
                    {
                        float maxWeight = item.itemData.CurrentWeightRounded;

                        HideWindow();

                        GeneralAvailability.PlayerUI.OpenExchander(0.5f, maxWeight <= volume ? maxWeight : volume, 0.5f, AddItemByVolume, AddItemFull, Back);
                    }
                    else
                    {
                        AddItemFull();
                    }
                }
                else
                {
                    //cooking
                }
            }
            else
            {
                if (item.itemData.scriptableData is FireFuelSD fuelSD)
                {
                    fire.AddFuel(fuelSD);
                    inventory.RemoveItem(item, 1);
                }

            }
        }
    }
    private void AddItemByVolume(float liquedVolume)
    {
        CookingSlot slot = fire.GetEmptySlot();
        if (slot)
        {
            ItemDataWrapper wat = item.itemData;//вода и тд

            if (wat.scriptableData is SnowItemSD)
            {

            }
            else
            {
                if (liquedVolume >= wat.CurrentBaseWeight)
                {
                    liquedVolume = wat.CurrentBaseWeight;
                    inventory.RemoveItem(item, 1);
                }
                else
                {
                    wat.CurrentBaseWeight -= liquedVolume;
                }
            }

            ItemObjectLiquidContainer itemObjectLiquidContainer = fire.AddItemObjectOnSlot<ItemObjectLiquidContainer>(tool, slot);
            inventory.RemoveItem(tool, 1);

            itemObjectLiquidContainer.SetLiquid(wat.scriptableData, liquedVolume);

            itemObjectLiquidContainer.StartProccess();
        }

        Back();
    }
    private void AddItemFull()
    {
        AddItemByVolume(volume);
    }


    private void Fuel()
    {
        fireMenu.Setup(fuelItems);

        pointerUseText.text = "ADD";

        Preparation();
    }
    private void Cooking()
    {
        fireMenu.Setup(cookingItems);

        pointerUseText.text = "COOK";

        Preparation();
    }
    private void Boiling()
    {
        tool = null;

        fireMenu.Setup(boilingItems);

        pointerUseText.text = "BOIL";

        Preparation();
    }

    //подготовка что бы вернутся назад к главному меню
    private void Preparation()
    {
        menuPrimary.HideWindow();
        fireMenu.ShowWindow();

        //use
        pointerUse.OpenButton();

        //back
        BackPointer.RemoveAllPressListeners();
        BackPointer.AddPressListener(pointerUse.CloseButton);
        BackPointer.AddPressListener(fireMenu.HideWindow);
        BackPointer.AddPressListener(BackOnPrimaryMenu);
    }
    private void PreparationToBoil()
    {
        BackPointer.RemoveAllPressListeners();
        BackPointer.AddPressListener(Boiling);
    }


    private void BackOnPrimaryMenu()
    {
        menuPrimary.ShowWindow();

        BackPointer.RemoveAllPressListeners();
        BackPointer.AddPressListener(Back);
    }



    public void Out()
    {
        if (IsOpened)
        {
            HideWindow();

            Dispose();

            pointerUse.CloseButton();
            fireMenu.HideWindow();
            fireMenu.Clear();
        }
    }
    private void Back()
    {
        onBack?.Invoke();

        Dispose();

        pointerUse.CloseButton();
        fireMenu.HideWindow();
        fireMenu.Clear();
    }
    private void Dispose()
    {
        fire.ClearUIActions();
        fire = null;

        item = null;
        tool = null;
        volume = 0;
    }

    private void FireDuration(string duration)
    {
        fireDurationText.text = duration;
    }
    private void FireTempretature(string temperature)
    {
        fireTemperatureText.text = temperature;
    }
    private void FireTemperaturePercent(float percent)
    {
        fireIcon.fillAmount = percent;
    }


    private void ItemUpdated(Item item)
    {
        this.item = item;
    }
}