using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WindowFireMenu : WindowUI
{
    public UnityAction onBack;

    [SerializeField] private CustomPointer pointerUse;
    [SerializeField] protected CustomPointer pointerBack;
    [Space]
    [SerializeField] private FireMenuPrimary menuPrimary;
    [SerializeField] private FireMenuCooking menuCooking;
    [SerializeField] private FireMenuBoiling menuBoiling;

    private Pointer BackPointer => pointerBack.pointer;

    private FireBuilding fire;

    private Item cookingItem;

    private void Awake()
    {
        pointerUse.pointer.AddPressListener(UseItem);

        menuPrimary.onBack += Back;
        menuPrimary.onFuel += Fuel;
        menuPrimary.onCooking += Cooking;
        menuPrimary.onBoiling += Boiling;

        menuCooking.onUpdated += CookingUpdated;
    }

    public void Setup(Inventory inventory, FireBuilding camfireBuilding)
    {
        this.fire = camfireBuilding;

        List<Item> fuelItems = inventory.GetAllBySD<ConsumableItemSD>();
        List<Item> cookingItems = inventory.GetAllFood(true);
        List<Item> boilingItems = inventory.GetAllBySD<ConsumableItemSD>();

        menuPrimary.buttonFuel.Reject();

        if (cookingItems.Count > 0)
        {
            menuPrimary.buttonCooking.Accept();
            menuCooking.Setup(cookingItems);
        }
        else
        {
            menuPrimary.buttonCooking.Reject();
        }
        menuPrimary.buttonBoliling.Reject();


        BackOnPrimaryMenu();
        ShowWindow();
    }

    private void UseItem()
    {
        if (fire.AddItem(cookingItem) == false)
            Debug.LogError("Can not cook it");
        Back();
    }


    private void Fuel()
    {

    }
    private void Cooking()
    {
        menuPrimary.HideWindow();
        menuCooking.ShowWindow();

        //use
        pointerUse.OpenButton();
        
        //back
        BackPointer.RemoveAllPressListeners();
        BackPointer.AddPressListener(pointerUse.CloseButton);
        BackPointer.AddPressListener(menuCooking.HideWindow);
        BackPointer.AddPressListener(BackOnPrimaryMenu);
    }
    private void Boiling()
    {
        menuPrimary.HideWindow();
        menuBoiling.ShowWindow();

        //use
        pointerUse.OpenButton();

        //back
        BackPointer.RemoveAllPressListeners();
        BackPointer.AddPressListener(pointerUse.CloseButton);
        BackPointer.AddPressListener(menuBoiling.HideWindow);
        BackPointer.AddPressListener(BackOnPrimaryMenu);
    }

    private void BackOnPrimaryMenu()
    {
        menuPrimary.ShowWindow();

        BackPointer.RemoveAllPressListeners();
        BackPointer.AddPressListener(Back);
    }

    private void CookingUpdated(Item item)
    {
        cookingItem = item;
    }
    private void BoilingUpdated(Item item)
    {
        //cookingItem = item;
    }

    private void Back()
    {
        onBack?.Invoke();

        menuCooking.Clear();
    }
}