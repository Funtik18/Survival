using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class WindowFireMenu : WindowUI
{
    public UnityAction onBack;

    [SerializeField] private TMPro.TextMeshProUGUI pointerUseText;
    [SerializeField] private CustomPointer pointerUse;
    [SerializeField] protected CustomPointer pointerBack;
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

        fuelItems = inventory.GetAllBySD<FireFuelSD>();
        cookingItems = inventory.GetAllFood(true);
        boilingItems = inventory.GetAllBySD<ConsumableItemSD>();

        if(fuelItems.Count > 0)
            menuPrimary.buttonFuel.Accept();
        else
            menuPrimary.buttonFuel.Reject();

        if (cookingItems.Count > 0)
            menuPrimary.buttonCooking.Accept();
        else
            menuPrimary.buttonCooking.Reject();

        if (boilingItems.Count > 0)
            menuPrimary.buttonBoliling.Accept();
        else
            menuPrimary.buttonBoliling.Reject();

        BackOnPrimaryMenu();
        ShowWindow();
    }

    public void Out()
    {
        if (IsOpened)
        {
            HideWindow();

            fireMenu.Clear();
        }
    }


    private void UseItem()
    {
        if (fire.AddItem(item))
        {
            inventory.RemoveItem(item, 1);
        }

        Back();
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
        fireMenu.Setup(boilingItems);

        pointerUseText.text = "BOIL";

        Preparation();
    }

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

    private void BackOnPrimaryMenu()
    {
        menuPrimary.ShowWindow();

        BackPointer.RemoveAllPressListeners();
        BackPointer.AddPressListener(Back);
    }

    private void ItemUpdated(Item item)
    {
        this.item = item;
    }

    private void Back()
    {
        onBack?.Invoke();

        fireMenu.HideWindow();
        fireMenu.Clear();
    }
}