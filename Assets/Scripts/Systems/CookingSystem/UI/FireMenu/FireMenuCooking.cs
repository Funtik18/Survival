using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireMenuCooking : WindowUI
{
    public UnityAction<Item> onUpdated;

    [SerializeField] private MenuChoose menu;
    [SerializeField] private TMPro.TextMeshProUGUI itemName;

    private void Awake()
    {
        menu.onChoosen += Choosen;
    }

    public void Setup(List<Item> items)
    {
        menu.Setup(items);
    }

    public void Clear()
    {
        menu.Clear();
    }

    private void Choosen(Item item)
    {
        itemName.text = item.itemData.scriptableData.objectName;
        onUpdated?.Invoke(item);
    }
}