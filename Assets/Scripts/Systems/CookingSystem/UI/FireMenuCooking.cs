using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMenuCooking : FireMenu
{
    [SerializeField] private MenuChoose menu;

    private void Awake()
    {
        menu.onChoosen += Choosen;
    }

    public void Setup(List<Item> items)
    {
        menu.Setup(items);
    }

    private void Choosen(Item item)
    {

    }
}