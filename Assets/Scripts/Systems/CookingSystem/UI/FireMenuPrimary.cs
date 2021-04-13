using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireMenuPrimary : FireMenu
{
    public UnityAction onBack;

    [Space]
    [SerializeField] private Pointer pointerFuel;
    [SerializeField] private Pointer pointerCooking;
    [SerializeField] private Pointer pointerBoliling;

    [Space]
    //[SerializeField] private FireMenuCooking menuCooking;
    [SerializeField] private FireMenuCooking menuCooking;
    [SerializeField] private FireMenuBoiling menuBoiling;

    private void Awake()
    {
        pointerFuel.AddPressListener(Fuel);
        pointerCooking.AddPressListener(Cooking);
        pointerBoliling.AddPressListener(Boiling);
    }

    public FireMenu Setup(Inventory inventory)
    {
        //List<Item> fuelItems =;
        List<Item> cookingItems = inventory.GetAllBySD<ConsuableItemSD>();
        //List<Item> boilingItems =;

        menuCooking.Setup(cookingItems);
        return this;
    }

    private void Fuel()
    {
        BackPrepare();
    }
    private void Cooking()
    {
        BackPrepare();
        menuCooking.OpenMenu();
    }
    private void Boiling()
    {
        BackPrepare();
        menuBoiling.OpenMenu();
    }

    private void BackPrepare()
    {
        pointerBack.RemoveAllPressListeners();
        pointerBack.AddPressListener(OpenMenu);
        pointerBack.AddPressListener(CloseAll);

        CloseMenu();
    }

    private void CloseAll()
    {
        menuBoiling.CloseMenu();
        menuCooking.CloseMenu();

        Back();
    }

    private void Back()
    {
        pointerBack.RemoveAllPressListeners();
        pointerBack.AddPressListener(onBack);
    }
}